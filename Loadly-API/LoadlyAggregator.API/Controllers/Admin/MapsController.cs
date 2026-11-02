using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Maps;
using LoadlyAggregator.API.Resources.Admin.Maps;
using LoadlyAggregator.Domain.Admin.Actions.Maps;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/maps")]
[Authorize]
public class MapsController : ControllerBase
{
    private readonly ExtractPlaceFromGoogleMapsAction _extractPlaceFromGoogleMapsAction;
    private readonly ExtractPlacesFromGoogleMapsAction _extractPlacesFromGoogleMapsAction;

    public MapsController(
        ExtractPlaceFromGoogleMapsAction extractPlaceFromGoogleMapsAction,
        ExtractPlacesFromGoogleMapsAction extractPlacesFromGoogleMapsAction)
    {
        _extractPlaceFromGoogleMapsAction = extractPlaceFromGoogleMapsAction;
        _extractPlacesFromGoogleMapsAction = extractPlacesFromGoogleMapsAction;
    }

    [HttpGet("extract-place")]
    public async Task<ActionResult<GoogleMapsPlaceResource>> ExtractPlaceFromGoogleMaps(
        [FromQuery] ExtractPlaceFromGoogleMapsRequest request)
    {
        var dto = await _extractPlaceFromGoogleMapsAction.ExecuteAsync(request.FromRequest());
        return Ok(GoogleMapsPlaceResource.FromDto(dto));
    }

    [HttpPost("extract-places")]
    public async Task<ActionResult<List<GoogleMapsPlaceResource>>> ExtractPlacesFromGoogleMaps(
        [FromBody] ExtractPlacesFromGoogleMapsRequest request)
    {
        var dtos = await _extractPlacesFromGoogleMapsAction.ExecuteAsync(request.FromRequest());
        return Ok(dtos.Select(GoogleMapsPlaceResource.FromDto).ToList());
    }
}
