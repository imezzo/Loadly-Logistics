using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Dashboard;
using LoadlyAggregator.API.Resources.Admin.Dashboard;
using LoadlyAggregator.Domain.Admin.Actions.Dashboard;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly GetTripsStatisticsAction _getTripsStatisticsAction;
    private readonly GetMonthlyTripsCostStatisticsAction _getMonthlyTripsCostStatisticsAction;
    private readonly CreateTargetAction _createTargetAction;
    private readonly UpdateTargetAction _updateTargetAction;
    private readonly DeleteTargetAction _deleteTargetAction;
    private readonly GetTargetAction _getTargetAction;
    private readonly ListTargetsAction _listTargetsAction;
    private readonly GetMonthlyTripsTargetsAction _getMonthlyTripsTargetsAction;
    private readonly GetMonthlyCostTargetsAction _getMonthlyCostTargetsAction;
    private readonly GetMonthlyTripsAvailableDriversAction _getMonthlyTripsAvailableDriversAction;
    private readonly GetWeeklyVehicleTripsComparisonAction _getWeeklyVehicleTripsComparisonAction;
    private readonly GetWeeklyDriverTripsComparisonAction _getWeeklyDriverTripsComparisonAction;
    private readonly GetMonthlyTargetsAction _getMonthlyTargetsAction;
    private readonly GetHomeDashboardAction _getHomeDashboardAction;
    private readonly GetSectorAggregationDashboardAction _getSectorAggregationDashboardAction;
    private readonly GetVehicleStatusStatisticsAction _getVehicleStatusStatisticsAction;
    private readonly ListDriverCompletedTripsByLifecycleActorAction _listDriverCompletedTripsByLifecycleActorAction;
    private readonly ExportDriverLifecycleTripsAction _exportDriverLifecycleTripsAction;

    public DashboardController(
        GetTripsStatisticsAction getTripsStatisticsAction,
        GetMonthlyTripsCostStatisticsAction getMonthlyTripsCostStatisticsAction,
        CreateTargetAction createTargetAction,
        UpdateTargetAction updateTargetAction,
        DeleteTargetAction deleteTargetAction,
        GetTargetAction getTargetAction,
        ListTargetsAction listTargetsAction,
        GetMonthlyTripsTargetsAction getMonthlyTripsTargetsAction,
        GetMonthlyCostTargetsAction getMonthlyCostTargetsAction,
        GetMonthlyTripsAvailableDriversAction getMonthlyTripsAvailableDriversAction,
        GetWeeklyVehicleTripsComparisonAction getWeeklyVehicleTripsComparisonAction,
        GetWeeklyDriverTripsComparisonAction getWeeklyDriverTripsComparisonAction,
        GetMonthlyTargetsAction getMonthlyTargetsAction,
        GetHomeDashboardAction getHomeDashboardAction,
        GetSectorAggregationDashboardAction getSectorAggregationDashboardAction,
        GetVehicleStatusStatisticsAction getVehicleStatusStatisticsAction,
        ListDriverCompletedTripsByLifecycleActorAction listDriverCompletedTripsByLifecycleActorAction,
        ExportDriverLifecycleTripsAction exportDriverLifecycleTripsAction)
    {
        _getTripsStatisticsAction = getTripsStatisticsAction;
        _getMonthlyTripsCostStatisticsAction = getMonthlyTripsCostStatisticsAction;
        _createTargetAction = createTargetAction;
        _updateTargetAction = updateTargetAction;
        _deleteTargetAction = deleteTargetAction;
        _getTargetAction = getTargetAction;
        _listTargetsAction = listTargetsAction;
        _getMonthlyTripsTargetsAction = getMonthlyTripsTargetsAction;
        _getMonthlyCostTargetsAction = getMonthlyCostTargetsAction;
        _getMonthlyTripsAvailableDriversAction = getMonthlyTripsAvailableDriversAction;
        _getWeeklyVehicleTripsComparisonAction = getWeeklyVehicleTripsComparisonAction;
        _getWeeklyDriverTripsComparisonAction = getWeeklyDriverTripsComparisonAction;
        _getMonthlyTargetsAction = getMonthlyTargetsAction;
        _getHomeDashboardAction = getHomeDashboardAction;
        _getSectorAggregationDashboardAction = getSectorAggregationDashboardAction;
        _getVehicleStatusStatisticsAction = getVehicleStatusStatisticsAction;
        _listDriverCompletedTripsByLifecycleActorAction = listDriverCompletedTripsByLifecycleActorAction;
        _exportDriverLifecycleTripsAction = exportDriverLifecycleTripsAction;
    }

    [HttpGet("trips")]
    [HasPermission(GetDashboardTripsRequest.Permission)]
    public async Task<ActionResult<TripsStatisticsResource>> GetTrips([FromQuery] GetDashboardTripsRequest request) =>
        Ok(TripsStatisticsResource.FromDto(await _getTripsStatisticsAction.ExecuteAsync(request.FromRequest())));

    [HttpGet("monthly-trips-cost")]
    [HasPermission(GetMonthlyTripsCostRequest.Permission)]
    public async Task<ActionResult> GetMonthlyTripsCost([FromQuery] GetMonthlyTripsCostRequest request) =>
        Ok((await _getMonthlyTripsCostStatisticsAction.ExecuteAsync(request.FromRequest()))
            .Select(MonthlyTripCostResource.FromDto));

    [HttpGet("monthly-trips-available-drivers")]
    [HasPermission(GetMonthlyTripsAvailableDriversRequest.Permission)]
    public async Task<ActionResult> GetMonthlyTripsAvailableDrivers([FromQuery] GetMonthlyTripsAvailableDriversRequest request) =>
        Ok((await _getMonthlyTripsAvailableDriversAction.ExecuteAsync(request.FromRequest()))
            .Select(MonthlyTripsAvailableDriversResource.FromDto));

    [HttpGet("targets")]
    [HasPermission(ListTargetsRequest.Permission)]
    public async Task<ActionResult> ListTargets([FromQuery] ListTargetsRequest request) =>
        Ok((await _listTargetsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<Domain.Admin.DTOs.Dashboard.TargetDto, TargetResource>(dto => TargetResource.FromDto(dto)));

    [HttpGet("targets/{id}")]
    [HasPermission(GetTargetRequest.Permission)]
    public async Task<ActionResult<TargetResource>> GetTarget(long id) =>
        Ok(TargetResource.FromDto(await _getTargetAction.ExecuteAsync(new GetTargetRequest { Id = id }.FromRequest())));

    [HttpPost("targets")]
    [HasPermission(CreateTargetRequest.Permission)]
    public async Task<ActionResult> CreateTarget([FromBody] CreateTargetRequest request)
    {
        await _createTargetAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "targets.created_successfully" });
    }

    [HttpPut("targets/{id}")]
    [HasPermission(UpdateTargetRequest.Permission)]
    public async Task<ActionResult> UpdateTarget(long id, [FromBody] UpdateTargetRequest request)
    {
        request.Id = id;
        await _updateTargetAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "targets.updated_successfully" });
    }

    [HttpDelete("targets/{id}")]
    [HasPermission(DeleteTargetRequest.Permission)]
    public async Task<ActionResult> DeleteTarget(long id)
    {
        await _deleteTargetAction.ExecuteAsync(new DeleteTargetRequest { Id = id }.FromRequest());
        return Ok(new { message = "targets.deleted_successfully" });
    }

    [HttpGet("monthly-targets-achievement")]
    [HasPermission(GetMonthlyTargetsRequest.Permission)]
    public async Task<ActionResult> GetMonthlyTargetsAchievement([FromQuery] GetMonthlyTargetsRequest request) =>
        Ok((await _getMonthlyTargetsAction.ExecuteAsync(request.FromRequest()))
            .Select(MonthlyTargetResource.FromDto));

    [HttpGet("monthly-trips-targets")]
    [HasPermission(GetMonthlyTripsTargetsRequest.Permission)]
    public async Task<ActionResult> GetMonthlyTripsTargets([FromQuery] GetMonthlyTripsTargetsRequest request) =>
        Ok((await _getMonthlyTripsTargetsAction.ExecuteAsync(request.FromRequest()))
            .Select(MonthlyTargetResource.FromDto));

    [HttpGet("monthly-cost-targets")]
    [HasPermission(GetMonthlyCostTargetsRequest.Permission)]
    public async Task<ActionResult> GetMonthlyCostTargets([FromQuery] GetMonthlyCostTargetsRequest request) =>
        Ok((await _getMonthlyCostTargetsAction.ExecuteAsync(request.FromRequest()))
            .Select(MonthlyTargetResource.FromDto));

    [HttpGet("weekly-vehicle-trips-comparison")]
    [HasPermission(GetWeeklyVehicleTripsComparisonRequest.Permission)]
    public async Task<ActionResult<WeeklyVehicleTripsComparisonResource>> GetWeeklyVehicleTripsComparison() =>
        Ok(WeeklyVehicleTripsComparisonResource.FromDto(await _getWeeklyVehicleTripsComparisonAction.ExecuteAsync()));

    [HttpGet("weekly-driver-trips-comparison")]
    [HasPermission(GetWeeklyDriverTripsComparisonRequest.Permission)]
    public async Task<ActionResult<WeeklyDriverTripsComparisonResource>> GetWeeklyDriverTripsComparison() =>
        Ok(WeeklyDriverTripsComparisonResource.FromDto(await _getWeeklyDriverTripsComparisonAction.ExecuteAsync()));

    // Home Dashboard Section
    [HttpGet("home")]
    [HasPermission(GetHomeDashboardRequest.Permission)]
    public async Task<ActionResult<HomeDashboardResource>> GetHome() =>
        Ok(HomeDashboardResource.FromDto(await _getHomeDashboardAction.ExecuteAsync()));

    [HttpGet("sectors")]
    [HasPermission(GetSectorAggregationDashboardRequest.Permission)]
    public async Task<ActionResult<SectorAggregationDashboardResource>> GetSectors(
        [FromQuery] GetSectorAggregationDashboardRequest request) =>
        Ok(SectorAggregationDashboardResource.FromDto(
            await _getSectorAggregationDashboardAction.ExecuteAsync(request.FromRequest())));

    [HttpGet("vehicle-status")]
    [HasPermission(GetVehicleStatusStatisticsRequest.Permission)]
    public async Task<ActionResult<VehicleStatusStatisticsResource>> GetVehicleStatus() =>
        Ok(VehicleStatusStatisticsResource.FromDto(await _getVehicleStatusStatisticsAction.ExecuteAsync()));

    [HttpGet("driver-completed-trips-by-lifecycle-actor")]
    [HasPermission(ListDriverCompletedTripsByLifecycleActorRequest.Permission)]
    public async Task<ActionResult> ListDriverCompletedTripsByLifecycleActor([FromQuery] ListDriverCompletedTripsByLifecycleActorRequest request) =>
        Ok((await _listDriverCompletedTripsByLifecycleActorAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<Domain.Admin.DTOs.Dashboard.DriverCompletedTripsByLifecycleActorDto, DriverCompletedTripsByLifecycleActorResource>(
                DriverCompletedTripsByLifecycleActorResource.FromDto));

    [HttpGet("driver-lifecycle-trips-export")]
    [HasPermission(ExportDriverLifecycleTripsRequest.Permission)]
    public async Task<IActionResult> ExportDriverLifecycleTrips(
        [FromQuery] ExportDriverLifecycleTripsRequest request)
    {
        DriverLifecycleTripsFileResource resource = DriverLifecycleTripsFileResource.FromDto(
            await _exportDriverLifecycleTripsAction.ExecuteAsync(request.FromRequest()));
        return File(resource.Content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", resource.FileName);
    }
}
