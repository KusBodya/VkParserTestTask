using Domain.Enums;
using Services.DeepInfra;
using Services.Phones;

namespace Services.Classification;

using System.Text.Json;
using Microsoft.Extensions.Logging;

/// <summary>Классификация релевантности поста через DeepInfra.</summary>
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
    Классифицируй текст объявления о недвижимости. Верни JSON:
    {{
            ""is_relevant"": bool,
            ""score"": float, 
            ""intent"": ""sell|buy|rent_out|rent_want|unknown"",
            ""property"": ""apartment|house|room|land|commercial|unknown""
    }}
            Текст: <<< {text} >>>";

        var json = await _client.ChatJsonAsync(system, user, ct);

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            bool rel = root.TryGetProperty("is_relevant", out var r) && r.GetBoolean();
            float score = root.TryGetProperty("score", out var s) ? (float)s.GetDouble() : 0.0f;

            IntentType intent = root.TryGetProperty("intent", out var i)
                ? i.GetString()?.ToLowerInvariant() switch
                {
                    "sell" => IntentType.Sell,
                    "buy" => IntentType.Buy,
                    "rent_out" => IntentType.RentOut,
                    "rent_want" => IntentType.RentWant,
                    _ => IntentType.Unknown
                }
                : IntentType.Unknown;

            PropertyType prop = root.TryGetProperty("property", out var p)
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

            return new(rel, score, intent, prop);
        }
        catch (Exception ex)
        {
            _log.LogWarning(ex, "Failed to parse DeepInfra JSON: {Json}", json);
            return new(false, 0f, IntentType.Unknown, PropertyType.Unknown);
        }
    }
}