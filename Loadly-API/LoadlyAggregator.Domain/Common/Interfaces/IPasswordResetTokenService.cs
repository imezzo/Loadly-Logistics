using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;




public interface IPasswordResetTokenService
{



    string GenerateResetToken(
        long userId,
        string userType,
        CommunicationChannel channel = CommunicationChannel.Email,
        int expirationHours = 2
    );




    PasswordResetTokenData? ValidateResetToken(string token);
}




public class PasswordResetTokenData
{
    public long UserId { get; set; }
    public string UserType { get; set; } = string.Empty;
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;
    public DateTime IssuedAt { get; set; }
}

