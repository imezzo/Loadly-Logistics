using System.Text.RegularExpressions;
using FluentValidation;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.API.Validators.Extensions;

public static class CollectorAuthChannelValidationRules
{
    private static readonly Regex UsernamePattern = new("^[a-zA-Z0-9_]+$", RegexOptions.Compiled);

    public static IRuleBuilderOptions<T, string> IsValidCollectorLogin<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        int minLength = 3)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.login_required")
            .MinimumLength(minLength).WithMessage($"validation.login_min_length|{minLength}")
            .Must(login => !IdentifierValidator.IsValidEmail(login.Trim()))
            .WithMessage("collectors.login_email_not_allowed")
            .Must(login => IsValidUsernameOrPhone(login.Trim()))
            .WithMessage("collectors.login_must_be_username_or_phone");
    }

    public static IRuleBuilderOptions<T, CommunicationChannel> IsValidCollectorDeliveryChannel<T>(
        this IRuleBuilder<T, CommunicationChannel> ruleBuilder)
    {
        return ruleBuilder
            .Must(channel => channel == CommunicationChannel.Sms)
            .WithMessage("collectors.delivery_channel_sms_only");
    }

    public static IRuleBuilderOptions<T, CommunicationChannel> IsValidCollectorMfaChannel<T>(
        this IRuleBuilder<T, CommunicationChannel> ruleBuilder)
    {
        return ruleBuilder
            .Must(channel => channel is CommunicationChannel.None or CommunicationChannel.Sms)
            .WithMessage("collectors.mfa_channel_none_or_sms_only");
    }

    private static bool IsValidUsernameOrPhone(string login) =>
        IdentifierValidator.IsValidPhone(login)
        || (login.Length >= 3 && UsernamePattern.IsMatch(login));
}
