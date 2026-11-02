using LoadlyAggregator.Shared.Enums;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Company.Auth;
using LoadlyAggregator.API.Resources.Company.Auth;
using LoadlyAggregator.Domain.Common.Actions.DeviceToken;
using LoadlyAggregator.Domain.Common.DTOs.DeviceToken;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.Actions.Auth;
using LoadlyAggregator.Infrastructure.Helpers;
using LoadlyAggregator.Shared.DTOs.Auth;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.API.Controllers.Company.Auth;

[ApiController]
[Route("api/company/auth")]
public class CompanyAuthController : ControllerBase
{
    private readonly CompanyLoginAction _loginAction;
    private readonly CompanyLoginWithOtpAction _loginWithOtpAction;
    private readonly CompanySendLoginOtpAction _sendLoginOtpAction;
    private readonly CompanyForgetPasswordAction _forgetPasswordAction;
    private readonly CompanyResetPasswordAction _resetPasswordAction;
    private readonly CompanyResetPasswordWithCodeAction _resetPasswordWithCodeAction;
    private readonly CompanyChangePasswordAction _changePasswordAction;
    private readonly CompanyEnableMfaAction _enableMfaAction;
    private readonly CompanyDisableMfaAction _disableMfaAction;
    private readonly CompanySendMfaCodeAction _sendMfaCodeAction;
    private readonly CompanyVerifyMfaAction _verifyMfaAction;
    private readonly CompanyRefreshTokenAction _refreshTokenAction;
    private readonly CompanyGetProfileAction _getProfileAction;
    private readonly CompanyUpdateProfileAction _updateProfileAction;
    private readonly CompanyResendVerificationAction _resendVerificationAction;
    private readonly CompanyVerifyEmailAction _verifyEmailAction;
    private readonly CompanyVerifyPhoneAction _verifyPhoneAction;
    private readonly CompanyListSessionsAction _listSessionsAction;
    private readonly CompanyRevokeSessionAction _revokeSessionAction;
    private readonly DeactivateDeviceTokensAction _deactivateDeviceTokensAction;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITranslationService? _translationService;
    private readonly CompanyValidatePasswordAction _validatePasswordAction;

    public CompanyAuthController(
        CompanyLoginAction loginAction,
        CompanyLoginWithOtpAction loginWithOtpAction,
        CompanySendLoginOtpAction sendLoginOtpAction,
        CompanyForgetPasswordAction forgetPasswordAction,
        CompanyResetPasswordAction resetPasswordAction,
        CompanyResetPasswordWithCodeAction resetPasswordWithCodeAction,
        CompanyChangePasswordAction changePasswordAction,
        CompanyEnableMfaAction enableMfaAction,
        CompanyDisableMfaAction disableMfaAction,
        CompanySendMfaCodeAction sendMfaCodeAction,
        CompanyVerifyMfaAction verifyMfaAction,
        CompanyRefreshTokenAction refreshTokenAction,
        CompanyGetProfileAction getProfileAction,
        CompanyUpdateProfileAction updateProfileAction,
        CompanyResendVerificationAction resendVerificationAction,
        CompanyVerifyEmailAction verifyEmailAction,
        CompanyVerifyPhoneAction verifyPhoneAction,
        CompanyListSessionsAction listSessionsAction,
        CompanyRevokeSessionAction revokeSessionAction,
        DeactivateDeviceTokensAction deactivateDeviceTokensAction,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        CompanyValidatePasswordAction validatePasswordAction,
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
        _updateProfileAction = updateProfileAction;
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
    }

    [HttpPost("login")]
    public async Task<ActionResult<CompanyLoginResource>> Login(
        [FromBody] CompanyLoginRequest request)
    {
        var deviceInfo = DeviceInfoHelper.GetCompleteDeviceInfo(HttpContext);
        var resource = CompanyLoginResource.FromResponse(
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
    public async Task<IActionResult> SendLoginOtp([FromBody] CompanySendLoginOtpRequest request)
    {
        var result = await _sendLoginOtpAction.ExecuteAsync(request.FromRequest());
        return Ok(result);
    }

    [HttpPost("login-with-otp")]
    public async Task<ActionResult<CompanyLoginResource>> LoginWithOtp(
        [FromBody] CompanyLoginWithOtpRequest request)
    {
        var deviceInfo = DeviceInfoHelper.GetCompleteDeviceInfo(HttpContext);
        return Ok(CompanyLoginResource.FromResponse(
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
            UserType = "company",
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
            _refreshTokenService.RevokeModelTokensAsync(ClaimsHelper.GetUserId(User), "Company"),
            _jwtTokenService.RevokeAllUserTokensAsync(ClaimsHelper.GetUserId(User), "company", expirationMinutes: 120));

    private async Task RevokeTokenFamilyOrAllAsync() =>
        await (string.IsNullOrWhiteSpace(User.FindFirst("tokenFamilyId")?.Value)
            ? Task.WhenAll(
                _refreshTokenService.RevokeModelTokensAsync(ClaimsHelper.GetUserId(User), "Company"),
                _jwtTokenService.RevokeAllUserTokensAsync(ClaimsHelper.GetUserId(User), "company", expirationMinutes: 120))
            : _refreshTokenService.RevokeTokenFamilyAsync(User.FindFirst("tokenFamilyId")!.Value));

    [HttpPost("refresh-token")]
    public async Task<ActionResult<CompanyLoginResource>> RefreshToken(
        [FromBody] CompanyRefreshTokenRequest request) =>
        Ok(CompanyLoginResource.FromResponse(
            await _refreshTokenAction.ExecuteAsync(
                new LoadlyAggregator.Domain.Common.DTOs.Auth.RefreshTokenRequestDto
                {
                    RefreshToken = request.RefreshToken
                })));

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] CompanyForgetPasswordRequest request)
    {
        await _forgetPasswordAction.ExecuteAsync(request.FromRequest());
        var message = request.Channel == Shared.Enums.CommunicationChannel.Sms
                    ? "auth.forget_password_sms_sent"
                    : "auth.forget_password_sent";
        return Ok(new { message });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] CompanyResetPasswordRequest request)
    {
        await _resetPasswordAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "auth.reset_password_success" });
    }

    [HttpPost("reset-password-with-code")]
    public async Task<IActionResult> ResetPasswordWithCode([FromBody] CompanyResetPasswordWithCodeRequest request)
    {
        await _resetPasswordWithCodeAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "auth.reset_password_success" });
    }

    [HttpPost("enable-mfa")]
    [Authorize]
    public async Task<IActionResult> EnableMfa([FromBody] CompanyEnableMfaRequest request)
    {
        await _enableMfaAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.mfa_enabled" });
    }

    [HttpPost("disable-mfa")]
    [Authorize]
    public async Task<IActionResult> DisableMfa([FromBody] CompanyDisableMfaRequest request)
    {
        await _disableMfaAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.mfa_disabled" });
    }

    [HttpPost("send-mfa-code")]
    [Authorize(Policy = "MfaPending")]
    public async Task<IActionResult> SendMfaCode([FromBody] CompanySendMfaCodeRequest? request = null)
    {
        await _sendMfaCodeAction.ExecuteAsync(
            (request ?? new CompanySendMfaCodeRequest { Channel = (Shared.Enums.CommunicationChannel)Shared.Enums.CommunicationChannel.Email })
                .FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.mfa_code_sent" });
    }

    [HttpPost("verify-mfa")]
    public async Task<ActionResult<CompanyLoginResource>> VerifyMfa([FromBody] CompanyVerifyMfaRequest request)
    {
        var deviceInfo = DeviceInfoHelper.GetCompleteDeviceInfo(HttpContext);
        return Ok(CompanyLoginResource.FromResponse(
            await _verifyMfaAction.ExecuteAsync(
                request.FromRequest(
                    ClaimsHelper.GetUserId(User),
                    User.FindFirst("userType")?.Value ?? "company",
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

    [HttpPut("profile")]
    [Authorize]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<object>> UpdateProfile([FromForm] CompanyUpdateProfileRequest request)
    {
        request.UserId = ClaimsHelper.GetUserId(User);
        var profile = await _updateProfileAction.ExecuteAsync(request.FromRequest());
        return Ok(profile);
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] CompanyChangePasswordRequest request)
    {
        await _changePasswordAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.change_password_success" });
    }

    [HttpPost("validate-password")]
    [Authorize]
    public async Task<ActionResult> ValidatePassword([FromBody] CompanyValidatePasswordRequest request)
    {
        await _validatePasswordAction.ExecuteAsync(
            request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.password_validated" });
    }

    [HttpPost("resend-verification")]
    [Authorize]
    public async Task<IActionResult> ResendVerification([FromBody] CompanyResendVerificationRequest? request = null)
    {
        await _resendVerificationAction.ExecuteAsync(
            (request ?? new CompanyResendVerificationRequest { Channel = (Shared.Enums.CommunicationChannel)Shared.Enums.CommunicationChannel.Email })
                .FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.verification_code_sent" });
    }

    [HttpPost("verify-email")]
    [Authorize]
    public async Task<IActionResult> VerifyEmail([FromBody] CompanyVerifyEmailRequest request)
    {
        await _verifyEmailAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.email_verified" });
    }

    [HttpPost("verify-phone")]
    [Authorize]
    public async Task<IActionResult> VerifyPhone([FromBody] CompanyVerifyPhoneRequest request)
    {
        await _verifyPhoneAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.phone_verified" });
    }

    [HttpGet("sessions")]
    [Authorize]
    public async Task<ActionResult<object>> GetSessions() =>
        Ok(await _listSessionsAction.ExecuteAsync(
            new CompanyListSessionsRequest
            {
                UserId = ClaimsHelper.GetUserId(User),
                CurrentTokenFamilyId = User.FindFirst("tokenFamilyId")?.Value
            }.FromRequest()));

    [HttpDelete("sessions/{sessionId}")]
    [Authorize]
    public async Task<ActionResult> RevokeSession(long sessionId)
    {
        await _revokeSessionAction.ExecuteAsync(
            new CompanyRevokeSessionRequest { UserId = ClaimsHelper.GetUserId(User), SessionId = sessionId }.FromRequest());
        return Ok(new { message = "sessions.revoked_successfully" });
    }
}
