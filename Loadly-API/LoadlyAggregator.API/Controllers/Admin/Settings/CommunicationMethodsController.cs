using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.CommunicationMethods;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.CommunicationMethods;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/communication-methods")]
[Authorize]
public class CommunicationMethodsController : ControllerBase
{
    private readonly CreateCommunicationMethodAction _createCommunicationMethodAction;
    private readonly GetCommunicationMethodAction _getCommunicationMethodAction;
    private readonly ListCommunicationMethodsAction _listCommunicationMethodsAction;
    private readonly UpdateCommunicationMethodAction _updateCommunicationMethodAction;
    private readonly DeleteCommunicationMethodAction _deleteCommunicationMethodAction;

    public CommunicationMethodsController(
        CreateCommunicationMethodAction createCommunicationMethodAction,
        GetCommunicationMethodAction getCommunicationMethodAction,
        ListCommunicationMethodsAction listCommunicationMethodsAction,
        UpdateCommunicationMethodAction updateCommunicationMethodAction,
        DeleteCommunicationMethodAction deleteCommunicationMethodAction)
    {
        _createCommunicationMethodAction = createCommunicationMethodAction;
        _getCommunicationMethodAction = getCommunicationMethodAction;
        _listCommunicationMethodsAction = listCommunicationMethodsAction;
        _updateCommunicationMethodAction = updateCommunicationMethodAction;
        _deleteCommunicationMethodAction = deleteCommunicationMethodAction;
    }

    [HttpGet]
    [HasPermission(ListCommunicationMethodsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListCommunicationMethodsRequest request) =>
        Ok((await _listCommunicationMethodsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<CommunicationMethodDto, CommunicationMethodResource>(dto => CommunicationMethodResource.FromDto(dto)));

    [HttpPost]
    [HasPermission(CreateCommunicationMethodRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateCommunicationMethodRequest request)
    {
        await _createCommunicationMethodAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "communication_methods.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetCommunicationMethodRequest.Permission)]
    public async Task<ActionResult<CommunicationMethodResource>> Get(long id) =>
        Ok(CommunicationMethodResource.FromDto(await _getCommunicationMethodAction.ExecuteAsync(new GetCommunicationMethodRequest { Id = id }.FromRequest())));

    [HttpPut("{id}")]
    [HasPermission(UpdateCommunicationMethodRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateCommunicationMethodRequest request)
    {
        request.Id = id;
        await _updateCommunicationMethodAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "communication_methods.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteCommunicationMethodRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteCommunicationMethodAction.ExecuteAsync(new DeleteCommunicationMethodRequest { Id = id }.FromRequest());
        return Ok(new { message = "communication_methods.deleted_successfully" });
    }
}
