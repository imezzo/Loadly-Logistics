namespace LoadlyAggregator.API.Constants;

/// <summary>
/// Centralized translation keys used across the application.
/// Keys follow the pattern: {module}.{action/state}
/// </summary>
public static class TranslationKeys
{
    /// <summary>
    /// Common/shared translation keys.
    /// </summary>
    public static class Common
    {
        public const string Success = "common.success";
        public const string Error = "common.error";
        public const string BadRequest = "common.bad_request";
        public const string Forbidden = "common.forbidden";
        public const string Unauthorized = "common.unauthorized";
        public const string NotFound = "common.not_found";
        public const string ValidationFailed = "common.validation_failed";
    }

    /// <summary>
    /// Authentication-related translation keys.
    /// </summary>
    public static class Auth
    {
        public const string LoginSuccess = "auth.login_success";
        public const string LoginFailed = "auth.login_failed";
        public const string MfaVerifySuccess = "auth.mfa_verify_success";
        public const string ForgetPasswordSent = "auth.forget_password_sent";
        public const string ResetPasswordSuccess = "auth.reset_password_success";
        public const string ChangePasswordSuccess = "auth.change_password_success";
        public const string MfaEnabled = "auth.mfa_enabled";
        public const string MfaDisabled = "auth.mfa_disabled";
        public const string MfaCodeSent = "auth.mfa_code_sent";
        public const string EmailVerified = "auth.email_verified";
        public const string PhoneVerified = "auth.phone_verified";
        public const string VerificationCodeSent = "auth.verification_code_sent";
        public const string LogoutSuccess = "auth.logout_success";
    }
}
