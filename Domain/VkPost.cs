using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

/// <summary>Пост ВКонтакте, который мы анализируем на предмет лидов.</summary>
public class VkPost
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Идентификатор владельца (пользователь &gt; 0, сообщество &lt; 0).</summary>
    public long OwnerId { get; set; }

    /// <summary>Идентификатор поста внутри стены владельца.</summary>
    public long PostId { get; set; }

    /// <summary>Прямая ссылка на пост.</summary>
    [NotMapped]
    public string PostUrl => $"https://vk.com/wall{OwnerId}_{PostId}";

    /// <summary>Дата публикации в UTC.</summary>
    public DateTimeOffset PostedAt { get; set; }

    /// <summary>Текст поста в исходном виде.</summary>
    [Column(TypeName = "text")]
    public string? Text { get; set; }

    /// <summary>Город из фильтров поиска, если он указан.</summary>
    [MaxLength(128)]
    public string? City { get; set; }

    /// <summary>FK на автора поста.</summary>
    public Guid AuthorId { get; set; }

    public VkAuthor? Author { get; set; }

    /// <summary>Служебный тег, фиксирующий источник или волну загрузки.</summary>
    [MaxLength(256)]
    public string? IngestionTag { get; set; }

    /// <summary>Сырой JSON ответа VK (хранится как jsonb).</summary>
    public string? RawJson { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
}
