using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Contracts;
using LoadlyAggregator.API.Resources.Admin.Contracts;
using LoadlyAggregator.Domain.Admin.Actions.Contracts;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/contract-billing-snapshots")]
[Authorize]
public class ContractBillingSnapshotsController : ControllerBase
{
    private readonly ExportContractBillingSnapshotExcelAction _exportContractBillingSnapshotExcelAction;
    private readonly CreateDraftContractBillingSnapshotAction _createDraftContractBillingSnapshotAction;
    private readonly UpdateDraftContractBillingSnapshotAction _updateDraftContractBillingSnapshotAction;
    private readonly PatchContractBillingSnapshotCalendarDayAction _patchContractBillingSnapshotCalendarDayAction;
    private readonly PatchContractBillingSnapshotCalendarDayTripCountAction _patchContractBillingSnapshotCalendarDayTripCountAction;
    private readonly PatchContractBillingSnapshotCommercialTermsAction _patchContractBillingSnapshotCommercialTermsAction;
    private readonly DeleteContractBillingSnapshotCommercialTermsAction _deleteContractBillingSnapshotCommercialTermsAction;
    private readonly GetContractBillingSnapshotAction _getContractBillingSnapshotAction;
    private readonly ListContractBillingSnapshotsAction _listContractBillingSnapshotsAction;
    private readonly DeleteContractBillingSnapshotAction _deleteContractBillingSnapshotAction;

    public ContractBillingSnapshotsController(
        ExportContractBillingSnapshotExcelAction exportContractBillingSnapshotExcelAction,
        CreateDraftContractBillingSnapshotAction createDraftContractBillingSnapshotAction,
        UpdateDraftContractBillingSnapshotAction updateDraftContractBillingSnapshotAction,
        PatchContractBillingSnapshotCalendarDayAction patchContractBillingSnapshotCalendarDayAction,
        PatchContractBillingSnapshotCalendarDayTripCountAction patchContractBillingSnapshotCalendarDayTripCountAction,
        PatchContractBillingSnapshotCommercialTermsAction patchContractBillingSnapshotCommercialTermsAction,
        DeleteContractBillingSnapshotCommercialTermsAction deleteContractBillingSnapshotCommercialTermsAction,
        GetContractBillingSnapshotAction getContractBillingSnapshotAction,
        ListContractBillingSnapshotsAction listContractBillingSnapshotsAction,
        DeleteContractBillingSnapshotAction deleteContractBillingSnapshotAction)
    {
        _exportContractBillingSnapshotExcelAction = exportContractBillingSnapshotExcelAction;
        _createDraftContractBillingSnapshotAction = createDraftContractBillingSnapshotAction;
        _updateDraftContractBillingSnapshotAction = updateDraftContractBillingSnapshotAction;
        _patchContractBillingSnapshotCalendarDayAction = patchContractBillingSnapshotCalendarDayAction;
        _patchContractBillingSnapshotCalendarDayTripCountAction = patchContractBillingSnapshotCalendarDayTripCountAction;
        _patchContractBillingSnapshotCommercialTermsAction = patchContractBillingSnapshotCommercialTermsAction;
        _deleteContractBillingSnapshotCommercialTermsAction = deleteContractBillingSnapshotCommercialTermsAction;
        _getContractBillingSnapshotAction = getContractBillingSnapshotAction;
        _listContractBillingSnapshotsAction = listContractBillingSnapshotsAction;
        _deleteContractBillingSnapshotAction = deleteContractBillingSnapshotAction;
    }

    [HttpGet]
    [HasPermission(ListContractBillingSnapshotsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListContractBillingSnapshotsRequest request) =>
        Ok((await _listContractBillingSnapshotsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<ContractBillingSnapshotDetailDto, ContractBillingSnapshotResource>(
                ContractBillingSnapshotResource.FromDto));

    [HttpGet("{snapshotId:long}")]
    [HasPermission(GetContractBillingSnapshotRequest.Permission)]
    public async Task<ActionResult<ContractBillingSnapshotResource>> Get(long snapshotId) =>
        Ok(ContractBillingSnapshotResource.FromDto(
            await _getContractBillingSnapshotAction.GetByIdAsync(snapshotId)));

    [HttpGet("{snapshotId:long}/excel")]
    [HasPermission(ExportContractBillingSnapshotExcelRequest.Permission)]
    public async Task<IActionResult> ExportExcel(
        [FromRoute] ExportContractBillingSnapshotExcelRequest request)
    {
        ContractBillingSnapshotExcelFileResource resource = ContractBillingSnapshotExcelFileResource.FromDto(
            await _exportContractBillingSnapshotExcelAction.ExecuteAsync(request.FromRequest()));
        return File(
            resource.Content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            resource.FileName);
    }

    [HttpDelete("{snapshotId:long}")]
    [HasPermission(DeleteContractBillingSnapshotRequest.Permission)]
    public async Task<ActionResult> Delete([FromRoute] DeleteContractBillingSnapshotRequest request)
    {
        await _deleteContractBillingSnapshotAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "contract_billing_snapshot.deleted_successfully" });
    }

    [HttpPost("{contractId:long}")]
    [HasPermission(CreateDraftContractBillingSnapshotRequest.Permission)]
    public async Task<ActionResult<DraftContractBillingSnapshotResource>> CreateDraft(
        long contractId,
        [FromBody] CreateDraftContractBillingSnapshotRequest request)
    {
        var (snapshotId, name, calculation) = await _createDraftContractBillingSnapshotAction.ExecuteAsync(
            request.FromRequest(contractId));
        return Ok(DraftContractBillingSnapshotResource.FromResult(snapshotId, name, calculation));
    }

    [HttpPut("{contractId:long}/{snapshotId:long}")]
    [HasPermission(UpdateDraftContractBillingSnapshotRequest.Permission)]
    public async Task<ActionResult<DraftContractBillingSnapshotResource>> UpdateDraft(
        long contractId,
        long snapshotId,
        [FromBody] UpdateDraftContractBillingSnapshotRequest request)
    {
        var (persistedId, name, calculation) = await _updateDraftContractBillingSnapshotAction.ExecuteAsync(
            request.FromRequest(contractId, snapshotId));
        return Ok(DraftContractBillingSnapshotResource.FromResult(persistedId, name, calculation));
    }

    [HttpPatch("{snapshotId:long}/calendar-days/{calendarDayId:long}")]
    [HasPermission(PatchContractBillingSnapshotCalendarDayRequest.Permission)]
    public async Task<ActionResult<DraftContractBillingSnapshotResource>> PatchCalendarDay(
        long snapshotId,
        long calendarDayId,
        [FromBody] PatchContractBillingSnapshotCalendarDayRequest request)
    {
        var (id, name, calculation) = await _patchContractBillingSnapshotCalendarDayAction.ExecuteAsync(
            request.FromRequest(snapshotId, calendarDayId));
        return Ok(DraftContractBillingSnapshotResource.FromResult(id, name, calculation));
    }

    [HttpPatch("{snapshotId:long}/calendar-days/{calendarDayId:long}/trip-count")]
    [HasPermission(PatchContractBillingSnapshotCalendarDayTripCountRequest.Permission)]
    public async Task<ActionResult<DraftContractBillingSnapshotResource>> PatchCalendarDayTripCount(
        long snapshotId,
        long calendarDayId,
        [FromBody] PatchContractBillingSnapshotCalendarDayTripCountRequest request)
    {
        var (id, name, calculation) = await _patchContractBillingSnapshotCalendarDayTripCountAction.ExecuteAsync(
            request.FromRequest(snapshotId, calendarDayId));
        return Ok(DraftContractBillingSnapshotResource.FromResult(id, name, calculation));
    }

    [HttpPatch("{snapshotId:long}/commercial-terms")]
    [HasPermission(PatchContractBillingSnapshotCommercialTermsRequest.Permission)]
    public async Task<ActionResult<DraftContractBillingSnapshotResource>> PatchCommercialTerms(
        long snapshotId,
        [FromBody] PatchContractBillingSnapshotCommercialTermsRequest request)
    {
        var (id, name, calculation) = await _patchContractBillingSnapshotCommercialTermsAction.ExecuteAsync(
            request.FromRequest(snapshotId));
        return Ok(DraftContractBillingSnapshotResource.FromResult(id, name, calculation));
    }

    [HttpDelete("{snapshotId:long}/commercial-terms/{seatRootSlotId:long}")]
    [HasPermission(DeleteContractBillingSnapshotCommercialTermsRequest.Permission)]
    public async Task<ActionResult<DraftContractBillingSnapshotResource>> DeleteCommercialTerms(
        long snapshotId,
        long seatRootSlotId)
    {
        var (id, name, calculation) = await _deleteContractBillingSnapshotCommercialTermsAction.ExecuteAsync(
            new DeleteContractBillingSnapshotCommercialTermsRequest { SeatRootSlotId = seatRootSlotId }
                .FromRequest(snapshotId));
        return Ok(DraftContractBillingSnapshotResource.FromResult(id, name, calculation));
    }
}
