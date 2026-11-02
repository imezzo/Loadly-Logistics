using System.Text.RegularExpressions;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Shared.Utilities;




public static class IdentifierValidator
{



    public static bool IsValidForChannel(string identifier, CommunicationChannel channel)
    {
        if (string.IsNullOrWhiteSpace(identifier))
            return false;

        return channel switch
        {
            CommunicationChannel.Email => IsValidEmail(identifier),
            CommunicationChannel.Sms => IsValidPhone(identifier),
            _ => false
        };
    }




    public static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase);
            return emailRegex.IsMatch(email);
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


        var cleanPhone = phone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace("+", "");


        return cleanPhone.Length >= 8 && cleanPhone.All(char.IsDigit);
    }




    public static string GetErrorMessage(CommunicationChannel channel)
    {
        return channel switch
        {
            CommunicationChannel.Email => "Please provide a valid email address when using email channel",
            CommunicationChannel.Sms => "Please provide a valid phone number when using SMS channel",
            _ => "Invalid identifier for the selected channel"
        };
    }
}

