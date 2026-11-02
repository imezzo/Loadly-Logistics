using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Settings.ServiceRequirements;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.Domain.Admin.Actions.ServiceRequirements;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin.Settings;

[ApiController]
[Route("api/admin/settings/service-requirements")]
[Authorize]
public class ServiceRequirementsController : ControllerBase
{
    private readonly CreateServiceRequirementAction _createServiceRequirementAction;
    private readonly GetServiceRequirementAction _getServiceRequirementAction;
    private readonly ListServiceRequirementsAction _listServiceRequirementsAction;
    private readonly UpdateServiceRequirementAction _updateServiceRequirementAction;
    private readonly DeleteServiceRequirementAction _deleteServiceRequirementAction;

    public ServiceRequirementsController(
        CreateServiceRequirementAction createServiceRequirementAction,
        GetServiceRequirementAction getServiceRequirementAction,
        ListServiceRequirementsAction listServiceRequirementsAction,
        UpdateServiceRequirementAction updateServiceRequirementAction,
        DeleteServiceRequirementAction deleteServiceRequirementAction)
    {
        _createServiceRequirementAction = createServiceRequirementAction;
        _getServiceRequirementAction = getServiceRequirementAction;
        _listServiceRequirementsAction = listServiceRequirementsAction;
        _updateServiceRequirementAction = updateServiceRequirementAction;
        _deleteServiceRequirementAction = deleteServiceRequirementAction;
    }

    [HttpGet]
    [HasPermission(ListServiceRequirementsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListServiceRequirementsRequest request) =>
        Ok((await _listServiceRequirementsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<ServiceRequirementDto, ServiceRequirementResource>(dto => ServiceRequirementResource.FromDto(dto)));

    [HttpPost]
    [HasPermission(CreateServiceRequirementRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateServiceRequirementRequest request)
    {
        await _createServiceRequirementAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "service_requirements.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetServiceRequirementRequest.Permission)]
    public async Task<ActionResult<ServiceRequirementResource>> Get(long id) =>
        Ok(ServiceRequirementResource.FromDto(await _getServiceRequirementAction.ExecuteAsync(new GetServiceRequirementRequest { Id = id }.FromRequest())));

    [HttpPut("{id}")]
    [HasPermission(UpdateServiceRequirementRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateServiceRequirementRequest request)
    {
        request.Id = id;
        await _updateServiceRequirementAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "service_requirements.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteServiceRequirementRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteServiceRequirementAction.ExecuteAsync(new DeleteServiceRequirementRequest { Id = id }.FromRequest());
        return Ok(new { message = "service_requirements.deleted_successfully" });
    }
}
