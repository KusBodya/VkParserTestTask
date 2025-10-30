using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain;

/// <summary>Результат анализа поста языковой моделью DeepInfra.</summary>
public class PostAnalysis
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PostIdFk { get; set; }
    public VkPost? Post { get; set; }

    public bool IsRelevant { get; set; }
    public float? RelevanceScore { get; set; }

    public IntentType Intent { get; set; } = IntentType.Unknown;
    public PropertyType PropertyType { get; set; } = PropertyType.Unknown;

    /// <summary>Сырые телефоны, извлечённые из текста поста.</summary>
    public List<string> PhonesRaw { get; set; } = new();

    /// <summary>Телефоны в формате E.164.</summary>
    public List<string> PhonesE164 { get; set; } = new();

    /// <summary>Диагностическая информация о работе модели.</summary>
    public string? ModelTrace { get; set; }

    /// <summary>Название модели, использованной для классификации.</summary>
    public string? ModelName { get; set; }

    /// <summary>Поставщик модели, применившейся при анализе.</summary>
    public string Provider { get; set; } = "DeepInfra";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
