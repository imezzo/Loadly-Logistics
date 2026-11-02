namespace LoadlyAggregator.Domain.Common.Services.Mfa;

public static class MfaRecipientResolver
{
    public static string GetRecipient(string method, string email, string phone)
    {
        return method == "email" ? email : phone;
    }
}
