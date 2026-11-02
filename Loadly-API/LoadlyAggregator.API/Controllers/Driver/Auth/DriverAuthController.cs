using LoadlyAggregator.Shared.Enums;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Driver.Auth;
using LoadlyAggregator.API.Resources.Driver.Auth;
using LoadlyAggregator.Domain.Common.Actions.DeviceToken;
using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.DTOs.DeviceToken;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Driver.Actions.Auth;
using LoadlyAggregator.Domain.Driver.DTOs.Auth;
using LoadlyAggregator.Shared.DTOs.Auth;
using LoadlyAggregator.Infrastructure.Helpers;
using LoadlyAggregator.Shared.DTOs.Common;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.API.Controllers.Driver.Auth;





[ApiController]
[Route("api/driver/auth")]
public class DriverAuthController : ControllerBase
{
    private readonly DriverLoginAction _loginAction;
    private readonly DriverLoginWithOtpAction _loginWithOtpAction;
    private readonly DriverSendLoginOtpAction _sendLoginOtpAction;
    private readonly DriverForgetPasswordAction _forgetPasswordAction;
    private readonly DriverResetPasswordAction _resetPasswordAction;
    private readonly DriverResetPasswordWithCodeAction _resetPasswordWithCodeAction;
    private readonly DriverChangePasswordAction _changePasswordAction;
    private readonly DriverEnableMfaAction _enableMfaAction;
    private readonly DriverDisableMfaAction _disableMfaAction;
    private readonly DriverSendMfaCodeAction _sendMfaCodeAction;
    private readonly DriverVerifyMfaAction _verifyMfaAction;
    private readonly DriverRefreshTokenAction _refreshTokenAction;
    private readonly DriverGetProfileAction _getProfileAction;
    private readonly DriverUpdateProfileAction _updateProfileAction;
    private readonly DriverResendVerificationAction _resendVerificationAction;
    private readonly DriverVerifyEmailAction _verifyEmailAction;
    private readonly DriverVerifyPhoneAction _verifyPhoneAction;
    private readonly DriverListSessionsAction _listSessionsAction;
    private readonly DriverRevokeSessionAction _revokeSessionAction;
    private readonly DeactivateDeviceTokensAction _deactivateDeviceTokensAction;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ITranslationService? _translationService;
    private readonly DriverValidatePasswordAction _validatePasswordAction;
    public DriverAuthController(
        DriverLoginAction loginAction,
        DriverLoginWithOtpAction loginWithOtpAction,
        DriverSendLoginOtpAction sendLoginOtpAction,
        DriverForgetPasswordAction forgetPasswordAction,
        DriverResetPasswordAction resetPasswordAction,
        DriverResetPasswordWithCodeAction resetPasswordWithCodeAction,
        DriverChangePasswordAction changePasswordAction,
        DriverEnableMfaAction enableMfaAction,
        DriverDisableMfaAction disableMfaAction,
        DriverSendMfaCodeAction sendMfaCodeAction,
        DriverVerifyMfaAction verifyMfaAction,
        DriverRefreshTokenAction refreshTokenAction,
        DriverGetProfileAction getProfileAction,
        DriverUpdateProfileAction updateProfileAction,
        DriverResendVerificationAction resendVerificationAction,
        DriverVerifyEmailAction verifyEmailAction,
        DriverVerifyPhoneAction verifyPhoneAction,
        DriverListSessionsAction listSessionsAction,
        DriverRevokeSessionAction revokeSessionAction,
        DeactivateDeviceTokensAction deactivateDeviceTokensAction,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService,
        DriverValidatePasswordAction validatePasswordAction,
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
    public async Task<ActionResult<DriverLoginResource>> Login(
        [FromBody] DriverLoginRequest request)
    {
        var deviceInfo = DeviceInfoHelper.GetCompleteDeviceInfo(HttpContext);
        var resource = DriverLoginResource.FromResponse(
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

        // When there's no user/token but there's a message and redirect,
        // return ApiResponse directly to avoid message duplication
        // (filter will skip ApiResponse and won't extract message again)
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
    public async Task<IActionResult> SendLoginOtp([FromBody] DriverSendLoginOtpRequest request)
    {
        var result = await _sendLoginOtpAction.ExecuteAsync(request.FromRequest());
        return Ok(result);
    }

    [HttpPost("login-with-otp")]
    public async Task<ActionResult<DriverLoginResource>> LoginWithOtp(
        [FromBody] DriverLoginWithOtpRequest request)
    {
        var deviceInfo = DeviceInfoHelper.GetCompleteDeviceInfo(HttpContext);
        return Ok(DriverLoginResource.FromResponse(
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
            UserType = "driver",
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
            _refreshTokenService.RevokeModelTokensAsync(ClaimsHelper.GetUserId(User), "Driver"),
            _jwtTokenService.RevokeAllUserTokensAsync(ClaimsHelper.GetUserId(User), "driver", expirationMinutes: 120));

    private async Task RevokeTokenFamilyOrAllAsync() =>
        await (string.IsNullOrWhiteSpace(User.FindFirst("tokenFamilyId")?.Value)
            ? Task.WhenAll(
                _refreshTokenService.RevokeModelTokensAsync(ClaimsHelper.GetUserId(User), "Driver"),
                _jwtTokenService.RevokeAllUserTokensAsync(ClaimsHelper.GetUserId(User), "driver", expirationMinutes: 120))
            : _refreshTokenService.RevokeTokenFamilyAsync(User.FindFirst("tokenFamilyId")!.Value));





    [HttpPost("refresh-token")]
    public async Task<ActionResult<DriverLoginResource>> RefreshToken(
        [FromBody] DriverRefreshTokenRequest request) =>
        Ok(DriverLoginResource.FromResponse(
            await _refreshTokenAction.ExecuteAsync(request.FromRequest())));

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] DriverForgetPasswordRequest request)
    {
        await _forgetPasswordAction.ExecuteAsync(request.FromRequest());
        var message = request.Channel == Shared.Enums.CommunicationChannel.Sms
            ? "auth.forget_password_sms_sent"
            : "auth.forget_password_sent";
        return Ok(new { message });
    }






    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] DriverResetPasswordRequest request)
    {
        await _resetPasswordAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "auth.reset_password_success" });
    }







    [HttpPost("reset-password-with-code")]
    public async Task<IActionResult> ResetPasswordWithCode([FromBody] DriverResetPasswordWithCodeRequest request)
    {
        await _resetPasswordWithCodeAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "auth.reset_password_success" });
    }

    [HttpPost("enable-mfa")]
    [Authorize]
    public async Task<IActionResult> EnableMfa([FromBody] DriverEnableMfaRequest request)
    {
        await _enableMfaAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.mfa_enabled" });
    }

    [HttpPost("disable-mfa")]
    [Authorize]
    public async Task<IActionResult> DisableMfa([FromBody] DriverDisableMfaRequest request)
    {
        await _disableMfaAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.mfa_disabled" });
    }

    [HttpPost("send-mfa-code")]
    [Authorize(Policy = "MfaPending")]
    public async Task<IActionResult> SendMfaCode([FromBody] DriverSendMfaCodeRequest? request = null)
    {
        await _sendMfaCodeAction.ExecuteAsync(
            (request ?? new DriverSendMfaCodeRequest { Channel = (Shared.Enums.CommunicationChannel)Shared.Enums.CommunicationChannel.Email })
                .FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.mfa_code_sent" });
    }

    [HttpPost("verify-mfa")]
    public async Task<ActionResult<DriverLoginResource>> VerifyMfa([FromBody] DriverVerifyMfaRequest request)
    {
        var deviceInfo = DeviceInfoHelper.GetCompleteDeviceInfo(HttpContext);
        return Ok(DriverLoginResource.FromResponse(
            await _verifyMfaAction.ExecuteAsync(
                request.FromRequest(
                    ClaimsHelper.GetUserId(User),
                    User.FindFirst("userType")?.Value ?? "driver",
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
    public async Task<ActionResult<object>> UpdateProfile([FromForm] DriverUpdateProfileRequest request)
    {
        request.UserId = ClaimsHelper.GetUserId(User);
        var profile = await _updateProfileAction.ExecuteAsync(request.FromRequest());
        return Ok(profile);
    }





















    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] DriverChangePasswordRequest request)
    {
        await _changePasswordAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.change_password_success" });
    }

    [HttpPost("validate-password")]
    [Authorize]
    public async Task<ActionResult> ValidatePassword([FromBody] DriverValidatePasswordRequest request)
    {
        await _validatePasswordAction.ExecuteAsync(
            request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.password_validated" });
    }





    [HttpPost("resend-verification")]
    [Authorize]
    public async Task<IActionResult> ResendVerification([FromBody] DriverResendVerificationRequest? request = null)
    {
        await _resendVerificationAction.ExecuteAsync(
            (request ?? new DriverResendVerificationRequest { Channel = (Shared.Enums.CommunicationChannel)Shared.Enums.CommunicationChannel.Email })
                .FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.verification_code_sent" });
    }





    [HttpPost("verify-email")]
    [Authorize]
    public async Task<IActionResult> VerifyEmail([FromBody] DriverVerifyEmailRequest request)
    {
        await _verifyEmailAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.email_verified" });
    }





    [HttpPost("verify-phone")]
    [Authorize]
    public async Task<IActionResult> VerifyPhone([FromBody] DriverVerifyPhoneRequest request)
    {
        await _verifyPhoneAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.phone_verified" });
    }




    [HttpGet("sessions")]
    [Authorize]
    public async Task<ActionResult<object>> GetSessions() =>
        Ok(await _listSessionsAction.ExecuteAsync(
            new DriverListSessionsRequest
            {
                UserId = ClaimsHelper.GetUserId(User),
                CurrentTokenFamilyId = User.FindFirst("tokenFamilyId")?.Value
            }.FromRequest()));




    [HttpDelete("sessions/{sessionId}")]
    [Authorize]
    public async Task<ActionResult> RevokeSession(long sessionId)
    {
        await _revokeSessionAction.ExecuteAsync(
            new DriverRevokeSessionRequest { UserId = ClaimsHelper.GetUserId(User), SessionId = sessionId }.FromRequest());
        return Ok(new { message = "sessions.revoked_successfully" });
    }
}
