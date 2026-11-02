using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using LoadlyAggregator.Domain.Common.Actions.Maintenance;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Controllers.Common;




[ApiController]
[Route("api/maintenance")]
public class MaintenanceController : ControllerBase
{
    private readonly TriggerRefreshTokenCleanupAction _triggerCleanupAction;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<MaintenanceController> _logger;

    public MaintenanceController(
        TriggerRefreshTokenCleanupAction triggerCleanupAction,
        IWebHostEnvironment environment,
        ILogger<MaintenanceController> logger)
    {
        _triggerCleanupAction = triggerCleanupAction;
        _environment = environment;
        _logger = logger;
    }









    [HttpPost("cleanup-refresh-tokens")]
    [Authorize]
    public async Task<ActionResult<RefreshTokenCleanupResult>> CleanupRefreshTokens()
    {
        if (!_environment.IsDevelopment())
        {
            _logger.LogWarning(
                "Attempted to access maintenance endpoint in {Environment} environment from IP {IpAddress}",
                _environment.EnvironmentName,
                HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown");
            return NotFound();
        }

        var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        var isLocalhost = remoteIp == "127.0.0.1" || remoteIp == "::1";
        var isPrivateNetwork = !string.IsNullOrEmpty(remoteIp) && (
            remoteIp.StartsWith("192.168.") ||
            remoteIp.StartsWith("10.") ||
            (remoteIp.StartsWith("172.") && IsPrivate172Range(remoteIp))
        );

        if (!isLocalhost && !isPrivateNetwork)
        {
            _logger.LogWarning(
                "Attempted to access maintenance endpoint from non-local IP {IpAddress} in Development environment",
                remoteIp ?? "unknown");
            return StatusCode(403);
        }

        _logger.LogInformation("Manual refresh token cleanup triggered via API");

        var result = await _triggerCleanupAction.ExecuteAsync();

        if (!result.Success)
        {
            return StatusCode(500, result);
        }

        return Ok(result);
    }

    private static bool IsPrivate172Range(string ip)
    {
        if (!ip.StartsWith("172."))
            return false;
        var parts = ip.Split('.');
        if (parts.Length < 2 || !int.TryParse(parts[1], out var secondOctet))
            return false;
        return secondOctet >= 16 && secondOctet <= 31;
    }
}
