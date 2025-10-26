using Domain.Enums;

namespace Services.Phones;

public sealed record ClassificationResult(
    bool IsRelevant,
    float Score,
    IntentType Intent,
    PropertyType Property
);