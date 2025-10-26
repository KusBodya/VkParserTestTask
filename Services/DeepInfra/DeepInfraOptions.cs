namespace Services.DeepInfra;

public sealed class DeepInfraOptions
{
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Напр.: "google/gemini-2.0-flash-exp" или другая совместимая модель.</summary>
    public string Model { get; set; } = "google/gemini-2.0-flash-exp";

    public Uri BaseUri { get; set; } = new("https://api.deepinfra.com/v1/openai/");
}