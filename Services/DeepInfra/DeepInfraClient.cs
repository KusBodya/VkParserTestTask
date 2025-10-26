namespace Services.DeepInfra;

using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>Минимальный OpenAI-совместимый клиент под DeepInfra Chat Completions.</summary>
public sealed class DeepInfraClient
{
    private readonly HttpClient _http;
    private readonly DeepInfraOptions _opt;
    private readonly ILogger<DeepInfraClient> _log;

    public DeepInfraClient(HttpClient http, IOptions<DeepInfraOptions> opt, ILogger<DeepInfraClient> log)
    {
        _http = http;
        _opt = opt.Value;
        _log = log;

        _http.BaseAddress = _opt.BaseUri; // .../v1/openai/
        if (!_http.DefaultRequestHeaders.Contains("Authorization"))
            _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_opt.ApiKey}");
    }

    public async Task<string> ChatJsonAsync(string system, string user, CancellationToken ct = default)
    {
        var req = new
        {
            model = _opt.Model,
            response_format = new { type = "json_object" },
            messages = new object[]
            {
                new { role = "system", content = system },
                new { role = "user", content = user }
            },
            temperature = 0.2
        };

        using var resp = await _http.PostAsJsonAsync("chat/completions", req, ct);
        resp.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync(ct));
        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return content ?? "{}";
    }
}