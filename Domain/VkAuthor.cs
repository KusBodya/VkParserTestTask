namespace Domain;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class VkAuthor
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public long VkOwnerId { get; set; }

    /// <summary>Отображаемое имя/название (может быть пустым, дополняем позже).</summary>
    [MaxLength(256)]
    public string? DisplayName { get; set; }

    /// <summary>Screen_name, если известен (даёт человекочитаемую ссылку).</summary>
    [MaxLength(128)]
    public string? ScreenName { get; set; }

    /// <summary>Прямая ссылка на профиль/сообщество.</summary>
    [NotMapped]
    public string ProfileUrl => ScreenName is { Length: > 0 }
        ? $"https://vk.com/{ScreenName}"
        : (VkOwnerId >= 0
            ? $"https://vk.com/id{VkOwnerId}"
            : $"https://vk.com/public{Math.Abs(VkOwnerId)}");

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}