using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.Sectors;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.Sectors;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/sectors")]
[Authorize]
public class SectorsController : ControllerBase
{
    private readonly CreateSectorAction _createSectorAction;
    private readonly GetSectorAction _getSectorAction;
    private readonly ListSectorsAction _listSectorsAction;
    private readonly UpdateSectorAction _updateSectorAction;
    private readonly DeleteSectorAction _deleteSectorAction;

    public SectorsController(
        CreateSectorAction createSectorAction,
        GetSectorAction getSectorAction,
        ListSectorsAction listSectorsAction,
        UpdateSectorAction updateSectorAction,
        DeleteSectorAction deleteSectorAction)
    {
        _createSectorAction = createSectorAction;
        _getSectorAction = getSectorAction;
        _listSectorsAction = listSectorsAction;
        _updateSectorAction = updateSectorAction;
        _deleteSectorAction = deleteSectorAction;
    }

    [HttpPost]
    [HasPermission(CreateSectorRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateSectorRequest request)
    {
        await _createSectorAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "sectors.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetSectorRequest.Permission)]
    public async Task<ActionResult<SectorResource>> Get(long id) =>
        Ok(SectorResource.FromDto(await _getSectorAction.ExecuteAsync(new GetSectorRequest { Id = id }.FromRequest())));

    [HttpGet]
    [HasPermission(ListSectorsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListSectorsRequest request) =>
        Ok((await _listSectorsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<SectorDto, SectorResource>(dto => SectorResource.FromDto(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateSectorRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateSectorRequest request)
    {
        request.Id = id;
        await _updateSectorAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "sectors.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteSectorRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteSectorAction.ExecuteAsync(new DeleteSectorRequest { Id = id }.FromRequest());
        return Ok(new { message = "sectors.deleted_successfully" });
    }
}
