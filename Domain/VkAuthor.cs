using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

/// <summary>Автор постов VK, откуда извлекаем лиды.</summary>
public class VkAuthor
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public long VkOwnerId { get; set; }

    /// <summary>Отображаемое имя автора, если удалось получить.</summary>
    [MaxLength(256)]
    public string? DisplayName { get; set; }

    /// <summary>Уникальный screen_name при наличии.</summary>
    [MaxLength(128)]
    public string? ScreenName { get; set; }

    /// <summary>Постоянная ссылка на профиль или группу автора.</summary>
    [NotMapped]
    public string ProfileUrl => ScreenName is { Length: > 0 }
        ? $"https://vk.com/{ScreenName}"
        : (VkOwnerId >= 0
            ? $"https://vk.com/id{VkOwnerId}"
            : $"https://vk.com/public{Math.Abs(VkOwnerId)}");

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
