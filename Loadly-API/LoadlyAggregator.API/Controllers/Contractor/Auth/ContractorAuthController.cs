using LoadlyAggregator.Shared.Enums;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Contractor.Auth;
using LoadlyAggregator.API.Resources.Contractor.Auth;
using LoadlyAggregator.Domain.Common.Actions.DeviceToken;
using LoadlyAggregator.Domain.Common.DTOs.DeviceToken;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.Actions.Auth;
using LoadlyAggregator.Infrastructure.Helpers;
using LoadlyAggregator.Shared.DTOs.Auth;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.API.Controllers.Contractor.Auth;

[ApiController]
[Route("api/contractor/auth")]
public class ContractorAuthController : ControllerBase
{
    private readonly ContractorLoginAction _loginAction;
    private readonly ContractorLoginWithOtpAction _loginWithOtpAction;
    private readonly ContractorSendLoginOtpAction _sendLoginOtpAction;
    private readonly ContractorForgetPasswordAction _forgetPasswordAction;
    private readonly ContractorResetPasswordAction _resetPasswordAction;
    private readonly ContractorResetPasswordWithCodeAction _resetPasswordWithCodeAction;
    private readonly ContractorChangePasswordAction _changePasswordAction;
    private readonly ContractorEnableMfaAction _enableMfaAction;
    private readonly ContractorDisableMfaAction _disableMfaAction;
    private readonly ContractorSendMfaCodeAction _sendMfaCodeAction;
    private readonly ContractorVerifyMfaAction _verifyMfaAction;
    private readonly ContractorRefreshTokenAction _refreshTokenAction;
    private readonly ContractorGetProfileAction _getProfileAction;
    private readonly ContractorResendVerificationAction _resendVerificationAction;
    private readonly ContractorVerifyEmailAction _verifyEmailAction;
    private readonly ContractorVerifyPhoneAction _verifyPhoneAction;
    private readonly ContractorListSessionsAction _listSessionsAction;
    private readonly ContractorRevokeSessionAction _revokeSessionAction;
    private readonly DeactivateDeviceTokensAction _deactivateDeviceTokensAction;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITranslationService? _translationService;
    private readonly ContractorValidatePasswordAction _validatePasswordAction;
    private readonly RegisterContractorAction _registerContractorAction;

    public ContractorAuthController(
        ContractorLoginAction loginAction,
        ContractorLoginWithOtpAction loginWithOtpAction,
        ContractorSendLoginOtpAction sendLoginOtpAction,
        ContractorForgetPasswordAction forgetPasswordAction,
        ContractorResetPasswordAction resetPasswordAction,
        ContractorResetPasswordWithCodeAction resetPasswordWithCodeAction,
        ContractorChangePasswordAction changePasswordAction,
        ContractorEnableMfaAction enableMfaAction,
        ContractorDisableMfaAction disableMfaAction,
        ContractorSendMfaCodeAction sendMfaCodeAction,
        ContractorVerifyMfaAction verifyMfaAction,
        ContractorRefreshTokenAction refreshTokenAction,
        ContractorGetProfileAction getProfileAction,
        ContractorResendVerificationAction resendVerificationAction,
        ContractorVerifyEmailAction verifyEmailAction,
        ContractorVerifyPhoneAction verifyPhoneAction,
        ContractorListSessionsAction listSessionsAction,
        ContractorRevokeSessionAction revokeSessionAction,
        DeactivateDeviceTokensAction deactivateDeviceTokensAction,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        ContractorValidatePasswordAction validatePasswordAction,
        RegisterContractorAction registerContractorAction,
        ITranslationService? translationService = null)
    {
        _loginAction = loginAction;
        _loginWithOtpAction = loginWithOtpAction;
        _sendLoginOtpAction = sendLoginOtpAction;
        _forgetPasswordAction = forgetPasswordAction;
        _resetPasswordAction = resetPasswordAction;
        _resetPasswordWithCodeAction = resetPasswordWithCodeAction;
        _changePasswordAction = changePasswordAction;
        _enableMfaAction = enableMfaAction;
        _disableMfaAction = disableMfaAction;
        _sendMfaCodeAction = sendMfaCodeAction;
        _verifyMfaAction = verifyMfaAction;
        _refreshTokenAction = refreshTokenAction;
        _getProfileAction = getProfileAction;
        _resendVerificationAction = resendVerificationAction;
        _verifyEmailAction = verifyEmailAction;
        _verifyPhoneAction = verifyPhoneAction;
        _listSessionsAction = listSessionsAction;
        _revokeSessionAction = revokeSessionAction;
        _deactivateDeviceTokensAction = deactivateDeviceTokensAction;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
        _translationService = translationService;
        _validatePasswordAction = validatePasswordAction;
        _registerContractorAction = registerContractorAction;
    }

    [HttpPost("register")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Register([FromForm] RegisterContractorRequest request)
    {
        await _registerContractorAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "auth.registration_pending_approval" });
    }

    [HttpPost("login")]
    public async Task<ActionResult<ContractorLoginResource>> Login(
        [FromBody] ContractorLoginRequest request)
    {
        var deviceInfo = DeviceInfoHelper.GetCompleteDeviceInfo(HttpContext);
        var resource = ContractorLoginResource.FromResponse(
            await _loginAction.ExecuteAsync(
                request.FromRequest(
                    DeviceInfoHelper.GetDeviceInfo(HttpContext).IpAddress,
                    DeviceInfoHelper.GetDeviceInfo(HttpContext).UserAgent,
                    DeviceInfoHelper.GetDeviceInfoJson(HttpContext),
                    DeviceInfoHelper.GetFcmToken(HttpContext),
                    deviceInfo.DeviceId,
                    deviceInfo.DeviceModel,
                    deviceInfo.DeviceOs,
                    deviceInfo.AppVersion)));

        if (resource.User == null && resource.AccessToken == null && !string.IsNullOrWhiteSpace(resource.Message))
        {
            var translatedMessage = TranslationHelper.TranslateMessage(resource.Message, _translationService);
            return Ok(ApiResponse<object>.Create(
                new { Redirect = resource.Redirect },
                translatedMessage));
        }

        return Ok(resource);
    }

    [HttpPost("send-login-otp")]
    public async Task<IActionResult> SendLoginOtp([FromBody] ContractorSendLoginOtpRequest request)
    {
        var result = await _sendLoginOtpAction.ExecuteAsync(request.FromRequest());
        return Ok(result);
    }

    [HttpPost("login-with-otp")]
    public async Task<ActionResult<ContractorLoginResource>> LoginWithOtp(
        [FromBody] ContractorLoginWithOtpRequest request)
    {
        var deviceInfo = DeviceInfoHelper.GetCompleteDeviceInfo(HttpContext);
        return Ok(ContractorLoginResource.FromResponse(
            await _loginWithOtpAction.ExecuteAsync(
                request.FromRequest(
                    DeviceInfoHelper.GetDeviceInfo(HttpContext).IpAddress,
                    DeviceInfoHelper.GetDeviceInfo(HttpContext).UserAgent,
                    DeviceInfoHelper.GetDeviceInfoJson(HttpContext),
                    DeviceInfoHelper.GetFcmToken(HttpContext),
                    deviceInfo.DeviceId,
                    deviceInfo.DeviceModel,
                    deviceInfo.DeviceOs,
                    deviceInfo.AppVersion))));
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] LogoutRequestDto? request = null)
    {
        return !ModelState.IsValid
            ? BadRequest(new
            {
                message = "validation.error",
                errors = ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray())
            })
            : await ExecuteLogoutAsync(request);
    }

    private async Task<IActionResult> ExecuteLogoutAsync(LogoutRequestDto? request)
    {
        var userId = ClaimsHelper.GetUserId(User);
        await RevokeCurrentTokenAsync();
        await (request?.RevokeAll == true
            ? RevokeAllTokensAsync()
            : RevokeTokenFamilyOrAllAsync());
        await _deactivateDeviceTokensAction.ExecuteAsync(new DeactivateDeviceTokensDto
        {
            UserId = userId,
            UserType = "contractor",
            FcmToken = DeviceInfoHelper.GetFcmToken(HttpContext),
            DeviceId = DeviceInfoHelper.GetCompleteDeviceInfo(HttpContext).DeviceId,
            RevokeAll = request?.RevokeAll == true
        });
        return Ok(new { message = "auth.logout_success" });
    }

    private async Task RevokeCurrentTokenAsync() =>
        await Task.WhenAll(
            new[] { User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value }
                .Where(jti => !string.IsNullOrWhiteSpace(jti))
                .Select(jti => _jwtTokenService.RevokeTokenAsync(jti!, expirationMinutes: 120)));

    private async Task RevokeAllTokensAsync() =>
        await Task.WhenAll(
            _refreshTokenService.RevokeModelTokensAsync(ClaimsHelper.GetUserId(User), "Contractor"),
            _jwtTokenService.RevokeAllUserTokensAsync(ClaimsHelper.GetUserId(User), "contractor", expirationMinutes: 120));

    private async Task RevokeTokenFamilyOrAllAsync() =>
        await (string.IsNullOrWhiteSpace(User.FindFirst("tokenFamilyId")?.Value)
            ? Task.WhenAll(
                _refreshTokenService.RevokeModelTokensAsync(ClaimsHelper.GetUserId(User), "Contractor"),
                _jwtTokenService.RevokeAllUserTokensAsync(ClaimsHelper.GetUserId(User), "Contractor", expirationMinutes: 120))
            : _refreshTokenService.RevokeTokenFamilyAsync(User.FindFirst("tokenFamilyId")!.Value));

    [HttpPost("refresh-token")]
    public async Task<ActionResult<ContractorLoginResource>> RefreshToken(
        [FromBody] ContractorRefreshTokenRequest request) =>
        Ok(ContractorLoginResource.FromResponse(
            await _refreshTokenAction.ExecuteAsync(
                new LoadlyAggregator.Domain.Common.DTOs.Auth.RefreshTokenRequestDto
                {
                    RefreshToken = request.RefreshToken
                })));

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ContractorForgetPasswordRequest request)
    {
        await _forgetPasswordAction.ExecuteAsync(request.FromRequest());
        var message = request.Channel == Shared.Enums.CommunicationChannel.Sms
                    ? "auth.forget_password_sms_sent"
                    : "auth.forget_password_sent";
        return Ok(new { message });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ContractorResetPasswordRequest request)
    {
        await _resetPasswordAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "auth.reset_password_success" });
    }

    [HttpPost("reset-password-with-code")]
    public async Task<IActionResult> ResetPasswordWithCode([FromBody] ContractorResetPasswordWithCodeRequest request)
    {
        await _resetPasswordWithCodeAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "auth.reset_password_success" });
    }

    [HttpPost("enable-mfa")]
    [Authorize]
    public async Task<IActionResult> EnableMfa([FromBody] ContractorEnableMfaRequest request)
    {
        await _enableMfaAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.mfa_enabled" });
    }

    [HttpPost("disable-mfa")]
    [Authorize]
    public async Task<IActionResult> DisableMfa([FromBody] ContractorDisableMfaRequest request)
    {
        await _disableMfaAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.mfa_disabled" });
    }

    [HttpPost("send-mfa-code")]
    [Authorize(Policy = "MfaPending")]
    public async Task<IActionResult> SendMfaCode([FromBody] ContractorSendMfaCodeRequest? request = null)
    {
        await _sendMfaCodeAction.ExecuteAsync(
            (request ?? new ContractorSendMfaCodeRequest { Channel = (Shared.Enums.CommunicationChannel)Shared.Enums.CommunicationChannel.Email })
                .FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.mfa_code_sent" });
    }

    [HttpPost("verify-mfa")]
    public async Task<ActionResult<ContractorLoginResource>> VerifyMfa([FromBody] ContractorVerifyMfaRequest request)
    {
        var deviceInfo = DeviceInfoHelper.GetCompleteDeviceInfo(HttpContext);
        return Ok(ContractorLoginResource.FromResponse(
            await _verifyMfaAction.ExecuteAsync(
                request.FromRequest(
                    ClaimsHelper.GetUserId(User),
                    User.FindFirst("userType")?.Value ?? "contractor",
                    DeviceInfoHelper.GetDeviceInfo(HttpContext).IpAddress,
                    DeviceInfoHelper.GetDeviceInfo(HttpContext).UserAgent,
                    DeviceInfoHelper.GetDeviceInfoJson(HttpContext),
                    DeviceInfoHelper.GetFcmToken(HttpContext),
                    deviceInfo.DeviceId,
                    deviceInfo.DeviceModel,
                    deviceInfo.DeviceOs,
                    deviceInfo.AppVersion))));
    }

    [HttpGet("profile")]
    [Authorize]
    public async Task<ActionResult<object>> GetProfile() =>
        Ok(await _getProfileAction.ExecuteAsync(ClaimsHelper.GetUserId(User)));

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ContractorChangePasswordRequest request)
    {
        await _changePasswordAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.change_password_success" });
    }

    [HttpPost("validate-password")]
    [Authorize]
    public async Task<ActionResult> ValidatePassword([FromBody] ContractorValidatePasswordRequest request)
    {
        await _validatePasswordAction.ExecuteAsync(
            request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.password_validated" });
    }

    [HttpPost("resend-verification")]
    [Authorize]
    public async Task<IActionResult> ResendVerification([FromBody] ContractorResendVerificationRequest? request = null)
    {
        await _resendVerificationAction.ExecuteAsync(
            (request ?? new ContractorResendVerificationRequest { Channel = (Shared.Enums.CommunicationChannel)Shared.Enums.CommunicationChannel.Email })
                .FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.verification_code_sent" });
    }

    [HttpPost("verify-email")]
    [Authorize]
    public async Task<IActionResult> VerifyEmail([FromBody] ContractorVerifyEmailRequest request)
    {
        await _verifyEmailAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.email_verified" });
    }

    [HttpPost("verify-phone")]
    [Authorize]
    public async Task<IActionResult> VerifyPhone([FromBody] ContractorVerifyPhoneRequest request)
    {
        await _verifyPhoneAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.phone_verified" });
    }

    [HttpGet("sessions")]
    [Authorize]
    public async Task<ActionResult<object>> GetSessions() =>
        Ok(await _listSessionsAction.ExecuteAsync(
            new ContractorListSessionsRequest
            {
                UserId = ClaimsHelper.GetUserId(User),
                CurrentTokenFamilyId = User.FindFirst("tokenFamilyId")?.Value
            }.FromRequest()));

    [HttpDelete("sessions/{sessionId}")]
    [Authorize]
    public async Task<ActionResult> RevokeSession(long sessionId)
    {
        await _revokeSessionAction.ExecuteAsync(
            new ContractorRevokeSessionRequest { UserId = ClaimsHelper.GetUserId(User), SessionId = sessionId }.FromRequest());
        return Ok(new { message = "sessions.revoked_successfully" });
    }
}
