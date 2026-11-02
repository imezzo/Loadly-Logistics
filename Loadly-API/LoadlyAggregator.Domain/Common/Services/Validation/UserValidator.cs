using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Common.Services.Validation;

public class UserValidator
{
    public static void ValidateUserExists<T>(T? user, string userType) where T : class =>
        _ = user ?? throw new UnauthorizedException("auth.login_failed");

    public static void ValidatePassword(IPasswordHashingService passwordHashingService, string providedPassword, string hashedPassword) =>
        _ = passwordHashingService.VerifyPassword(providedPassword, hashedPassword)
            ? true
            : throw new UnauthorizedException("auth.login_failed");

    public static void ValidateStatus(UserStatus status) =>
        _ = status switch
        {
            UserStatus.ACTIVE => true,
            _ => throw new UnauthorizedException("auth.account_not_active")
        };

    public static void ValidateEmailVerified(bool emailVerified) =>
        _ = emailVerified
            ? true
            : throw new UnauthorizedException("auth.email_not_verified");

    public static void ValidatePhoneVerified(bool phoneVerified) =>
        _ = phoneVerified
            ? true
            : throw new UnauthorizedException("auth.phone_not_verified");

    public static void ValidateMfaEnabled(CommunicationChannel mfaChannel) =>
        _ = mfaChannel switch
        {
            CommunicationChannel.None => throw new ValidationException("auth.mfa_not_enabled"),
            _ => true
        };

    public static void ValidateMfaChannel(CommunicationChannel channel) =>
        _ = Enum.IsDefined(typeof(CommunicationChannel), channel)
            ? true
            : throw new ValidationException("auth.mfa_channel_invalid");

    public static void ValidateChannelVerified(CommunicationChannel channel, bool emailVerified, bool phoneVerified) =>
        _ = (channel, emailVerified, phoneVerified) switch
        {
            (CommunicationChannel.Email, false, _) => throw new ValidationException("auth.email_mfa_not_verified"),
            (CommunicationChannel.Sms, _, false) => throw new ValidationException("auth.phone_mfa_not_verified"),
            _ => true
        };
}
