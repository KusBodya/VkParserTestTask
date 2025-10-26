using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain;

/// <summary>Результат LLM-классификации поста (DeepInfra).</summary>
public class PostAnalysis
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public Guid PostIdFk { get; set; }
    public VkPost? Post { get; set; }

    public bool IsRelevant { get; set; }
    public float? RelevanceScore { get; set; }

    public IntentType Intent { get; set; } = IntentType.Unknown;
    public PropertyType PropertyType { get; set; } = PropertyType.Unknown;

    /// <summary>Все найденные телефоны (как в тексте).</summary>
    public List<string> PhonesRaw { get; set; } = new();

    /// <summary>Нормализованные телефоны (E.164).</summary>
    public List<string> PhonesE164 { get; set; } = new();

    /// <summary>Трассировка: промпт/ответ модели (для отладки).</summary>
    public string? ModelTrace { get; set; }

    /// <summary>Название модели.</summary>
    public string? ModelName { get; set; }

    /// <summary>Провайдер (по умолчанию DeepInfra).</summary>
    public string Provider { get; set; } = "DeepInfra";

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}