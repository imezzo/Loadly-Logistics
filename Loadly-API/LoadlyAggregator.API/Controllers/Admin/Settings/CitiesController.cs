using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.Cities;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.Cities;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/addresses/cities")]
[Authorize]
public class CitiesController : ControllerBase
{
    private readonly CreateCityAction _createCityAction;
    private readonly GetCityAction _getCityAction;
    private readonly ListCitiesAction _listCitiesAction;
    private readonly UpdateCityAction _updateCityAction;
    private readonly DeleteCityAction _deleteCityAction;

    public CitiesController(
        CreateCityAction createCityAction,
        GetCityAction getCityAction,
        ListCitiesAction listCitiesAction,
        UpdateCityAction updateCityAction,
        DeleteCityAction deleteCityAction)
    {
        _createCityAction = createCityAction;
        _getCityAction = getCityAction;
        _listCitiesAction = listCitiesAction;
        _updateCityAction = updateCityAction;
        _deleteCityAction = deleteCityAction;
    }

    [HttpPost]
    [HasPermission(CreateCityRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateCityRequest request)
    {
        await _createCityAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "cities.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetCityRequest.Permission)]
    public async Task<ActionResult<CityResource>> Get(long id) =>
        Ok(CityResource.FromDto(await _getCityAction.ExecuteAsync(new GetCityRequest { Id = id }.FromRequest())));

    [HttpGet]
    [HasPermission(ListCitiesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListCitiesRequest request) =>
        Ok((await _listCitiesAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<CityDto, CityResource>(dto => CityResource.FromDto(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateCityRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateCityRequest request)
    {
        request.Id = id;
        await _updateCityAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "cities.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteCityRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteCityAction.ExecuteAsync(new DeleteCityRequest { Id = id }.FromRequest());
        return Ok(new { message = "cities.deleted_successfully" });
    }
}
