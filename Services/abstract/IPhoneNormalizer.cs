namespace Services.Phones;

public interface IPhoneNormalizer
{
    /// <summary>Вернуть E.164 телефон (+79991234567) или null, если не удалось.</summary>
    string? ToE164(string input, string defaultCountry = "RU");
}