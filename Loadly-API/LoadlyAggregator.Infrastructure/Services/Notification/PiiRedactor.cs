using System.Linq;
using System.Text.RegularExpressions;

namespace LoadlyAggregator.Infrastructure.Services.Notification;

public static class PiiRedactor
{
    private static readonly Regex EmailRegex = new(@"^([^@])([^@]*)@(.+)$", RegexOptions.Compiled);

    public static string RedactEmail(string? email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return "[empty]";
        }

        var match = EmailRegex.Match(email);
        return match.Success
            ? $"{match.Groups[1].Value}***@{match.Groups[3].Value}"
            : "***@***";
    }

    public static string RedactPhone(string? phone)
    {
        if (string.IsNullOrEmpty(phone))
        {
            return "[empty]";
        }

        var digits = new string(phone.Where(char.IsDigit).ToArray());
        return digits.Length <= 4 ? "****" : $"***{digits[^4..]}";
    }

    public static string RedactToken(string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return "[empty]";
        }

        return token.Length <= 8 ? "***" : $"{token[..4]}...{token[^4..]}";
    }
}
