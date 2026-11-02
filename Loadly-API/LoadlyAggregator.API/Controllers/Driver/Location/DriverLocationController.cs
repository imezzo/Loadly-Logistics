using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Driver.Location;
using LoadlyAggregator.Domain.Driver.Actions.Location;

namespace LoadlyAggregator.API.Controllers.Driver.Location;

[ApiController]
[Route("api/driver")]
[Authorize]
public class DriverLocationController : ControllerBase
{
    private readonly DriverGoOnlineAction _goOnlineAction;
    private readonly DriverGoOfflineAction _goOfflineAction;

    public DriverLocationController(
        DriverGoOnlineAction goOnlineAction,
        DriverGoOfflineAction goOfflineAction)
    {
        _goOnlineAction = goOnlineAction;
        _goOfflineAction = goOfflineAction;
    }

    [HttpPost("go-online")]
    public async Task<IActionResult> GoOnline([FromBody] DriverGoOnlineRequest request)
    {
        await _goOnlineAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "drivers.go_online_success" });
    }

    [HttpPost("go-offline")]
    public async Task<IActionResult> GoOffline([FromBody] DriverGoOfflineRequest? request = null)
    {
        await _goOfflineAction.ExecuteAsync(
            (request ?? new DriverGoOfflineRequest()).FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "drivers.go_offline_success" });
    }

    [HttpPost("location/ping")]
    public IActionResult Ping()
    {
        return Ok(new { message = "drivers.location_updated_successfully" });
    }
}
