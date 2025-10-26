using Domain.Enums;

namespace Domain;

#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>Агрегированный лид — то, что отдаём через API.</summary>
public class Lead
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    public LeadSource Source { get; set; } = LeadSource.Vk;

    public Guid PostIdFk { get; set; }
    public VkPost? Post { get; set; }

    public Guid AuthorId { get; set; }
    public VkAuthor? Author { get; set; }

    [MaxLength(32)] public string? PrimaryPhoneE164 { get; set; }

    public List<string> AllPhonesE164 { get; set; } = new();

    public IntentType Intent { get; set; } = IntentType.Unknown;
    public PropertyType PropertyType { get; set; } = PropertyType.Unknown;

    [MaxLength(128)] public string? City { get; set; }

    public LeadStatus Status { get; set; } = LeadStatus.New;

    /// <summary>Причина отклонения/комментарий модератора.</summary>
    [MaxLength(512)]
    public string? Notes { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    [NotMapped] public string? PostUrl => Post?.PostUrl;

    [NotMapped] public string? AuthorUrl => Author?.ProfileUrl;
}