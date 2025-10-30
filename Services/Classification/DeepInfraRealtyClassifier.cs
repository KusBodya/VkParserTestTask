#nullable enable

using System.Text.Json;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Services.DeepInfra;
using Services.Phones;

namespace Services.Classification;

/// <summary>Классифицирует посты о недвижимости с помощью DeepInfra.</summary>
public sealed class DeepInfraRealtyClassifier : IRealtyClassifier
{
    private readonly DeepInfraClient _client;
    private readonly ILogger<DeepInfraRealtyClassifier> _log;

    public DeepInfraRealtyClassifier(DeepInfraClient client, ILogger<DeepInfraRealtyClassifier> log)
    {
        _client = client;
        _log = log;
    }

    public async Task<ClassificationResult> ClassifyAsync(string text, CancellationToken ct = default)
    {
        var system = "You are a classifier for Russian real-estate posts. Return strict JSON only.";
        var user = @$"
Проанализируй объявление и ответь JSON объектом в формате:
{{
    ""is_relevant"": true|false,
    ""score"": число от 0 до 1,
    ""intent"": ""sell|buy|rent_out|rent_want|unknown"",
    ""property"": ""apartment|house|room|land|commercial|unknown""
}}
Если текст не относится к сделкам с недвижимостью, укажи is_relevant=false.
Текст объявления: <<< {text} >>>";

        _log.LogInformation("DeepInfra prompt: {Prompt}", user);
        var json = await _client.ChatJsonAsync(system, user, ct);
        _log.LogInformation("DeepInfra raw JSON: {Json}", json);

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            var rel = root.TryGetProperty("is_relevant", out var r) && r.GetBoolean();
            var score = root.TryGetProperty("score", out var s) ? (float)s.GetDouble() : 0f;

            var intent = root.TryGetProperty("intent", out var i)
                ? i.GetString()?.ToLowerInvariant() switch
                {
                    "sell" => IntentType.Sell,
                    "buy" => IntentType.Buy,
                    "rent_out" => IntentType.RentOut,
                    "rent_want" => IntentType.RentWant,
                    _ => IntentType.Unknown
                }
                : IntentType.Unknown;

            var property = root.TryGetProperty("property", out var p)
                ? p.GetString()?.ToLowerInvariant() switch
                {
                    "apartment" => PropertyType.Apartment,
                    "house" => PropertyType.House,
                    "room" => PropertyType.Room,
                    "land" => PropertyType.Land,
                    "commercial" => PropertyType.Commercial,
                    _ => PropertyType.Unknown
                }
                : PropertyType.Unknown;

            return new ClassificationResult(rel, score, intent, property, null, json);
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "Failed to parse DeepInfra JSON: {Json}", json);
            return new ClassificationResult(false, 0f, IntentType.Unknown, PropertyType.Unknown, null, json);
        }
    }
}
