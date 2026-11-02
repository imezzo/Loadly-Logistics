using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Leads;
using LoadlyAggregator.API.Resources.Admin.Leads;
using LoadlyAggregator.Domain.Admin.Actions.Leads;
using LoadlyAggregator.Domain.Admin.DTOs.Leads;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/leads")]
[Authorize]
public class LeadsController : ControllerBase
{
    private readonly CreateLeadAction _createLeadAction;
    private readonly GetLeadAction _getLeadAction;
    private readonly GetLeadByPhoneAction _getLeadByPhoneAction;
    private readonly ListLeadsAction _listLeadsAction;
    private readonly UpdateLeadAction _updateLeadAction;
    private readonly DeleteLeadAction _deleteLeadAction;

    public LeadsController(
        CreateLeadAction createLeadAction,
        GetLeadAction getLeadAction,
        GetLeadByPhoneAction getLeadByPhoneAction,
        ListLeadsAction listLeadsAction,
        UpdateLeadAction updateLeadAction,
        DeleteLeadAction deleteLeadAction)
    {
        _createLeadAction = createLeadAction;
        _getLeadAction = getLeadAction;
        _getLeadByPhoneAction = getLeadByPhoneAction;
        _listLeadsAction = listLeadsAction;
        _updateLeadAction = updateLeadAction;
        _deleteLeadAction = deleteLeadAction;
    }

    [HttpGet]
    [HasPermission(ListLeadsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListLeadsRequest request) =>
        Ok((await _listLeadsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<LeadDto, LeadResource>(dto => LeadResource.FromDto(dto)));

    [HttpPost]
    [HasPermission(CreateLeadRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateLeadRequest request)
    {
        await _createLeadAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "leads.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetLeadRequest.Permission)]
    public async Task<ActionResult<LeadResource>> Get(long id) =>
        Ok(LeadResource.FromDto(await _getLeadAction.ExecuteAsync(new GetLeadRequest { Id = id }.FromRequest())));

    [HttpGet("by-phone")]
    // [HasPermission(GetLeadByPhoneRequest.Permission)]
    public async Task<ActionResult<LeadResource>> GetByPhone([FromQuery] GetLeadByPhoneRequest request) =>
        Ok(LeadResource.FromDto(await _getLeadByPhoneAction.ExecuteAsync(request.FromRequest())));

    [HttpPut("{id}")]
    [HasPermission(UpdateLeadRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateLeadRequest request)
    {
        request.Id = id;
        await _updateLeadAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "leads.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteLeadRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteLeadAction.ExecuteAsync(new DeleteLeadRequest { Id = id }.FromRequest());
        return Ok(new { message = "leads.deleted_successfully" });
    }
}
