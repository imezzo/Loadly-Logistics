using System.Text.RegularExpressions;
namespace LoadlyAggregator.Shared.Utilities;

public static class ValidationHelper
{
    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
    public static bool IsValidPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        var phonePattern = @"^\+?[\d\s\-\(\)]{7,20}$";
        if (!Regex.IsMatch(phone, phonePattern))
            return false;

        return phone.Count(char.IsDigit) >= 7;
    }
    public static bool IsValidEgyptianPhone(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
            return false;

        string digitsOnly = string.Concat(phone.Where(char.IsDigit));



        if (digitsOnly.Length == 11 && digitsOnly.StartsWith("01"))
        {
            char thirdDigit = digitsOnly[2];
            return thirdDigit is '0' or '1' or '2' or '5';
        }

        if (!phone.TrimStart().StartsWith('+') || !digitsOnly.StartsWith("20"))
            return false;
        string nationalNumber = digitsOnly[2..];

        if (nationalNumber.Length == 10 && nationalNumber.StartsWith('1'))
        {
            char thirdDigit = nationalNumber[1];
            return thirdDigit is '0' or '1' or '2' or '5';
        }
        return false;
    }
}
