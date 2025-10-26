namespace Services.Vk;

public sealed class VkOptions
{
    public string? AccessToken { get; set; }
    public string City { get; set; } = "Москва";
    public int LookbackHours { get; set; } = 12;
    public int PerKeywordLimit { get; set; } = 100;
}
