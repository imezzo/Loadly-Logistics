using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Admin.DeviceToken;
using LoadlyAggregator.Domain.Common.Actions.DeviceToken;
using LoadlyAggregator.Infrastructure.Helpers;

namespace LoadlyAggregator.API.Controllers.Admin.DeviceToken;

[ApiController]
[Route("api/admin/device-token")]
[Authorize]
public class DeviceTokenController : ControllerBase
{
    private readonly RegisterDeviceTokenAction _registerDeviceTokenAction;

    public DeviceTokenController(RegisterDeviceTokenAction registerDeviceTokenAction)
    {
        _registerDeviceTokenAction = registerDeviceTokenAction;
    }

    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDeviceTokenRequest request)
    {
        var userId = ClaimsHelper.GetUserId(User);
        var deviceInfo = DeviceInfoHelper.GetCompleteDeviceInfo(HttpContext);

        await _registerDeviceTokenAction.ExecuteAsync(
            request.FromRequest(
                userId,
                deviceInfo.DeviceId,
                deviceInfo.DeviceModel,
                deviceInfo.DeviceOs,
                deviceInfo.AppVersion));

        return Ok(new { message = "device_token.registered_successfully" });
    }
}
