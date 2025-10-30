#nullable enable

using Domain.Enums;

namespace Services.Classification;

/// <summary>Результат классификации поста сервисом DeepInfra.</summary>
public sealed record ClassificationResult(
    bool IsRelevant,
    float Score,
    IntentType Intent,
    PropertyType Property,
    string? ModelName = null,
    string? RawJson = null
);
