namespace Services.Phones;

using System.Text.RegularExpressions;

public sealed class PhoneExtractor : IPhoneExtractor
{
    // Примерно: +7 999 123-45-67, 8 (999) 123 45 67, 999-123-45-67
    private static readonly Regex PhoneRegex = new(
        pattern: @"(?<!\d)(?:\+?\s?7|8)?[\s\-\(]*\d{3}[\s\-\)]*\d{3}[\s\-]*\d{2}[\s\-]*\d{2}(?!\d)",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

    public IEnumerable<string> ExtractCandidates(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) yield break;
        foreach (Match m in PhoneRegex.Matches(text))
        {
            var s = m.Value.Trim();
            if (!string.IsNullOrWhiteSpace(s))
                yield return s;
        }
    }
}