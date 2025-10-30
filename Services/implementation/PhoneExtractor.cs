using System.Text.RegularExpressions;

namespace Services.Phones;

/// <summary>Извлекает телефонные номера из текста объявлений.</summary>
public sealed class PhoneExtractor : IPhoneExtractor
{
    private static readonly Regex PhoneRegex = new(
        pattern: @"(?<!\d)(?:\+?\s?7|8)?[\s\-\(]*\d{3}[\s\-\)]*\d{3}[\s\-]*\d{2}[\s\-]*\d{2}(?!\d)",
        options: RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Multiline);

    public IEnumerable<string> ExtractCandidates(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            yield break;
        }

        foreach (Match m in PhoneRegex.Matches(text))
        {
            var s = m.Value.Trim();
            if (!string.IsNullOrWhiteSpace(s))
            {
                yield return s;
            }
        }
    }
}