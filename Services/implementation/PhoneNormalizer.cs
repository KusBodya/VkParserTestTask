namespace Services.Phones;

using System.Text;

/// <summary>Простая нормализация в E.164 без внешних либ (достаточно для RU/СНГ).</summary>
public sealed class PhoneNormalizer : IPhoneNormalizer
{
    public string? ToE164(string input, string defaultCountry = "RU")
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        var digits = new StringBuilder();
        foreach (var ch in input)
        {
            if (char.IsDigit(ch)) digits.Append(ch);
            else if (ch == '+' && digits.Length == 0) digits.Append('+');
        }

        var s = digits.ToString();
        if (s.Length == 0) return null;

        if (s[0] != '+')
        {
            if (defaultCountry == "RU" && s.Length == 11 && s.StartsWith("8"))
                s = "+7" + s[1..];
            else if (defaultCountry == "RU" && s.Length == 10)
                s = "+7" + s;
            else
                s = "+" + s;
        }

        var onlyDigits = s.TrimStart('+');
        if (onlyDigits.Length < 7 || onlyDigits.Length > 15) return null;
        return "+" + onlyDigits;
    }
}