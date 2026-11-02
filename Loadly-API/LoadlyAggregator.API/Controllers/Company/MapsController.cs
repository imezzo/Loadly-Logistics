using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Company.Maps;
using LoadlyAggregator.API.Resources.Company.Maps;
using LoadlyAggregator.Domain.Company.Actions.Maps;

namespace LoadlyAggregator.API.Controllers.Company;

[ApiController]
[Route("api/company/maps")]
[Authorize]
public class MapsController : ControllerBase
{
    private readonly CompanyExtractPlaceFromGoogleMapsAction _extractPlaceFromGoogleMapsAction;
    private readonly CompanyExtractPlacesFromGoogleMapsAction _extractPlacesFromGoogleMapsAction;

    public MapsController(
        CompanyExtractPlaceFromGoogleMapsAction extractPlaceFromGoogleMapsAction,
        CompanyExtractPlacesFromGoogleMapsAction extractPlacesFromGoogleMapsAction)
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
