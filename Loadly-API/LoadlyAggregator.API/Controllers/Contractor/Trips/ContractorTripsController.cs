using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Contractor.Trips;
using LoadlyAggregator.API.Resources.Contractor.Trips;
using LoadlyAggregator.Domain.Contractor.Actions.Trips;
using LoadlyAggregator.Domain.Contractor.DTOs.Trips;

namespace LoadlyAggregator.API.Controllers.Contractor.Trips;

[ApiController]
[Route("api/contractor/trips")]
[Authorize]
public class ContractorTripsController : ControllerBase
{
    private readonly ContractorGetTripAction _contractorGetTripAction;
    private readonly ContractorListTripsAction _contractorListTripsAction;
    private readonly ContractorGetTripInvitationStatisticsAction _getTripInvitationStatisticsAction;
    private readonly ContractorGetTripStatusStatisticsAction _getTripStatusStatisticsAction;
    private readonly ContractorGetTripPayoutStatisticsAction _getTripPayoutStatisticsAction;
    private readonly ContractorDownloadTripDocumentAction _downloadTripDocumentAction;
    private readonly ContractorCreateBookingRequestAction _createBookingRequestAction;
    private readonly ContractorWithdrawTripBookingAction _withdrawTripBookingAction;
    private readonly ContractorChangeTripDriverAction _changeTripDriverAction;
    private readonly IMapper _mapper;

    public ContractorTripsController(
        ContractorGetTripAction contractorGetTripAction,
        ContractorListTripsAction contractorListTripsAction,
        ContractorGetTripInvitationStatisticsAction getTripInvitationStatisticsAction,
        ContractorGetTripStatusStatisticsAction getTripStatusStatisticsAction,
        ContractorGetTripPayoutStatisticsAction getTripPayoutStatisticsAction,
        ContractorDownloadTripDocumentAction downloadTripDocumentAction,
        ContractorCreateBookingRequestAction createBookingRequestAction,
        ContractorWithdrawTripBookingAction withdrawTripBookingAction,
        ContractorChangeTripDriverAction changeTripDriverAction,
        IMapper mapper)
    {
        _contractorGetTripAction = contractorGetTripAction;
        _contractorListTripsAction = contractorListTripsAction;
        _getTripInvitationStatisticsAction = getTripInvitationStatisticsAction;
        _getTripStatusStatisticsAction = getTripStatusStatisticsAction;
        _getTripPayoutStatisticsAction = getTripPayoutStatisticsAction;
        _downloadTripDocumentAction = downloadTripDocumentAction;
        _createBookingRequestAction = createBookingRequestAction;
        _withdrawTripBookingAction = withdrawTripBookingAction;
        _changeTripDriverAction = changeTripDriverAction;
        _mapper = mapper;
    }

    [HttpPost("{id:long}/apply")]
    public async Task<ActionResult> Apply(long id)
    {
        var contractorId = ClaimsHelper.GetUserId(User);
        await _createBookingRequestAction.ExecuteAsync(new ContractorCreateBookingRequest().FromRequest(id, contractorId));
        return Ok(new { message = "trips.booking_applied_successfully" });
    }

    [HttpPost("{id:long}/withdraw")]
    public async Task<ActionResult> Withdraw(long id)
    {
        var contractorId = ClaimsHelper.GetUserId(User);
        await _withdrawTripBookingAction.ExecuteAsync(
            new ContractorWithdrawTripBookingRequest().FromRequest(id, contractorId));
        return Ok(new { message = "trips.booking_withdrawn_successfully" });
    }

    [HttpPut("{id:long}/driver")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> ChangeDriver(long id, [FromForm] ContractorChangeTripDriverRequest request)
    {
        var contractorId = ClaimsHelper.GetUserId(User);
        await _changeTripDriverAction.ExecuteAsync(request.FromRequest(id, contractorId));
        return Ok(new { message = "trips.driver_changed_successfully" });
    }

    [HttpGet("invitation-statistics")]
    public async Task<ActionResult<ContractorTripInvitationStatisticsResource>> GetInvitationStatistics()
    {
        var contractorId = ClaimsHelper.GetUserId(User);
        return Ok(ContractorTripInvitationStatisticsResource.FromDto(
            await _getTripInvitationStatisticsAction.ExecuteAsync(contractorId)));
    }

    [HttpGet("status-statistics")]
    public async Task<ActionResult<ContractorTripStatusStatisticsResource>> GetStatusStatistics()
    {
        var contractorId = ClaimsHelper.GetUserId(User);
        return Ok(ContractorTripStatusStatisticsResource.FromDto(
            await _getTripStatusStatisticsAction.ExecuteAsync(contractorId)));
    }

    [HttpGet("payout-statistics")]
    public async Task<ActionResult<ContractorTripPayoutStatisticsResource>> GetPayoutStatistics()
    {
        var contractorId = ClaimsHelper.GetUserId(User);
        return Ok(ContractorTripPayoutStatisticsResource.FromDto(
            await _getTripPayoutStatisticsAction.ExecuteAsync(contractorId)));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ContractorTripResource>> Get(long id)
    {
        var contractorId = ClaimsHelper.GetUserId(User);
        return Ok(_mapper.Map<ContractorTripResource>(
            await _contractorGetTripAction.ExecuteAsync(new ContractorGetTripRequest { Id = id }.FromRequest(contractorId))));
    }

    [HttpGet("{id:long}/documents/download")]
    public async Task<IActionResult> DownloadDocument(long id, [FromQuery] string path)
    {
        var contractorId = ClaimsHelper.GetUserId(User);
        return Redirect(await _downloadTripDocumentAction.ExecuteAsync(
            new ContractorDownloadTripDocumentRequest { TripId = id, Path = path }.FromRequest(contractorId)));
    }

    [HttpGet]
    public async Task<ActionResult> List([FromQuery] ContractorListTripsRequest request)
    {
        var contractorId = ClaimsHelper.GetUserId(User);
        return Ok((await _contractorListTripsAction.ExecuteAsync(request.FromRequest(contractorId)))
            .ToResourceCollection<ContractorTripDto, ContractorTripResource>(dto => _mapper.Map<ContractorTripResource>(dto)));
    }
}
