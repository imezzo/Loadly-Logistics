using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.API.Controllers;

[ApiController]
[Route("api/lookups")]
[Authorize]
public class LookupsController(ILookupFactory lookupFactory) : ControllerBase
{
    private readonly ILookupFactory _lookupFactory = lookupFactory;
    private static readonly string[] UserEntities = ["admins", "companies", "contractors", "collectors"];

    [HttpGet("{entity}/{lookup}")]
    public async Task<ActionResult<LookupResponse>> GetLookup(
        string entity,
        string lookup,
        [FromQuery] LookupRequest? request = null)
    {
        request ??= new LookupRequest();

        if (UserEntities.Contains(entity, StringComparer.OrdinalIgnoreCase))
        {
            var userType = User.FindFirst("userType")?.Value;
            if (!string.Equals(userType, "admin", StringComparison.OrdinalIgnoreCase))
            {
                return StatusCode(403, new { message = "Admin access required for user entity lookups" });
            }
        }

        var response = await _lookupFactory.ExecuteLookupAsync(entity, lookup, request);
        return Ok(response);
    }
}
