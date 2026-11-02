using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Driver.Trips;
using LoadlyAggregator.API.Resources.Admin.Trips;
using LoadlyAggregator.API.Resources.Driver.Trips;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Driver.Actions.Trips;

namespace LoadlyAggregator.API.Controllers.Driver.Trips;

[ApiController]
[Route("api/driver/trips")]
[Authorize]
public class DriverTripsController : ControllerBase
{
    private readonly DriverStartLoadingCargoAction _driverStartLoadingCargoAction;
    private readonly DriverCompleteLoadingCargoAction _driverCompleteLoadingCargoAction;
    private readonly DriverStartUnloadingCargoAction _driverStartUnloadingCargoAction;
    private readonly DriverCompleteUnloadingCargoAction _driverCompleteUnloadingCargoAction;
    private readonly DriverStartTripAction _driverStartTripAction;
    private readonly DriverGetTripAction _driverGetTripAction;
    private readonly DriverListTripsAction _driverListTripsAction;
    private readonly DriverArriveAtWaypointAction _driverArriveAtWaypointAction;
    private readonly DriverDepartFromWaypointAction _driverDepartFromWaypointAction;
    private readonly DriverEntersCustomsAction _driverEntersCustomsAction;
    private readonly DriverExitsCustomsAction _driverExitsCustomsAction;
    private readonly DriverCompleteTripAction _driverCompleteTripAction;
    private readonly SuggestOdometerReadingAction _suggestOdometerReadingAction;
    private readonly DriverDownloadTripDocumentAction _downloadTripDocumentAction;
    private readonly DriverSyncRouteAction _driverSyncRouteAction;
    private readonly IMapper _mapper;

    public DriverTripsController(
        DriverStartLoadingCargoAction driverStartLoadingCargoAction,
        DriverCompleteLoadingCargoAction driverCompleteLoadingCargoAction,
        DriverStartUnloadingCargoAction driverStartUnloadingCargoAction,
        DriverCompleteUnloadingCargoAction driverCompleteUnloadingCargoAction,
        DriverStartTripAction driverStartTripAction,
        DriverGetTripAction driverGetTripAction,
        DriverListTripsAction driverListTripsAction,
        DriverArriveAtWaypointAction driverArriveAtWaypointAction,
        DriverDepartFromWaypointAction driverDepartFromWaypointAction,
        DriverEntersCustomsAction driverEntersCustomsAction,
        DriverExitsCustomsAction driverExitsCustomsAction,
        DriverCompleteTripAction driverCompleteTripAction,
        SuggestOdometerReadingAction suggestOdometerReadingAction,
        DriverDownloadTripDocumentAction downloadTripDocumentAction,
        DriverSyncRouteAction driverSyncRouteAction,
        IMapper mapper)
    {
        _driverStartLoadingCargoAction = driverStartLoadingCargoAction;
        _driverCompleteLoadingCargoAction = driverCompleteLoadingCargoAction;
        _driverStartUnloadingCargoAction = driverStartUnloadingCargoAction;
        _driverCompleteUnloadingCargoAction = driverCompleteUnloadingCargoAction;
        _driverStartTripAction = driverStartTripAction;
        _driverGetTripAction = driverGetTripAction;
        _driverListTripsAction = driverListTripsAction;
        _driverArriveAtWaypointAction = driverArriveAtWaypointAction;
        _driverDepartFromWaypointAction = driverDepartFromWaypointAction;
        _driverEntersCustomsAction = driverEntersCustomsAction;
        _driverExitsCustomsAction = driverExitsCustomsAction;
        _driverCompleteTripAction = driverCompleteTripAction;
        _suggestOdometerReadingAction = suggestOdometerReadingAction;
        _downloadTripDocumentAction = downloadTripDocumentAction;
        _driverSyncRouteAction = driverSyncRouteAction;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<DriverTripResource>> Get(long id)
    {
        var driverId = ClaimsHelper.GetUserId(User);
        return Ok(_mapper.Map<DriverTripResource>(await _driverGetTripAction.ExecuteAsync(new DriverGetTripRequest { Id = id }.FromRequest(driverId))));
    }

    [HttpGet("{id}/documents/download")]
    public async Task<IActionResult> DownloadDocument(long id, [FromQuery] string path)
    {
        var driverId = ClaimsHelper.GetUserId(User);
        return Redirect(await _downloadTripDocumentAction.ExecuteAsync(new DriverDownloadTripDocumentRequest { TripId = id, Path = path }.FromRequest(driverId)));
    }

    [HttpGet]
    public async Task<ActionResult> List([FromQuery] DriverListTripsRequest request)
    {
        var driverId = ClaimsHelper.GetUserId(User);
        return Ok((await _driverListTripsAction.ExecuteAsync(request.FromRequest(driverId)))
            .ToResourceCollection<TripDto, DriverTripResource>(dto => _mapper.Map<DriverTripResource>(dto)));
    }

    [HttpPost("{tripId:long}/waypoints/{waypointId:long}/loading/start")]
    public async Task<IActionResult> StartLoadingCargo(long tripId, long waypointId, [FromForm] DriverStartLoadingCargoRequest? request = null)
    {
        var dto = (request ?? new DriverStartLoadingCargoRequest()).FromRequest(tripId, waypointId, ClaimsHelper.GetUserId(User));
        dto.CurrentLocation = HttpContext.GetDriverLocation();

        await _driverStartLoadingCargoAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.loading_started_successfully" });
    }

    [HttpPost("{tripId:long}/waypoints/{waypointId:long}/loading/complete")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CompleteLoadingCargo(long tripId, long waypointId, [FromForm] DriverCompleteLoadingCargoRequest request)
    {
        var dto = request.FromRequest(tripId, waypointId, ClaimsHelper.GetUserId(User));
        dto.CurrentLocation = HttpContext.GetDriverLocation();

        await _driverCompleteLoadingCargoAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.loading_completed_successfully" });
    }

    [HttpPost("{tripId:long}/waypoints/{waypointId:long}/unloading/start")]
    public async Task<IActionResult> StartUnloadingCargo(long tripId, long waypointId, [FromForm] DriverStartUnloadingCargoRequest? request = null)
    {
        var dto = (request ?? new DriverStartUnloadingCargoRequest()).FromRequest(tripId, waypointId, ClaimsHelper.GetUserId(User));
        dto.CurrentLocation = HttpContext.GetDriverLocation();

        await _driverStartUnloadingCargoAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.unloading_started_successfully" });
    }

    [HttpPost("{tripId:long}/waypoints/{waypointId:long}/unloading/complete")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CompleteUnloadingCargo(long tripId, long waypointId, [FromForm] DriverCompleteUnloadingCargoRequest request)
    {
        var dto = request.FromRequest(tripId, waypointId, ClaimsHelper.GetUserId(User));
        dto.CurrentLocation = HttpContext.GetDriverLocation();

        await _driverCompleteUnloadingCargoAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.unloading_completed_successfully" });
    }

    [HttpPost("{tripId:long}/start")]
    public async Task<ActionResult<DriverStartTripResource>> StartTrip(long tripId, [FromBody] DriverStartTripRequest? request = null)
    {
        var dto = (request ?? new DriverStartTripRequest()).FromRequest(tripId, ClaimsHelper.GetUserId(User));
        dto.CurrentLocation = HttpContext.GetDriverLocation();

        var nextWaypoint = await _driverStartTripAction.ExecuteAsync(dto);
        return Ok(DriverStartTripResource.FromDto(nextWaypoint));
    }

    [HttpPost("{tripId:long}/waypoints/{waypointId:long}/arrive")]
    public async Task<IActionResult> ArriveAtWaypoint(long tripId, long waypointId, [FromForm] DriverArriveAtWaypointRequest request)
    {
        var dto = request.FromRequest(tripId, waypointId, ClaimsHelper.GetUserId(User));
        dto.CurrentLocation = HttpContext.GetDriverLocation();

        await _driverArriveAtWaypointAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.waypoint_arrived_successfully" });
    }

    [HttpPost("{tripId:long}/waypoints/{waypointId:long}/depart")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<DriverDepartFromWaypointResource>> DepartFromWaypoint(long tripId, long waypointId, [FromForm] DriverDepartFromWaypointRequest request)
    {
        var dto = request.FromRequest(tripId, waypointId, ClaimsHelper.GetUserId(User));
        dto.CurrentLocation = HttpContext.GetDriverLocation();

        var nextWaypoint = await _driverDepartFromWaypointAction.ExecuteAsync(dto);
        return Ok(DriverDepartFromWaypointResource.FromDto(nextWaypoint));
    }

    [HttpPost("{tripId:long}/customs/enter")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> EnterCustoms(long tripId, [FromForm] DriverEntersCustomsRequest? request = null)
    {
        var dto = (request ?? new DriverEntersCustomsRequest()).FromRequest(tripId, ClaimsHelper.GetUserId(User));
        dto.CurrentLocation = HttpContext.GetDriverLocation();

        await _driverEntersCustomsAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.customs_entered_successfully" });
    }

    [HttpPost("{tripId:long}/customs/exit")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> ExitCustoms(long tripId, [FromForm] DriverExitsCustomsRequest? request = null)
    {
        var dto = (request ?? new DriverExitsCustomsRequest()).FromRequest(tripId, ClaimsHelper.GetUserId(User));
        dto.CurrentLocation = HttpContext.GetDriverLocation();

        await _driverExitsCustomsAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.customs_exited_successfully" });
    }

    [HttpPost("{tripId:long}/odometer/suggest")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<SuggestOdometerReadingResource>> SuggestOdometerReading(
        long tripId,
        [FromForm] SuggestOdometerReadingRequest request)
    {
        var dto = request.FromRequest(
            tripId,
            ClaimsHelper.GetUserId(User),
            ClaimsHelper.GetUserTypeOptional(User) ?? "Driver",
            HttpContext.GetCorrelationId());

        var result = await _suggestOdometerReadingAction.ExecuteAsync(dto);
        return Ok(SuggestOdometerReadingResource.FromDto(result));
    }

    [HttpPost("{tripId:long}/complete")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CompleteTrip(long tripId, [FromForm] DriverCompleteTripRequest request)
    {
        var dto = request.FromRequest(tripId, ClaimsHelper.GetUserId(User));
        dto.CurrentLocation = HttpContext.GetDriverLocation();

        await _driverCompleteTripAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.completed_successfully" });
    }

    [HttpPost("{tripId:long}/route-sync")]
    public async Task<IActionResult> SyncRoute(long tripId, [FromBody] DriverSyncRouteRequest request)
    {
        await _driverSyncRouteAction.ExecuteAsync(request.FromRequest(tripId, ClaimsHelper.GetUserId(User)));
        return Ok(new { message = "trips.route_synced_successfully" });
    }
}
