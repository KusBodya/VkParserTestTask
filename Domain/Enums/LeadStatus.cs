namespace Domain.Enums;

/// <summary>Статус лида в нашей системе.</summary>
public enum LeadStatus
{
    New = 0,
    Validated = 1,
    Contacted = 2,
    Converted = 3,
    Duplicate = 4,
    Rejected = 5
}