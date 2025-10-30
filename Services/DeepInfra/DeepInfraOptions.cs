namespace Services.DeepInfra;

/// <summary>Конфигурация доступа к API DeepInfra.</summary>
public sealed class DeepInfraOptions
{
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>Название модели, например "google/gemini-2.0-flash-exp".</summary>
    public string Model { get; set; } = "google/gemini-2.0-flash-exp";

    public Uri BaseUri { get; set; } = new("https://api.deepinfra.com/v1/openai/");
}
