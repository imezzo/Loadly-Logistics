using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Common.AppVersion;
using LoadlyAggregator.API.Resources.Common.AppVersion;
using LoadlyAggregator.Domain.Common.Actions.AppVersion;

namespace LoadlyAggregator.API.Controllers.Common;

[ApiController]
[Route("api/app-version")]
[AllowAnonymous]
public class AppVersionController : ControllerBase
{
    private readonly CheckAppVersionAction _checkAppVersionAction;

    public AppVersionController(CheckAppVersionAction checkAppVersionAction)
    {
        _checkAppVersionAction = checkAppVersionAction;
    }

    [HttpGet("check")]
    public async Task<ActionResult<AppVersionCheckResource>> Check([FromQuery] CheckAppVersionRequest request)
    {
        var response = await _checkAppVersionAction.ExecuteAsync(request.FromRequest());
        return Ok(AppVersionCheckResource.FromResponse(response));
    }
}
