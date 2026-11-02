using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.Containers;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.Containers;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/containers")]
[Authorize]
public class ContainersController : ControllerBase
{
    private readonly CreateContainerAction _createContainerAction;
    private readonly GetContainerAction _getContainerAction;
    private readonly ListContainersAction _listContainersAction;
    private readonly UpdateContainerAction _updateContainerAction;
    private readonly DeleteContainerAction _deleteContainerAction;

    public ContainersController(
        CreateContainerAction createContainerAction,
        GetContainerAction getContainerAction,
        ListContainersAction listContainersAction,
        UpdateContainerAction updateContainerAction,
        DeleteContainerAction deleteContainerAction)
    {
        _createContainerAction = createContainerAction;
        _getContainerAction = getContainerAction;
        _listContainersAction = listContainersAction;
        _updateContainerAction = updateContainerAction;
        _deleteContainerAction = deleteContainerAction;
    }

    [HttpGet]
    [HasPermission(ListContainersRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListContainersRequest request) =>
        Ok((await _listContainersAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<ContainerDto, ContainerResource>(dto => ContainerResource.FromDto(dto)));

    [HttpPost]
    [HasPermission(CreateContainerRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateContainerRequest request)
    {
        await _createContainerAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "containers.created_successfully" });
    }

    [HttpGet("{id:long}")]
    [HasPermission(GetContainerRequest.Permission)]
    public async Task<ActionResult<ContainerResource>> Get(long id) =>
        Ok(ContainerResource.FromDto(await _getContainerAction.ExecuteAsync(new GetContainerRequest { Id = id }.FromRequest())));

    [HttpPut("{id:long}")]
    [HasPermission(UpdateContainerRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateContainerRequest request)
    {
        request.Id = id;
        await _updateContainerAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "containers.updated_successfully" });
    }

    [HttpDelete("{id:long}")]
    [HasPermission(DeleteContainerRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteContainerAction.ExecuteAsync(new DeleteContainerRequest { Id = id }.FromRequest());
        return Ok(new { message = "containers.deleted_successfully" });
    }
}
