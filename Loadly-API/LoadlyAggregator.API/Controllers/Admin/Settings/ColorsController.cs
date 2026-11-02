using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.Colors;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.Colors;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/colors")]
[Authorize]
public class ColorsController : ControllerBase
{
    private readonly CreateColorAction _createColorAction;
    private readonly GetColorAction _getColorAction;
    private readonly ListColorsAction _listColorsAction;
    private readonly UpdateColorAction _updateColorAction;
    private readonly DeleteColorAction _deleteColorAction;

    public ColorsController(
        CreateColorAction createColorAction,
        GetColorAction getColorAction,
        ListColorsAction listColorsAction,
        UpdateColorAction updateColorAction,
        DeleteColorAction deleteColorAction)
    {
        _createColorAction = createColorAction;
        _getColorAction = getColorAction;
        _listColorsAction = listColorsAction;
        _updateColorAction = updateColorAction;
        _deleteColorAction = deleteColorAction;
    }

    [HttpPost]
    [HasPermission(CreateColorRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateColorRequest request)
    {
        await _createColorAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "colors.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetColorRequest.Permission)]
    public async Task<ActionResult<ColorResource>> Get(long id) =>
        Ok(ColorResource.FromDto(await _getColorAction.ExecuteAsync(new GetColorRequest { Id = id }.FromRequest())));

    [HttpGet]
    public async Task<ActionResult> List([FromQuery] ListColorsRequest request) =>
        Ok((await _listColorsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<ColorDto, ColorResource>(dto => ColorResource.FromDto(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateColorRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateColorRequest request)
    {
        request.Id = id;
        await _updateColorAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "colors.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteColorRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteColorAction.ExecuteAsync(new DeleteColorRequest { Id = id }.FromRequest());
        return Ok(new { message = "colors.deleted_successfully" });
    }
}
