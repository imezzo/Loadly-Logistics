using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Contractor.Trips;
using LoadlyAggregator.API.Resources.Admin.Trips;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Contractor.Actions.Trips;

namespace LoadlyAggregator.API.Controllers.Contractor.Trips;

[ApiController]
[Route("api/contractor/booking-requests")]
[Authorize]
public class ContractorBookingRequestsController : ControllerBase
{
    private readonly ContractorListBookingRequestsAction _listBookingRequestsAction;
    private readonly ContractorGetBookingRequestAction _getBookingRequestAction;
    private readonly IMapper _mapper;

    public ContractorBookingRequestsController(
        ContractorListBookingRequestsAction listBookingRequestsAction,
        ContractorGetBookingRequestAction getBookingRequestAction,
        IMapper mapper)
    {
        _listBookingRequestsAction = listBookingRequestsAction;
        _getBookingRequestAction = getBookingRequestAction;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult> List([FromQuery] ContractorListBookingRequestsRequest request)
    {
        var contractorId = ClaimsHelper.GetUserId(User);
        return Ok((await _listBookingRequestsAction.ExecuteAsync(request.FromRequest(contractorId)))
            .ToResourceCollection<ContractorTripBookingRequestDto, ContractorTripBookingRequestResource>(
                dto => _mapper.Map<ContractorTripBookingRequestResource>(dto)));
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<ContractorTripBookingRequestResource>> Get(long id)
    {
        var contractorId = ClaimsHelper.GetUserId(User);
        return Ok(_mapper.Map<ContractorTripBookingRequestResource>(
            await _getBookingRequestAction.ExecuteAsync(
                new ContractorGetBookingRequestRequest { Id = id }.FromRequest(contractorId))));
    }
}
