using LoadlyAggregator.Shared.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Collector.Auth;
using LoadlyAggregator.API.Resources.Collector.Auth;
using LoadlyAggregator.Domain.Collector.Actions.Auth;
using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Actions.DeviceToken;
using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.DTOs.DeviceToken;
using LoadlyAggregator.Infrastructure.Helpers;
using LoadlyAggregator.Shared.DTOs.Auth;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.API.Controllers.Collector.Auth;





[ApiController]
[Route("api/collector/auth")]
public class AdminAuthController : ControllerBase
{
    private readonly CollectorLoginAction _loginAction;
    private readonly CollectorLoginWithOtpAction _loginWithOtpAction;
    private readonly CollectorSendLoginOtpAction _sendLoginOtpAction;
    private readonly CollectorForgetPasswordAction _forgetPasswordAction;
    private readonly CollectorResetPasswordAction _resetPasswordAction;
    private readonly CollectorResetPasswordWithCodeAction _resetPasswordWithCodeAction;
    private readonly CollectorChangePasswordAction _changePasswordAction;
    private readonly CollectorEnableMfaAction _enableMfaAction;
    private readonly CollectorDisableMfaAction _disableMfaAction;
    private readonly CollectorSendMfaCodeAction _sendMfaCodeAction;
    private readonly CollectorVerifyMfaAction _verifyMfaAction;
    private readonly CollectorRefreshTokenAction _refreshTokenAction;
    private readonly CollectorGetProfileAction _getProfileAction;
    private readonly CollectorUpdateProfileAction _updateProfileAction;
    private readonly CollectorResendVerificationAction _resendVerificationAction;
    private readonly CollectorVerifyEmailAction _verifyEmailAction;
    private readonly CollectorVerifyPhoneAction _verifyPhoneAction;
    private readonly CollectorListSessionsAction _listSessionsAction;
    private readonly CollectorRevokeSessionAction _revokeSessionAction;
    private readonly DeactivateDeviceTokensAction _deactivateDeviceTokensAction;
    private readonly Domain.Common.Interfaces.IRefreshTokenService _refreshTokenService;
    private readonly Domain.Common.Interfaces.IJwtTokenService _jwtTokenService;
    private readonly CollectorValidatePasswordAction _validatePasswordAction;
    public AdminAuthController(
        CollectorLoginAction loginAction,
        CollectorLoginWithOtpAction loginWithOtpAction,
        CollectorSendLoginOtpAction sendLoginOtpAction,
        CollectorForgetPasswordAction forgetPasswordAction,
        CollectorResetPasswordAction resetPasswordAction,
        CollectorResetPasswordWithCodeAction resetPasswordWithCodeAction,
        CollectorChangePasswordAction changePasswordAction,
        CollectorEnableMfaAction enableMfaAction,
        CollectorDisableMfaAction disableMfaAction,
        CollectorSendMfaCodeAction sendMfaCodeAction,
        CollectorVerifyMfaAction verifyMfaAction,
        CollectorRefreshTokenAction refreshTokenAction,
        CollectorGetProfileAction getProfileAction,
        CollectorUpdateProfileAction updateProfileAction,
        CollectorResendVerificationAction resendVerificationAction,
        CollectorVerifyEmailAction verifyEmailAction,
        CollectorVerifyPhoneAction verifyPhoneAction,
        CollectorListSessionsAction listSessionsAction,
        CollectorRevokeSessionAction revokeSessionAction,
        DeactivateDeviceTokensAction deactivateDeviceTokensAction,
        Domain.Common.Interfaces.IRefreshTokenService refreshTokenService,
        Domain.Common.Interfaces.IJwtTokenService jwtTokenService,
        CollectorValidatePasswordAction validatePasswordAction)
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
        _validatePasswordAction = validatePasswordAction;
    }




    [HttpPost("login")]
    public async Task<ActionResult<CollectorLoginResource>> Login(
        [FromBody] CollectorLoginRequest request)
    {
        var deviceInfo = DeviceInfoHelper.GetCompleteDeviceInfo(HttpContext);
        return Ok(CollectorLoginResource.FromResponse(
            await _loginAction.ExecuteAsync(
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

    [HttpPost("send-login-otp")]
    public async Task<IActionResult> SendLoginOtp([FromBody] CollectorSendLoginOtpRequest request)
    {
        var result = await _sendLoginOtpAction.ExecuteAsync(request.FromRequest());
        return Ok(result);
    }

    [HttpPost("login-with-otp")]
    public async Task<ActionResult<CollectorLoginResource>> LoginWithOtp(
        [FromBody] CollectorLoginWithOtpRequest request)
    {
        var deviceInfo = DeviceInfoHelper.GetCompleteDeviceInfo(HttpContext);
        return Ok(CollectorLoginResource.FromResponse(
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
            UserType = "collector",
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
            _refreshTokenService.RevokeModelTokensAsync(ClaimsHelper.GetUserId(User), "Collector"),
            _jwtTokenService.RevokeAllUserTokensAsync(ClaimsHelper.GetUserId(User), "collector", expirationMinutes: 120));

    private async Task RevokeTokenFamilyOrAllAsync() =>
        await (string.IsNullOrWhiteSpace(User.FindFirst("tokenFamilyId")?.Value)
            ? Task.WhenAll(
                _refreshTokenService.RevokeModelTokensAsync(ClaimsHelper.GetUserId(User), "Collector"),
                _jwtTokenService.RevokeAllUserTokensAsync(ClaimsHelper.GetUserId(User), "collector", expirationMinutes: 120))
            : _refreshTokenService.RevokeTokenFamilyAsync(User.FindFirst("tokenFamilyId")!.Value));





    [HttpPost("refresh-token")]
    public async Task<ActionResult<CollectorLoginResource>> RefreshToken(
        [FromBody] CollectorRefreshTokenRequest request) =>
        Ok(CollectorLoginResource.FromResponse(
            await _refreshTokenAction.ExecuteAsync(request.FromRequest())));

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] CollectorForgetPasswordRequest request)
    {
        await _forgetPasswordAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "auth.forget_password_sms_sent" });
    }






    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] CollectorResetPasswordRequest request)
    {
        await _resetPasswordAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "auth.reset_password_success" });
    }







    [HttpPost("reset-password-with-code")]
    public async Task<IActionResult> ResetPasswordWithCode([FromBody] CollectorResetPasswordWithCodeRequest request)
    {
        await _resetPasswordWithCodeAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "auth.reset_password_success" });
    }

    [HttpPost("enable-mfa")]
    [Authorize]
    public async Task<IActionResult> EnableMfa([FromBody] CollectorEnableMfaRequest request)
    {
        await _enableMfaAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.mfa_enabled" });
    }

    [HttpPost("disable-mfa")]
    [Authorize]
    public async Task<IActionResult> DisableMfa([FromBody] CollectorDisableMfaRequest request)
    {
        await _disableMfaAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.mfa_disabled" });
    }

    [HttpPost("send-mfa-code")]
    [Authorize(Policy = "MfaPending")]
    public async Task<IActionResult> SendMfaCode([FromBody] CollectorSendMfaCodeRequest? request = null)
    {
        await _sendMfaCodeAction.ExecuteAsync(
            (request ?? new CollectorSendMfaCodeRequest { Channel = Shared.Enums.CommunicationChannel.Sms })
                .FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.mfa_code_sent" });
    }

    [HttpPost("verify-mfa")]
    public async Task<ActionResult<CollectorLoginResource>> VerifyMfa([FromBody] CollectorVerifyMfaRequest request)
    {
        var deviceInfo = DeviceInfoHelper.GetCompleteDeviceInfo(HttpContext);
        return Ok(CollectorLoginResource.FromResponse(
            await _verifyMfaAction.ExecuteAsync(
                request.FromRequest(
                    ClaimsHelper.GetUserId(User),
                    User.FindFirst("userType")?.Value ?? "collector",
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
    public async Task<IActionResult> ChangePassword([FromBody] CollectorChangePasswordRequest request)
    {
        await _changePasswordAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.change_password_success" });
    }

    [HttpPost("validate-password")]
    [Authorize]
    public async Task<ActionResult> ValidatePassword([FromBody] CollectorValidatePasswordRequest request)
    {
        await _validatePasswordAction.ExecuteAsync(
            request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.password_validated" });
    }





    [HttpPost("resend-verification")]
    [Authorize]
    public async Task<IActionResult> ResendVerification([FromBody] CollectorResendVerificationRequest? request = null)
    {
        await _resendVerificationAction.ExecuteAsync(
            (request ?? new CollectorResendVerificationRequest { Channel = Shared.Enums.CommunicationChannel.Email })
                .FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.verification_code_sent" });
    }





    [HttpPost("verify-email")]
    [Authorize]
    public async Task<IActionResult> VerifyEmail([FromBody] CollectorVerifyEmailRequest request)
    {
        await _verifyEmailAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.email_verified" });
    }





    [HttpPost("verify-phone")]
    [Authorize]
    public async Task<IActionResult> VerifyPhone([FromBody] CollectorVerifyPhoneRequest request)
    {
        await _verifyPhoneAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.phone_verified" });
    }




    [HttpGet("sessions")]
    [Authorize]
    public async Task<ActionResult<object>> GetSessions() =>
        Ok(await _listSessionsAction.ExecuteAsync(
            new CollectorListSessionsRequest
            {
                UserId = ClaimsHelper.GetUserId(User),
                CurrentTokenFamilyId = User.FindFirst("tokenFamilyId")?.Value
            }.FromRequest()));




    [HttpDelete("sessions/{sessionId}")]
    [Authorize]
    public async Task<ActionResult> RevokeSession(long sessionId)
    {
        await _revokeSessionAction.ExecuteAsync(
            new CollectorRevokeSessionRequest { UserId = ClaimsHelper.GetUserId(User), SessionId = sessionId }.FromRequest());
        return Ok(new { message = "sessions.revoked_successfully" });
    }
}
