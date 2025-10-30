namespace Services.Phones;

/// <summary>Поставщик стандартных ключевых фраз для поиска объявлений.</summary>
public sealed class KeywordProvider : IKeywordProvider
{
    private static readonly string[] Defaults =
    {
        "продам квартиру", "продам комнату", "продам дом", "продам участок",
        "куплю квартиру", "куплю комнату", "куплю дом", "куплю участок",
        "сдам квартиру", "сдам комнату", "сдам дом",
        "сниму квартиру", "сниму комнату", "сниму дом",
        "аренда квартиры", "аренда комнаты", "аренда дома"
    };

    public IReadOnlyList<string> GetKeywords() => Defaults;
}
