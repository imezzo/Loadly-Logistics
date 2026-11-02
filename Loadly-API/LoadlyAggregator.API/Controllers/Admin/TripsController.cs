using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Trips;
using LoadlyAggregator.API.Resources.Admin.Trips;
using LoadlyAggregator.Domain.Admin.Actions.Trips;
using LoadlyAggregator.Domain.Admin.Actions.Trips.Responses;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Actions;
using LoadlyAggregator.Domain.Common.DTOs;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/trips")]
[Authorize]
public class TripsController : ControllerBase
{
    private readonly CreateTripAction _createTripAction;
    private readonly CreateHistoricalTripAction _createHistoricalTripAction;
    private readonly BulkCreateTripsAction _bulkCreateTripsAction;
    private readonly BulkImportTripsAction _bulkImportTripsAction;
    private readonly GetTripAction _getTripAction;
    private readonly GetTripRouteAction _getTripRouteAction;
    private readonly ListTripsAction _listTripsAction;
    private readonly UpdateTripAction _updateTripAction;
    private readonly UpdateTripChargesAction _updateTripChargesAction;
    private readonly CalculateAndPersistTripChargeAction _calculateAndPersistTripChargeAction;
    private readonly AuditTripAction _auditTripAction;
    private readonly BulkUpdateTripChargesInvoiceTypeAction _bulkUpdateTripChargesInvoiceTypeAction;
    private readonly SummarizeTripChargesByInvoiceTypeAction _summarizeTripChargesByInvoiceTypeAction;
    private readonly SetTripOverNightCostBearerAction _setTripOverNightCostBearerAction;
    private readonly SetTripDispatchingModeAction _setTripDispatchingModeAction;
    private readonly UpdateTripDispatcherNotesAction _updateTripDispatcherNotesAction;
    private readonly CancelTripAction _cancelTripAction;
    private readonly CompleteTripAction _completeTripAction;
    private readonly DeleteTripAction _deleteTripAction;
    private readonly DownloadTripDocumentAction _downloadTripDocumentAction;
    private readonly TrackTripsAction _trackTripsAction;
    private readonly PushTripToContractorsAction _pushTripToContractorsAction;
    private readonly ApproveTripBookingRequestAction _approveTripBookingRequestAction;
    private readonly ILanguageService _languageService;
    private readonly IMapper _mapper;
    private readonly ExportTripsExcelAction _exportTripsExcelAction;
    private readonly PreviewTripContractorPriceAction _previewTripContractorPriceAction;

    public TripsController(
        CreateTripAction createTripAction,
        CreateHistoricalTripAction createHistoricalTripAction,
        BulkCreateTripsAction bulkCreateTripsAction,
        BulkImportTripsAction bulkImportTripsAction,
        GetTripAction getTripAction,
        GetTripRouteAction getTripRouteAction,
        ListTripsAction listTripsAction,
        UpdateTripAction updateTripAction,
        UpdateTripChargesAction updateTripChargesAction,
        CalculateAndPersistTripChargeAction calculateAndPersistTripChargeAction,
        AuditTripAction auditTripAction,
        BulkUpdateTripChargesInvoiceTypeAction bulkUpdateTripChargesInvoiceTypeAction,
        SummarizeTripChargesByInvoiceTypeAction summarizeTripChargesByInvoiceTypeAction,
        SetTripOverNightCostBearerAction setTripOverNightCostBearerAction,
        SetTripDispatchingModeAction setTripDispatchingModeAction,
        UpdateTripDispatcherNotesAction updateTripDispatcherNotesAction,
        CancelTripAction cancelTripAction,
        CompleteTripAction completeTripAction,
        DeleteTripAction deleteTripAction,
        DownloadTripDocumentAction downloadTripDocumentAction,
        TrackTripsAction trackTripsAction,
        PushTripToContractorsAction pushTripToContractorsAction,
        ApproveTripBookingRequestAction approveTripBookingRequestAction,
        ILanguageService languageService,
        IMapper mapper,
        ExportTripsExcelAction exportTripsExcelAction,
        PreviewTripContractorPriceAction previewTripContractorPriceAction)
    {
        _createTripAction = createTripAction;
        _createHistoricalTripAction = createHistoricalTripAction;
        _bulkCreateTripsAction = bulkCreateTripsAction;
        _bulkImportTripsAction = bulkImportTripsAction;
        _getTripAction = getTripAction;
        _getTripRouteAction = getTripRouteAction;
        _listTripsAction = listTripsAction;
        _updateTripAction = updateTripAction;
        _updateTripChargesAction = updateTripChargesAction;
        _calculateAndPersistTripChargeAction = calculateAndPersistTripChargeAction;
        _auditTripAction = auditTripAction;
        _bulkUpdateTripChargesInvoiceTypeAction = bulkUpdateTripChargesInvoiceTypeAction;
        _summarizeTripChargesByInvoiceTypeAction = summarizeTripChargesByInvoiceTypeAction;
        _setTripOverNightCostBearerAction = setTripOverNightCostBearerAction;
        _setTripDispatchingModeAction = setTripDispatchingModeAction;
        _updateTripDispatcherNotesAction = updateTripDispatcherNotesAction;
        _cancelTripAction = cancelTripAction;
        _completeTripAction = completeTripAction;
        _deleteTripAction = deleteTripAction;
        _downloadTripDocumentAction = downloadTripDocumentAction;
        _trackTripsAction = trackTripsAction;
        _pushTripToContractorsAction = pushTripToContractorsAction;
        _approveTripBookingRequestAction = approveTripBookingRequestAction;
        _languageService = languageService;
        _mapper = mapper;
        _exportTripsExcelAction = exportTripsExcelAction;
        _previewTripContractorPriceAction = previewTripContractorPriceAction;
    }

    [HttpGet("contractor-price-preview")]
    [HasPermission(PreviewTripContractorPriceRequest.Permission)]
    public async Task<ActionResult<PreviewTripContractorPriceResource>> PreviewContractorPrice(
        [FromQuery] PreviewTripContractorPriceRequest request) =>
        Ok(PreviewTripContractorPriceResource.FromDto(
            await _previewTripContractorPriceAction.ExecuteAsync(request.FromRequest())));

    [HttpGet("track")]
    [HasPermission(TrackTripsRequest.Permission)]
    public async Task<ActionResult> Track([FromQuery] TrackTripsRequest request)
    {
        var lang = _languageService.GetCurrentLanguage();
        return Ok((await _trackTripsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<TripTrackItemDto, TripTrackItemResource>(dto => TripTrackItemResource.FromDto(dto, lang)));
    }

    [HttpGet("excel")]
    [HasPermission(ExportTripsExcelRequest.Permission)]
    public async Task<IActionResult> ExportExcel([FromQuery] ExportTripsExcelRequest request)
    {
        TripsExcelFileResource resource = TripsExcelFileResource.FromDto(
            await _exportTripsExcelAction.ExecuteAsync(request.FromRequest()));
        return File(resource.Content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", resource.FileName);
    }

    [HttpPost]
    [HasPermission(CreateTripRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateTripRequest request)
    {
        await _createTripAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "trips.created_successfully" });
    }

    [HttpPost("historical")]
    [HasPermission(CreateHistoricalTripRequest.Permission)]
    public async Task<ActionResult> CreateHistorical([FromBody] CreateHistoricalTripRequest request)
    {
        await _createHistoricalTripAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "trips.created_successfully" });
    }

    [HttpPost("bulk")]
    [HasPermission(BulkCreateTripsRequest.Permission)]
    public async Task<ActionResult<BulkCreateTripsResponse>> BulkCreate([FromBody] BulkCreateTripsRequest request)
    {
        return Ok(await _bulkCreateTripsAction.ExecuteAsync(request.FromRequest()));
    }

    [HttpPost("import")]
    [HasPermission(ImportTripsRequest.Permission)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<BulkImportTripsResultResource>> Import([FromForm] ImportTripsRequest request)
    {
        var result = await _bulkImportTripsAction.ExecuteAsync(request.FromRequest());
        return Ok(BulkImportTripsResultResource.FromResult(result));
    }

    [HttpPost("{id:long}/push-to-contractors")]
    [HasPermission(PushTripToContractorsRequest.Permission)]
    public async Task<ActionResult> PushToContractors(long id, [FromBody] PushTripToContractorsRequest request)
    {
        await _pushTripToContractorsAction.ExecuteAsync(request.FromRequest(id));
        return Ok(new { message = "trips.pushed_to_contractors_successfully" });
    }

    [HttpPost("{id:long}/booking-requests/{requestId:long}/approve")]
    [HasPermission(ApproveTripBookingRequestRequest.Permission)]
    public async Task<ActionResult> ApproveBookingRequest(long id, long requestId)
    {
        await _approveTripBookingRequestAction.ExecuteAsync(
            new ApproveTripBookingRequestRequest().FromRequest(id, requestId));
        return Ok(new { message = "trips.booking_request_approved_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetTripRequest.Permission)]
    public async Task<ActionResult<TripResource>> Get(long id) =>
        Ok(_mapper.Map<TripResource>(await _getTripAction.ExecuteAsync(new GetTripRequest { Id = id }.FromRequest())));

    [HttpGet("{id:long}/route")]
    [HasPermission(GetTripRouteRequest.Permission)]
    public async Task<ActionResult<TripRouteResource>> GetRoute(long id) =>
        Ok(TripRouteResource.FromDto(
            await _getTripRouteAction.ExecuteAsync(new GetTripRouteRequest { TripId = id }.FromRequest())));

    [HttpGet("{id}/documents/download")]
    [HasPermission(DownloadTripDocumentRequest.Permission)]
    public async Task<IActionResult> DownloadDocument(long id, [FromQuery] string path) =>
        Redirect(await _downloadTripDocumentAction.ExecuteAsync(new DownloadTripDocumentRequest { TripId = id, Path = path }.FromRequest()));

    [HttpGet]
    [HasPermission(ListTripsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListTripsRequest request) =>
        Ok((await _listTripsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<TripDto, TripResource>(dto => _mapper.Map<TripResource>(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateTripRequest.Permission)]
    public async Task<ActionResult> Update(long id, [FromBody] UpdateTripRequest request)
    {
        await _updateTripAction.ExecuteAsync(request.FromRequest(id));
        return Ok(new { message = "trips.updated_successfully" });
    }

    [HttpPut("{id:long}/charges")]
    [HasPermission(UpdateTripChargesRequest.Permission)]
    public async Task<ActionResult> UpdateCharges(long id, [FromBody] UpdateTripChargesRequest request)
    {
        await _updateTripChargesAction.ExecuteAsync(request.FromRequest(id));
        return Ok(new { message = "trips.charges_updated_successfully" });
    }

    [HttpPost("{id:long}/recalculate-charges")]
    [HasPermission(UpdateTripChargesRequest.Permission)]
    public async Task<ActionResult> RecalculateCharges(long id)
    {
        await _calculateAndPersistTripChargeAction.ExecuteAsync(new CalculateAndPersistTripChargeDto
        {
            TripId = id
        });
        return Ok(new { message = "trips.charges_recalculated_successfully" });
    }

    [HttpPost("{id:long}/audit")]
    [HasPermission(AuditTripRequest.Permission)]
    public async Task<ActionResult> Audit(long id, [FromBody] AuditTripRequest request)
    {
        await _auditTripAction.ExecuteAsync(request.FromRequest(id));
        return Ok(new { message = "trips.audited_successfully" });
    }

    [HttpPost("charges/invoice-summary")]
    [HasPermission(SummarizeTripChargesByInvoiceTypeRequest.Permission)]
    public async Task<ActionResult<TripChargesInvoiceSummaryResource>> SummarizeChargesByInvoiceType(
        [FromBody] SummarizeTripChargesByInvoiceTypeRequest request) =>
        Ok(TripChargesInvoiceSummaryResource.FromDto(
            await _summarizeTripChargesByInvoiceTypeAction.ExecuteAsync(request.FromRequest())));

    [HttpPatch("charges/invoice-type")]
    [HasPermission(BulkUpdateTripChargesInvoiceTypeRequest.Permission)]
    public async Task<ActionResult> BulkUpdateChargesInvoiceType(
        [FromBody] BulkUpdateTripChargesInvoiceTypeRequest request)
    {
        await _bulkUpdateTripChargesInvoiceTypeAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "trips.charges_invoice_type_updated_successfully" });
    }

    [HttpPatch("{id:long}/overnight-cost-bearer")]
    [HasPermission(SetTripOverNightCostBearerRequest.Permission)]
    public async Task<ActionResult> SetOverNightCostBearer(long id, [FromBody] SetTripOverNightCostBearerRequest request)
    {
        await _setTripOverNightCostBearerAction.ExecuteAsync(request.FromRequest(id));
        return Ok(new { message = "trips.over_night_cost_bearer_updated_successfully" });
    }

    [HttpPatch("{id:long}/set-dispatching-mode")]
    [HasPermission(SetTripDispatchingModeRequest.Permission)]
    public async Task<ActionResult> SetDispatchingMode(long id, [FromBody] SetTripDispatchingModeRequest request)
    {
        await _setTripDispatchingModeAction.ExecuteAsync(request.FromRequest(id));
        return Ok(new { message = "trips.dispatching_mode_updated_successfully" });
    }

    [HttpPatch("{id:long}/dispatcher-notes")]
    [HasPermission(UpdateTripDispatcherNotesRequest.Permission)]
    public async Task<ActionResult> UpdateDispatcherNotes(long id, [FromBody] UpdateTripDispatcherNotesRequest request)
    {
        await _updateTripDispatcherNotesAction.ExecuteAsync(request.FromRequest(id));
        return Ok(new { message = "trips.dispatcher_notes_updated_successfully" });
    }

    [HttpPost("{id:long}/cancel")]
    [HasPermission(CancelTripRequest.Permission)]
    public async Task<ActionResult> Cancel(long id, [FromBody] CancelTripRequest request)
    {
        await _cancelTripAction.ExecuteAsync(request.FromRequest(id));
        return Ok(new { message = "trips.cancelled_successfully" });
    }

    [HttpPost("{id:long}/complete")]
    [HasPermission(CompleteTripRequest.Permission)]
    public async Task<ActionResult> Complete(long id)
    {
        await _completeTripAction.ExecuteAsync(new CompleteTripRequest { Id = id }.FromRequest());
        return Ok(new { message = "trips.completed_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteTripRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteTripAction.ExecuteAsync(new DeleteTripRequest { Id = id }.FromRequest());
        return Ok(new { message = "trips.deleted_successfully" });
    }
}
