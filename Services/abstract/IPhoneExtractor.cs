namespace Services.Phones;

public interface IPhoneExtractor
{
    /// <summary>Извлечь все кандидаты телефонов из текста (как есть, без нормализации).</summary>
    IEnumerable<string> ExtractCandidates(string? text);
}