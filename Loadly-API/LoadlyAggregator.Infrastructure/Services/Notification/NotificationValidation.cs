using System.Text.RegularExpressions;

namespace LoadlyAggregator.Infrastructure.Services.Notification;

public static class NotificationValidation
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex PhoneRegex = new(
        @"^\+?[\d\s\-()]{10,20}$",
        RegexOptions.Compiled);

    public static bool IsValidEmail(string? email) =>
        !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email.Trim());

    public static bool IsValidPhone(string? phone) =>
        !string.IsNullOrWhiteSpace(phone) && PhoneRegex.IsMatch(phone.Replace(" ", ""));

    public static bool IsValidDeviceToken(string? token) =>
        !string.IsNullOrWhiteSpace(token) && token.Trim().Length >= 10;
}
