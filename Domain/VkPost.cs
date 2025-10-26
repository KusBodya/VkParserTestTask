namespace Domain;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>Пост из VK (newsfeed.search результат).</summary>
public class VkPost
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>VK owner_id (пользователь > 0, сообщество < 0).</summary>
    public long OwnerId { get; set; }

    /// <summary>VK post_id внутри владельца.</summary>
    public long PostId { get; set; }

    /// <summary>Полная ссылка на пост.</summary>
    [NotMapped]
    public string PostUrl => $"https://vk.com/wall{OwnerId}_{PostId}";

    /// <summary>Время публикации (UTC).</summary>
    public DateTimeOffset PostedAt { get; set; }

    /// <summary>Сырой текст поста.</summary>
    [MaxLength(8000)]
    public string? Text { get; set; }

    /// <summary>В каком городе искали/нашли пост.</summary>
    [MaxLength(128)]
    public string? City { get; set; }

    /// <summary>FK на автора.</summary>
    public Guid AuthorId { get; set; }

    public VkAuthor? Author { get; set; }

    /// <summary>Короткая метка источника (запрос + город) для отладки.</summary>
    [MaxLength(256)]
    public string? IngestionTag { get; set; }

    /// <summary>Сырые данные VK (json), полезно хранить как jsonb в БД.</summary>
    public string? RawJson { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}