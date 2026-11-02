using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Collector.Trips;
using LoadlyAggregator.API.Resources.Collector.Trips;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Collector.Actions.Trips;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Collector.Trips;

[ApiController]
[Route("api/collector/trips")]
[Authorize]
public class CollectorTripsController : ControllerBase
{
    private readonly CollectTripDocumentsAction _collectTripDocumentsAction;
    private readonly ConfirmTripDocumentsAction _confirmTripDocumentsAction;
    private readonly BulkCollectTripsDocumentsAction _bulkCollectTripsDocumentsAction;
    private readonly BulkConfirmTripsDocumentsAction _bulkConfirmTripsDocumentsAction;
    private readonly CollectorListTripsAction _collectorListTripsAction;
    private readonly CollectorGetTripAction _collectorGetTripAction;
    private readonly IMapper _mapper;

    public CollectorTripsController(
        CollectTripDocumentsAction collectTripDocumentsAction,
        ConfirmTripDocumentsAction confirmTripDocumentsAction,
        BulkCollectTripsDocumentsAction bulkCollectTripsDocumentsAction,
        BulkConfirmTripsDocumentsAction bulkConfirmTripsDocumentsAction,
        CollectorListTripsAction collectorListTripsAction,
        CollectorGetTripAction collectorGetTripAction,
        IMapper mapper)
    {
        _collectTripDocumentsAction = collectTripDocumentsAction;
        _confirmTripDocumentsAction = confirmTripDocumentsAction;
        _bulkCollectTripsDocumentsAction = bulkCollectTripsDocumentsAction;
        _bulkConfirmTripsDocumentsAction = bulkConfirmTripsDocumentsAction;
        _collectorListTripsAction = collectorListTripsAction;
        _collectorGetTripAction = collectorGetTripAction;
        _mapper = mapper;
    }

    [HttpGet]
    [HasPermission(CollectorListTripsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] CollectorListTripsRequest request) =>
        Ok((await _collectorListTripsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<TripDto, CollectorTripResource>(dto => _mapper.Map<CollectorTripResource>(dto)));

    [HttpGet("{id:long}")]
    [HasPermission(CollectorGetTripRequest.Permission)]
    public async Task<ActionResult<CollectorTripResource>> Get(long id) =>
        Ok(_mapper.Map<CollectorTripResource>(await _collectorGetTripAction.ExecuteAsync(new CollectorGetTripRequest { Id = id }.FromRequest())));

    [HttpPost("{id:long}/documents/collect")]
    [HasPermission(CollectTripDocumentsRequest.Permission)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> CollectDocuments(long id, [FromForm] CollectTripDocumentsRequest request)
    {
        await _collectTripDocumentsAction.ExecuteAsync(request.FromRequest(id, ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "trips.documents_collected_successfully" });
    }

    [HttpPost("{id:long}/documents/confirm")]
    [HasPermission(ConfirmTripDocumentsRequest.Permission)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> ConfirmDocuments(long id, [FromForm] ConfirmTripDocumentsRequest request)
    {
        await _confirmTripDocumentsAction.ExecuteAsync(request.FromRequest(id, ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "trips.documents_confirmed_successfully" });
    }

    [HttpPost("documents/bulk-collect")]
    [HasPermission(BulkCollectTripsDocumentsRequest.Permission)]
    public async Task<ActionResult> BulkCollectDocuments([FromBody] BulkCollectTripsDocumentsRequest request)
    {
        await _bulkCollectTripsDocumentsAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "trips.bulk_collected_successfully" });
    }

    [HttpPost("documents/bulk-confirm")]
    [HasPermission(BulkConfirmTripsDocumentsRequest.Permission)]
    public async Task<ActionResult> BulkConfirmDocuments([FromBody] BulkConfirmTripsDocumentsRequest request)
    {
        await _bulkConfirmTripsDocumentsAction.ExecuteAsync(request.FromRequest(ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "trips.bulk_confirmed_successfully" });
    }
}
