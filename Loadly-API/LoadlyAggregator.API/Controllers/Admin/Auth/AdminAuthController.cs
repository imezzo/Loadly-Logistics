using LoadlyAggregator.Shared.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Admin.Auth;
using LoadlyAggregator.API.Resources.Admin.Auth;
using LoadlyAggregator.Domain.Admin.Actions.Auth;
using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Actions.DeviceToken;
using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.DTOs.DeviceToken;
using LoadlyAggregator.Infrastructure.Helpers;
using LoadlyAggregator.Shared.DTOs.Auth;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.API.Controllers.Admin.Auth;





[ApiController]
[Route("api/admin/auth")]
public class AdminAuthController : ControllerBase
{
    private readonly AdminLoginAction _loginAction;
    private readonly AdminForgetPasswordAction _forgetPasswordAction;
    private readonly AdminResetPasswordAction _resetPasswordAction;
    private readonly AdminResetPasswordWithCodeAction _resetPasswordWithCodeAction;
    private readonly AdminChangePasswordAction _changePasswordAction;
    private readonly AdminEnableMfaAction _enableMfaAction;
    private readonly AdminDisableMfaAction _disableMfaAction;
    private readonly AdminSendMfaCodeAction _sendMfaCodeAction;
    private readonly AdminVerifyMfaAction _verifyMfaAction;
    private readonly AdminRefreshTokenAction _refreshTokenAction;
    private readonly AdminGetProfileAction _getProfileAction;
    private readonly AdminUpdateProfileAction _updateProfileAction;
    private readonly AdminResendVerificationAction _resendVerificationAction;
    private readonly AdminVerifyEmailAction _verifyEmailAction;
    private readonly AdminVerifyPhoneAction _verifyPhoneAction;
    private readonly AdminListSessionsAction _listSessionsAction;
    private readonly AdminRevokeSessionAction _revokeSessionAction;
    private readonly DeactivateDeviceTokensAction _deactivateDeviceTokensAction;
    private readonly Domain.Common.Interfaces.IRefreshTokenService _refreshTokenService;
    private readonly Domain.Common.Interfaces.IJwtTokenService _jwtTokenService;
    private readonly AdminValidatePasswordAction _validatePasswordAction;
    public AdminAuthController(
        AdminLoginAction loginAction,
        AdminForgetPasswordAction forgetPasswordAction,
        AdminResetPasswordAction resetPasswordAction,
        AdminResetPasswordWithCodeAction resetPasswordWithCodeAction,
        AdminChangePasswordAction changePasswordAction,
        AdminEnableMfaAction enableMfaAction,
        AdminDisableMfaAction disableMfaAction,
        AdminSendMfaCodeAction sendMfaCodeAction,
        AdminVerifyMfaAction verifyMfaAction,
        AdminRefreshTokenAction refreshTokenAction,
        AdminGetProfileAction getProfileAction,
        AdminUpdateProfileAction updateProfileAction,
        AdminResendVerificationAction resendVerificationAction,
        AdminVerifyEmailAction verifyEmailAction,
        AdminVerifyPhoneAction verifyPhoneAction,
        AdminListSessionsAction listSessionsAction,
        AdminRevokeSessionAction revokeSessionAction,
        DeactivateDeviceTokensAction deactivateDeviceTokensAction,
        Domain.Common.Interfaces.IRefreshTokenService refreshTokenService,
        Domain.Common.Interfaces.IJwtTokenService jwtTokenService,
        AdminValidatePasswordAction validatePasswordAction)
    {
        _loginAction = loginAction;
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
    public async Task<ActionResult<AdminLoginResource>> Login(
        [FromBody] AdminLoginRequest request)
    {
        var deviceInfo = DeviceInfoHelper.GetCompleteDeviceInfo(HttpContext);
        return Ok(AdminLoginResource.FromResponse(
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
            UserType = "admin",
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
            _refreshTokenService.RevokeModelTokensAsync(ClaimsHelper.GetUserId(User), "Admin"),
            _jwtTokenService.RevokeAllUserTokensAsync(ClaimsHelper.GetUserId(User), "admin", expirationMinutes: 120));

    private async Task RevokeTokenFamilyOrAllAsync() =>
        await (string.IsNullOrWhiteSpace(User.FindFirst("tokenFamilyId")?.Value)
            ? Task.WhenAll(
                _refreshTokenService.RevokeModelTokensAsync(ClaimsHelper.GetUserId(User), "Admin"),
                _jwtTokenService.RevokeAllUserTokensAsync(ClaimsHelper.GetUserId(User), "admin", expirationMinutes: 120))
            : _refreshTokenService.RevokeTokenFamilyAsync(User.FindFirst("tokenFamilyId")!.Value));



    [HttpPost("refresh-token")]
    public async Task<ActionResult<AdminLoginResource>> RefreshToken(
        [FromBody] AdminRefreshTokenRequest request) =>
        Ok(AdminLoginResource.FromResponse(
            await _refreshTokenAction.ExecuteAsync(request.FromRequest())));

    [HttpPost("forget-password")]
    public async Task<IActionResult> ForgetPassword([FromBody] AdminForgetPasswordRequest request)
    {
        await _forgetPasswordAction.ExecuteAsync(request.FromRequest());
        var message = request.Channel == Shared.Enums.CommunicationChannel.Sms
            ? "auth.forget_password_sms_sent"
            : "auth.forget_password_sent";
        return Ok(new { message });
    }




    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] AdminResetPasswordRequest request)
    {
        await _resetPasswordAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "auth.reset_password_success" });
    }




    [HttpPost("reset-password-with-code")]
    public async Task<IActionResult> ResetPasswordWithCode([FromBody] AdminResetPasswordWithCodeRequest request)
    {
        await _resetPasswordWithCodeAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "auth.reset_password_success" });
    }

    [HttpPost("enable-mfa")]
    [Authorize]
    public async Task<IActionResult> EnableMfa([FromBody] AdminEnableMfaRequest request)
    {
        await _enableMfaAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.mfa_enabled" });
    }

    [HttpPost("disable-mfa")]
    [Authorize]
    public async Task<IActionResult> DisableMfa([FromBody] AdminDisableMfaRequest request)
    {
        await _disableMfaAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.mfa_disabled" });
    }

    [HttpPost("send-mfa-code")]
    [Authorize(Policy = "MfaPending")]
    public async Task<IActionResult> SendMfaCode([FromBody] AdminSendMfaCodeRequest? request = null)
    {
        await _sendMfaCodeAction.ExecuteAsync(
            (request ?? new AdminSendMfaCodeRequest { Channel = Shared.Enums.CommunicationChannel.Sms })
                .FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.mfa_code_sent" });
    }

    [HttpPost("verify-mfa")]
    public async Task<ActionResult<AdminLoginResource>> VerifyMfa([FromBody] AdminVerifyMfaRequest request)
    {
        var deviceInfo = DeviceInfoHelper.GetCompleteDeviceInfo(HttpContext);
        return Ok(AdminLoginResource.FromResponse(
            await _verifyMfaAction.ExecuteAsync(
                request.FromRequest(
                    ClaimsHelper.GetUserId(User),
                    User.FindFirst("userType")?.Value ?? "admin",
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
    public async Task<IActionResult> ChangePassword([FromBody] AdminChangePasswordRequest request)
    {
        await _changePasswordAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.change_password_success" });
    }

    [HttpPost("validate-password")]
    [Authorize]
    public async Task<ActionResult> ValidatePassword([FromBody] AdminValidatePasswordRequest request)
    {
        await _validatePasswordAction.ExecuteAsync(
            request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.password_validated" });
    }





    [HttpPost("resend-verification")]
    [Authorize]
    public async Task<IActionResult> ResendVerification([FromBody] AdminResendVerificationRequest? request = null)
    {
        await _resendVerificationAction.ExecuteAsync(
            (request ?? new AdminResendVerificationRequest { Channel = Shared.Enums.CommunicationChannel.Email })
                .FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.verification_code_sent" });
    }





    [HttpPost("verify-email")]
    [Authorize]
    public async Task<IActionResult> VerifyEmail([FromBody] AdminVerifyEmailRequest request)
    {
        await _verifyEmailAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.email_verified" });
    }





    [HttpPost("verify-phone")]
    [Authorize]
    public async Task<IActionResult> VerifyPhone([FromBody] AdminVerifyPhoneRequest request)
    {
        await _verifyPhoneAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "auth.phone_verified" });
    }




    [HttpGet("sessions")]
    [Authorize]
    public async Task<ActionResult<object>> GetSessions() =>
        Ok(await _listSessionsAction.ExecuteAsync(
            new AdminListSessionsRequest
            {
                UserId = ClaimsHelper.GetUserId(User),
                CurrentTokenFamilyId = User.FindFirst("tokenFamilyId")?.Value
            }.FromRequest()));




    [HttpDelete("sessions/{sessionId}")]
    [Authorize]
    public async Task<ActionResult> RevokeSession(long sessionId)
    {
        await _revokeSessionAction.ExecuteAsync(
            new AdminRevokeSessionRequest { UserId = ClaimsHelper.GetUserId(User), SessionId = sessionId }.FromRequest());
        return Ok(new { message = "sessions.revoked_successfully" });
    }
}
