using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.ContractApprovals;
using LoadlyAggregator.API.Requests.Admin.Contracts;
using LoadlyAggregator.API.Resources.Admin.Contracts;
using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.Actions.ContractApprovals;
using LoadlyAggregator.Domain.Admin.Actions.Contracts;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Infrastructure.Authorization;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/contracts")]
[Authorize]
public class ContractsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly CreateContractAction _createContractAction;
    private readonly GetContractAction _getContractAction;
    private readonly GetContractConsumptionTypeAction _getContractConsumptionTypeAction;
    private readonly GetContractVehicleTypeIdAction _getContractVehicleTypeIdAction;
    private readonly GetContractVehicleTypeAction _getContractVehicleTypeAction;
    private readonly GetContractDestinationAction _getContractDestinationAction;
    private readonly ListContractsAction _listContractsAction;
    private readonly CalculateContractDestinationCostsAction _calculateContractDestinationCostsAction;
    private readonly CalculateContractSlotChargesAction _calculateContractSlotChargesAction;
    private readonly UpdateContractAction _updateContractAction;
    private readonly DeleteContractAction _deleteContractAction;
    private readonly PullBackToDraftContractAction _pullBackToDraftContractAction;
    private readonly DeactivateContractAction _deactivateContractAction;
    private readonly ReactivateContractAction _reactivateContractAction;
    private readonly TerminateContractAction _terminateContractAction;
    private readonly SubmitContractApprovalDecisionAction _submitContractApprovalDecisionAction;
    private readonly GetApprovalStatusAction _getApprovalStatusAction;
    private readonly ListContractApprovalsAction _listContractApprovalsAction;
    private readonly ExportContractDestinationsExcelAction _exportContractDestinationsExcelAction;
    private readonly ExportContractSlotChargesExcelAction _exportContractSlotChargesExcelAction;

    public ContractsController(
        IMapper mapper,
        CreateContractAction createContractAction,
        GetContractAction getContractAction,
        GetContractConsumptionTypeAction getContractConsumptionTypeAction,
        GetContractVehicleTypeIdAction getContractVehicleTypeIdAction,
        GetContractVehicleTypeAction getContractVehicleTypeAction,
        GetContractDestinationAction getContractDestinationAction,
        ListContractsAction listContractsAction,
        CalculateContractDestinationCostsAction calculateContractDestinationCostsAction,
        CalculateContractSlotChargesAction calculateContractSlotChargesAction,
        UpdateContractAction updateContractAction,
        DeleteContractAction deleteContractAction,
        PullBackToDraftContractAction pullBackToDraftContractAction,
        DeactivateContractAction deactivateContractAction,
        ReactivateContractAction reactivateContractAction,
        TerminateContractAction terminateContractAction,
        SubmitContractApprovalDecisionAction submitContractApprovalDecisionAction,
        GetApprovalStatusAction getApprovalStatusAction,
        ListContractApprovalsAction listContractApprovalsAction,
        ExportContractDestinationsExcelAction exportContractDestinationsExcelAction,
        ExportContractSlotChargesExcelAction exportContractSlotChargesExcelAction)
    {
        _mapper = mapper;
        _createContractAction = createContractAction;
        _getContractAction = getContractAction;
        _getContractConsumptionTypeAction = getContractConsumptionTypeAction;
        _getContractVehicleTypeIdAction = getContractVehicleTypeIdAction;
        _getContractVehicleTypeAction = getContractVehicleTypeAction;
        _getContractDestinationAction = getContractDestinationAction;
        _listContractsAction = listContractsAction;
        _calculateContractDestinationCostsAction = calculateContractDestinationCostsAction;
        _calculateContractSlotChargesAction = calculateContractSlotChargesAction;
        _updateContractAction = updateContractAction;
        _deleteContractAction = deleteContractAction;
        _pullBackToDraftContractAction = pullBackToDraftContractAction;
        _deactivateContractAction = deactivateContractAction;
        _reactivateContractAction = reactivateContractAction;
        _terminateContractAction = terminateContractAction;
        _submitContractApprovalDecisionAction = submitContractApprovalDecisionAction;
        _getApprovalStatusAction = getApprovalStatusAction;
        _listContractApprovalsAction = listContractApprovalsAction;
        _exportContractDestinationsExcelAction = exportContractDestinationsExcelAction;
        _exportContractSlotChargesExcelAction = exportContractSlotChargesExcelAction;
    }



    [HttpPost]
    [HasPermission(CreateContractRequest.Permission)]
    [Consumes("multipart/form-data")]
    [RequestFormLimits(ValueCountLimit = 10240)]
    public async Task<ActionResult> Create([FromForm] CreateContractRequest request)
    {
        await _createContractAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "contracts.created_successfully" });
    }

    [HttpGet("{id:long}/destinations/excel")]
    [HasPermission(ExportContractDestinationsExcelRequest.Permission)]
    public async Task<IActionResult> ExportDestinationsExcel(long id, [FromQuery] ExportContractDestinationsExcelRequest request)
    {
        request.Id = id;
        ContractDestinationsExcelFileResource resource = ContractDestinationsExcelFileResource.FromDto(
            await _exportContractDestinationsExcelAction.ExecuteAsync(request.FromRequest()));
        return File(resource.Content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", resource.FileName);
    }

    [HttpGet("destinations/{destinationId}")]
    // [HasPermission(GetContractDestinationRequest.Permission)]
    public async Task<ActionResult<ContractDestinationResource>> GetDestination(long destinationId) =>
        Ok(_mapper.Map<ContractDestinationResource>(await _getContractDestinationAction.ExecuteAsync(
            new GetContractDestinationRequest { DestinationId = destinationId }.FromRequest())));

    [HttpGet("{id:long}/consumption-type")]
    public async Task<ActionResult<ContractConsumptionTypeResource>> GetConsumptionType(long id) =>
        Ok(ContractConsumptionTypeResource.FromDto(await _getContractConsumptionTypeAction.ExecuteAsync(new GetContractRequest { Id = id }.FromRequest())));

    [HttpGet("{contractId:long}/vehicle-types/{vehicleTypeId:long}")]
    [HasPermission(GetContractVehicleTypeIdRequest.Permission)]
    public async Task<ActionResult<ContractVehicleTypeIdResource>> GetContractVehicleTypeId(long contractId, long vehicleTypeId) =>
        Ok(ContractVehicleTypeIdResource.FromDto(await _getContractVehicleTypeIdAction.ExecuteAsync(
            new GetContractVehicleTypeIdRequest { ContractId = contractId, VehicleTypeId = vehicleTypeId }.FromRequest())));

    [HttpGet("vehicle-types/{id:long}")]
    [HasPermission(GetContractVehicleTypeRequest.Permission)]
    public async Task<ActionResult<ContractVehicleTypeRelationResource>> GetContractVehicleType(long id) =>
        Ok(_mapper.Map<ContractVehicleTypeRelationResource>(await _getContractVehicleTypeAction.ExecuteAsync(
            new GetContractVehicleTypeRequest { Id = id }.FromRequest())));

    [HttpGet("{id}")]
    [HasPermission(GetContractRequest.Permission)]
    public async Task<ActionResult<ContractResource>> Get(long id) =>
        Ok(_mapper.Map<ContractResource>(await _getContractAction.ExecuteAsync(new GetContractRequest { Id = id }.FromRequest())));

    [HttpGet]
    [HasPermission(ListContractsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListContractsRequest request) =>
        Ok((await _listContractsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<ContractDto, ContractResource>(dto => _mapper.Map<ContractResource>(dto)));

    [HttpPost("destinations/calculate")]
    [HasPermission(CalculateContractDestinationCostsRequest.Permission)]
    public async Task<ActionResult<CalculateContractDestinationCostsDto>> CalculateDestinationsCosts(
        [FromBody] CalculateContractDestinationCostsRequest request) =>
        Ok(await _calculateContractDestinationCostsAction.ExecuteAsync(request.FromRequest()));

    [HttpGet("{id:long}/slot-charges/excel")]
    [HasPermission(ExportContractSlotChargesExcelRequest.Permission)]
    public async Task<IActionResult> ExportSlotChargesExcel(long id, [FromQuery] ExportContractSlotChargesExcelRequest request)
    {
        request.ContractId = id;
        ContractSlotChargesExcelFileResource resource = ContractSlotChargesExcelFileResource.FromDto(
            await _exportContractSlotChargesExcelAction.ExecuteAsync(request.FromRequest()));
        return File(resource.Content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", resource.FileName);
    }

    [HttpPost("{id}/slot-charges/calculate")]
    [HasPermission(CalculateContractSlotChargesRequest.Permission)]
    public async Task<ActionResult<CalculateIncludedContractSlotChargesResource>> CalculateSlotCharges(
        long id,
        [FromBody] CalculateContractSlotChargesRequest request) =>
        Ok(CalculateIncludedContractSlotChargesResource.FromDto(
            await _calculateContractSlotChargesAction.ExecuteAsync(request.FromRequest(id))));

    [HttpPut("{id}")]
    [HasPermission(UpdateContractRequest.Permission)]
    [Consumes("multipart/form-data")]
    [RequestFormLimits(ValueCountLimit = 10240)]
    public async Task<ActionResult> Update(long id, [FromForm] UpdateContractRequest request)
    {
        request.Id = id;
        await _updateContractAction.ExecuteAsync(request.FromRequest(id));
        return Ok(new { message = "contracts.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteContractRequest.Permission)]
    public async Task<ActionResult> Delete(long id, [FromRoute] DeleteContractRequest request)
    {
        request.Id = id;
        await _deleteContractAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "contracts.deleted_successfully" });
    }

    [HttpPost("{id}/pull-back")]
    [HasPermission(PullBackToDraftContractRequest.Permission)]
    public async Task<ActionResult> PullBackToDraft(long id)
    {
        await _pullBackToDraftContractAction.ExecuteAsync(new PullBackToDraftContractRequest { Id = id }.FromRequest());
        return Ok(new { message = "contracts.pulled_back_to_draft_successfully" });
    }

    [HttpPost("{id}/deactivate")]
    [HasPermission(DeactivateContractRequest.Permission)]
    public async Task<ActionResult> Deactivate(long id, [FromBody] DeactivateContractRequest request)
    {
        request.Id = id;
        await _deactivateContractAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "contracts.deactivated_successfully" });
    }

    [HttpPost("{id}/reactivate")]
    [HasPermission(ReactivateContractRequest.Permission)]
    public async Task<ActionResult> Reactivate(long id, [FromBody] ReactivateContractRequest request)
    {
        request.Id = id;
        await _reactivateContractAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "contracts.reactivated_successfully" });
    }

    [HttpPost("{id}/terminate")]
    [HasPermission(TerminateContractRequest.Permission)]
    public async Task<ActionResult> Terminate(long id, [FromBody] TerminateContractRequest request)
    {
        request.Id = id;
        await _terminateContractAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "contracts.terminated_successfully" });
    }

    [HttpGet("{id}/approvals/status")]
    [HasPermission(GetApprovalStatusRequest.Permission)]
    public async Task<ActionResult<object>> GetApprovalStatus(long id) =>
        Ok(await _getApprovalStatusAction.ExecuteAsync(new GetApprovalStatusRequest { ContractId = id }.FromRequest()));

    [HttpPost("{id}/approvals/submit-decision")]
    public async Task<ActionResult> SubmitApprovalDecision(
        long id,
        [FromBody] SubmitContractApprovalDecisionRequest request)
    {
        request.Id = id;
        await _submitContractApprovalDecisionAction.ExecuteAsync(request.FromRequest());
        var messageKey = request.Action == ContractApprovalDecision.Approve
            ? "contracts.approved_successfully"
            : "contracts.rejected_successfully";
        return Ok(new { message = messageKey });
    }

    [HttpGet("{id}/approvals")]
    [HasPermission(ListContractApprovalsRequest.Permission)]
    public async Task<ActionResult<ListingDto<ContractApprovalDto>>> ListApprovals(
        long id,
        [FromQuery] ListContractApprovalsRequest request)
    {
        request.ContractId = id;
        return Ok(await _listContractApprovalsAction.ExecuteAsync(request.FromRequest()));
    }
}
