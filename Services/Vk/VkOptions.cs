namespace Services.Vk;

/// <summary>Настройки доступа к VK API и фильтров поиска.</summary>
public sealed class VkOptions
{
    public string? AccessToken { get; set; }
    public string City { get; set; } = string.Empty;
    public int LookbackHours { get; set; } = 12;
    public int PerKeywordLimit { get; set; } = 100;
}
