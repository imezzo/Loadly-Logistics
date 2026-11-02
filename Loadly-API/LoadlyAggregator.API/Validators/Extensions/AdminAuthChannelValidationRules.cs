using FluentValidation;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.API.Validators.Extensions;

public static class AdminAuthChannelValidationRules{
    /// <summary>Stored <c>admins.mfa_channel</c> on create/update — MFA off or SMS only.</summary>
    public static IRuleBuilderOptions<T, CommunicationChannel> MustBeAdminMfaChannel<T>(
        this IRuleBuilder<T, CommunicationChannel> ruleBuilder,
        string propertyName = "MfaChannel")
    {
        return ruleBuilder
            .IsValidEnum<T, CommunicationChannel>(propertyName)
            .Must(c => c is CommunicationChannel.None or CommunicationChannel.Sms)
            .WithMessage("validation.admin_mfa_channel_must_be_none_or_sms");
    }

    /// <summary>Per-request delivery channel (forget-password, send-mfa-code, enable-mfa) — SMS only.</summary>
    public static IRuleBuilderOptions<T, CommunicationChannel> MustBeAdminDeliveryChannel<T>(
        this IRuleBuilder<T, CommunicationChannel> ruleBuilder,
        string propertyName = "Channel")
    {
        return ruleBuilder
            .IsValidEnum<T, CommunicationChannel>(propertyName)
            .Must(c => c == CommunicationChannel.Sms)
            .WithMessage("validation.admin_channel_must_be_sms");
    }

    /// <summary>Admin login identifier — username or phone only; email is not allowed. Phone format is validated only when input looks like a phone number.</summary>
    public static IRuleBuilderOptions<T, string> MustBeAdminLoginIdentifier<T>(
        this IRuleBuilder<T, string> ruleBuilder,
        int minLength = 3)
    {
        return ruleBuilder
            .NotEmpty().WithMessage("validation.login_required")
            .MinimumLength(minLength).WithMessage($"validation.login_min_length|{minLength}")
            .Must(login => !IdentifierValidator.IsValidEmail(login.Trim()))
            .WithMessage("validation.admin_login_email_not_allowed")
            .Must(login =>
            {
                var trimmed = login.Trim();
                return !LooksLikePhone(trimmed) || IdentifierValidator.IsValidPhone(trimmed);
            })
            .WithMessage("validation.phone_invalid");
    }

    private static bool LooksLikePhone(string value)
    {
        var clean = value.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "").Replace("+", "");
        return clean.Length >= 8 && clean.All(char.IsDigit);
    }
}
