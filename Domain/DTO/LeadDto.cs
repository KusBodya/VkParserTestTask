namespace Domain.DTO;

/// <summary>Контракт ответа API по ТЗ.</summary>
public sealed record LeadDto(
    string Phone,
    string PostUrl,
    string AuthorUrl
);