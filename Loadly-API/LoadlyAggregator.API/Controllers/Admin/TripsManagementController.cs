using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.TripsManagement;
using LoadlyAggregator.API.Resources.Admin.TripsManagement;
using LoadlyAggregator.Domain.Admin.Actions.TripsManagement;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/trips-management")]
[Authorize]
public class TripsManagementController : ControllerBase
{
    private readonly StartLoadingCargoAction _startLoadingCargoAction;
    private readonly CompleteLoadingCargoAction _completeLoadingCargoAction;
    private readonly StartUnloadingCargoAction _startUnloadingCargoAction;
    private readonly CompleteUnloadingCargoAction _completeUnloadingCargoAction;
    private readonly StartTripAction _startTripAction;
    private readonly ArriveAtWaypointAction _arriveAtWaypointAction;
    private readonly DepartFromWaypointAction _departFromWaypointAction;
    private readonly ArriveAndStartLoadingAction _arriveAndStartLoadingAction;
    private readonly ArriveAndStartUnloadingAction _arriveAndStartUnloadingAction;
    private readonly CompleteLoadingAndDepartAction _completeLoadingAndDepartAction;
    private readonly CompleteUnloadingAndDepartAction _completeUnloadingAndDepartAction;
    private readonly EntersCustomsAction _entersCustomsAction;
    private readonly ExitsCustomsAction _exitsCustomsAction;
    private readonly CompleteTripAction _completeTripAction;

    public TripsManagementController(
        StartLoadingCargoAction startLoadingCargoAction,
        CompleteLoadingCargoAction completeLoadingCargoAction,
        StartUnloadingCargoAction startUnloadingCargoAction,
        CompleteUnloadingCargoAction completeUnloadingCargoAction,
        StartTripAction startTripAction,
        ArriveAtWaypointAction arriveAtWaypointAction,
        DepartFromWaypointAction departFromWaypointAction,
        ArriveAndStartLoadingAction arriveAndStartLoadingAction,
        ArriveAndStartUnloadingAction arriveAndStartUnloadingAction,
        CompleteLoadingAndDepartAction completeLoadingAndDepartAction,
        CompleteUnloadingAndDepartAction completeUnloadingAndDepartAction,
        EntersCustomsAction entersCustomsAction,
        ExitsCustomsAction exitsCustomsAction,
        CompleteTripAction completeTripAction)
    {
        _startLoadingCargoAction = startLoadingCargoAction;
        _completeLoadingCargoAction = completeLoadingCargoAction;
        _startUnloadingCargoAction = startUnloadingCargoAction;
        _completeUnloadingCargoAction = completeUnloadingCargoAction;
        _startTripAction = startTripAction;
        _arriveAtWaypointAction = arriveAtWaypointAction;
        _departFromWaypointAction = departFromWaypointAction;
        _arriveAndStartLoadingAction = arriveAndStartLoadingAction;
        _arriveAndStartUnloadingAction = arriveAndStartUnloadingAction;
        _completeLoadingAndDepartAction = completeLoadingAndDepartAction;
        _completeUnloadingAndDepartAction = completeUnloadingAndDepartAction;
        _entersCustomsAction = entersCustomsAction;
        _exitsCustomsAction = exitsCustomsAction;
        _completeTripAction = completeTripAction;
    }

    [HttpPost("{tripId:long}/waypoints/{waypointId:long}/loading/start")]
    [HasPermission(StartLoadingCargoRequest.Permission)]
    public async Task<IActionResult> StartLoadingCargo(long tripId, long waypointId, [FromForm] StartLoadingCargoRequest request)
    {
        var dto = request.FromRequest(tripId, waypointId);

        await _startLoadingCargoAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.loading_started_successfully" });
    }

    [HttpPost("{tripId:long}/waypoints/{waypointId:long}/loading/complete")]
    [Consumes("multipart/form-data")]
    [HasPermission(CompleteLoadingCargoRequest.Permission)]
    public async Task<IActionResult> CompleteLoadingCargo(long tripId, long waypointId, [FromForm] CompleteLoadingCargoRequest request)
    {
        var dto = request.FromRequest(tripId, waypointId);

        await _completeLoadingCargoAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.loading_completed_successfully" });
    }

    [HttpPost("{tripId:long}/waypoints/{waypointId:long}/unloading/start")]
    [HasPermission(StartUnloadingCargoRequest.Permission)]
    public async Task<IActionResult> StartUnloadingCargo(long tripId, long waypointId, [FromForm] StartUnloadingCargoRequest request)
    {
        var dto = request.FromRequest(tripId, waypointId);

        await _startUnloadingCargoAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.unloading_started_successfully" });
    }

    [HttpPost("{tripId:long}/waypoints/{waypointId:long}/unloading/complete")]
    [Consumes("multipart/form-data")]
    [HasPermission(CompleteUnloadingCargoRequest.Permission)]
    public async Task<IActionResult> CompleteUnloadingCargo(long tripId, long waypointId, [FromForm] CompleteUnloadingCargoRequest request)
    {
        var dto = request.FromRequest(tripId, waypointId);

        await _completeUnloadingCargoAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.unloading_completed_successfully" });
    }

    [HttpPost("{tripId:long}/start")]
    [HasPermission(StartTripRequest.Permission)]
    public async Task<ActionResult<StartTripResource>> StartTrip(long tripId, [FromBody] StartTripRequest request)
    {
        var dto = request.FromRequest(tripId);

        var nextWaypoint = await _startTripAction.ExecuteAsync(dto);
        return Ok(StartTripResource.FromDto(nextWaypoint));
    }

    [HttpPost("{tripId:long}/waypoints/{waypointId:long}/arrive")]
    [HasPermission(ArriveAtWaypointRequest.Permission)]
    public async Task<IActionResult> ArriveAtWaypoint(long tripId, long waypointId, [FromForm] ArriveAtWaypointRequest request)
    {
        var dto = request.FromRequest(tripId, waypointId);

        await _arriveAtWaypointAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.waypoint_arrived_successfully" });
    }

    [HttpPost("{tripId:long}/waypoints/{waypointId:long}/depart")]
    [Consumes("multipart/form-data")]
    [HasPermission(DepartFromWaypointRequest.Permission)]
    public async Task<ActionResult<DepartFromWaypointResource>> DepartFromWaypoint(long tripId, long waypointId, [FromForm] DepartFromWaypointRequest request)
    {
        var dto = request.FromRequest(tripId, waypointId);

        var nextWaypoint = await _departFromWaypointAction.ExecuteAsync(dto);
        return Ok(DepartFromWaypointResource.FromDto(nextWaypoint));
    }

    [HttpPost("{tripId:long}/waypoints/{waypointId:long}/loading/arrive-and-start")]
    [HasPermission(ArriveAndStartLoadingRequest.Permission)]
    public async Task<IActionResult> ArriveAndStartLoading(long tripId, long waypointId, [FromForm] ArriveAndStartLoadingRequest request)
    {
        var dto = request.FromRequest(tripId, waypointId);

        await _arriveAndStartLoadingAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.loading_started_successfully" });
    }

    [HttpPost("{tripId:long}/waypoints/{waypointId:long}/loading/complete-and-depart")]
    [Consumes("multipart/form-data")]
    [HasPermission(CompleteLoadingAndDepartRequest.Permission)]
    public async Task<ActionResult<DepartFromWaypointResource>> CompleteLoadingAndDepart(long tripId, long waypointId, [FromForm] CompleteLoadingAndDepartRequest request)
    {
        var dto = request.FromRequest(tripId, waypointId);

        var nextWaypoint = await _completeLoadingAndDepartAction.ExecuteAsync(dto);
        return Ok(DepartFromWaypointResource.FromDto(nextWaypoint));
    }

    [HttpPost("{tripId:long}/waypoints/{waypointId:long}/unloading/arrive-and-start")]
    [HasPermission(ArriveAndStartUnloadingRequest.Permission)]
    public async Task<IActionResult> ArriveAndStartUnloading(long tripId, long waypointId, [FromForm] ArriveAndStartUnloadingRequest request)
    {
        var dto = request.FromRequest(tripId, waypointId);

        await _arriveAndStartUnloadingAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.unloading_started_successfully" });
    }

    [HttpPost("{tripId:long}/waypoints/{waypointId:long}/unloading/complete-and-depart")]
    [Consumes("multipart/form-data")]
    [HasPermission(CompleteUnloadingAndDepartRequest.Permission)]
    public async Task<ActionResult<DepartFromWaypointResource>> CompleteUnloadingAndDepart(long tripId, long waypointId, [FromForm] CompleteUnloadingAndDepartRequest request)
    {
        var dto = request.FromRequest(tripId, waypointId);

        var nextWaypoint = await _completeUnloadingAndDepartAction.ExecuteAsync(dto);
        return Ok(DepartFromWaypointResource.FromDto(nextWaypoint));
    }

    [HttpPost("{tripId:long}/customs/enter")]
    [Consumes("multipart/form-data")]
    [HasPermission(EntersCustomsRequest.Permission)]
    public async Task<IActionResult> EnterCustoms(long tripId, [FromForm] EntersCustomsRequest request)
    {
        var dto = request.FromRequest(tripId);

        await _entersCustomsAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.customs_entered_successfully" });
    }

    [HttpPost("{tripId:long}/customs/exit")]
    [Consumes("multipart/form-data")]
    [HasPermission(ExitsCustomsRequest.Permission)]
    public async Task<IActionResult> ExitCustoms(long tripId, [FromForm] ExitsCustomsRequest request)
    {
        var dto = request.FromRequest(tripId);

        await _exitsCustomsAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.customs_exited_successfully" });
    }

    [HttpPost("{tripId:long}/complete")]
    [Consumes("multipart/form-data")]
    [HasPermission(CompleteTripRequest.Permission)]
    public async Task<IActionResult> CompleteTrip(long tripId, [FromForm] CompleteTripRequest request)
    {
        var dto = request.FromRequest(tripId);

        await _completeTripAction.ExecuteAsync(dto);
        return Ok(new { message = "trips.completed_successfully" });
    }
}
