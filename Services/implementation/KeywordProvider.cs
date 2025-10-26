namespace Services.Phones;

public sealed class KeywordProvider : IKeywordProvider
{
    private static readonly string[] Defaults =
    {
        "продам квартиру", "продам дом", "продам комнату", "продам участок",
        "куплю квартиру", "куплю дом", "куплю участок",
        "сдам квартиру", "сдам дом", "сдам комнату",
        "сниму квартиру", "сниму комнату", "аренда квартиры", "аренда комнаты",
        "долгосрочная аренда", "краткосрочная аренда"
    };

    public IReadOnlyList<string> GetKeywords() => Defaults;
}