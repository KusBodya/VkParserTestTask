namespace Services.Phones;

public interface IKeywordProvider
{
    IReadOnlyList<string> GetKeywords();
}