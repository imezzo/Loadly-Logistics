using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.IndustryTypes;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.IndustryTypes;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/industry-types")]
[Authorize]
public class IndustryTypesController : ControllerBase
{
    private readonly CreateIndustryTypeAction _createIndustryTypeAction;
    private readonly GetIndustryTypeAction _getIndustryTypeAction;
    private readonly ListIndustryTypesAction _listIndustryTypesAction;
    private readonly UpdateIndustryTypeAction _updateIndustryTypeAction;
    private readonly DeleteIndustryTypeAction _deleteIndustryTypeAction;

    public IndustryTypesController(
        CreateIndustryTypeAction createIndustryTypeAction,
        GetIndustryTypeAction getIndustryTypeAction,
        ListIndustryTypesAction listIndustryTypesAction,
        UpdateIndustryTypeAction updateIndustryTypeAction,
        DeleteIndustryTypeAction deleteIndustryTypeAction)
    {
        _createIndustryTypeAction = createIndustryTypeAction;
        _getIndustryTypeAction = getIndustryTypeAction;
        _listIndustryTypesAction = listIndustryTypesAction;
        _updateIndustryTypeAction = updateIndustryTypeAction;
        _deleteIndustryTypeAction = deleteIndustryTypeAction;
    }

    [HttpGet]
    [HasPermission(ListIndustryTypesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListIndustryTypesRequest request) =>
        Ok((await _listIndustryTypesAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<IndustryTypeDto, IndustryTypeResource>(dto => IndustryTypeResource.FromDto(dto)));

    [HttpPost]
    [HasPermission(CreateIndustryTypeRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateIndustryTypeRequest request)
    {
        await _createIndustryTypeAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "industry_types.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetIndustryTypeRequest.Permission)]
    public async Task<ActionResult<IndustryTypeResource>> Get(long id) =>
        Ok(IndustryTypeResource.FromDto(await _getIndustryTypeAction.ExecuteAsync(new GetIndustryTypeRequest { Id = id }.FromRequest())));

    [HttpPut("{id}")]
    [HasPermission(UpdateIndustryTypeRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateIndustryTypeRequest request)
    {
        request.Id = id;
        await _updateIndustryTypeAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "industry_types.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteIndustryTypeRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteIndustryTypeAction.ExecuteAsync(new DeleteIndustryTypeRequest { Id = id }.FromRequest());
        return Ok(new { message = "industry_types.deleted_successfully" });
    }
}
