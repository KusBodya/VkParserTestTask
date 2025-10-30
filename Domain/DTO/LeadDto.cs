namespace Domain.DTO;

/// <summary>Упрощённый DTO лида, который возвращает API.</summary>
public sealed record LeadDto(
    string Phone,
    string PostUrl,
    string AuthorUrl
);
