using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Contractor.Drivers;
using LoadlyAggregator.API.Resources.Admin.Drivers;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Contractor.Actions.Drivers;

namespace LoadlyAggregator.API.Controllers.Contractor.Drivers;

[ApiController]
[Route("api/contractor/drivers")]
[Authorize]
public class ContractorDriversController : ControllerBase
{
    private readonly ContractorListDriversAction _contractorListDriversAction;
    private readonly ContractorGetDriverAction _contractorGetDriverAction;
    private readonly ContractorCreateDriverAction _contractorCreateDriverAction;
    private readonly IMapper _mapper;

    public ContractorDriversController(
        ContractorListDriversAction contractorListDriversAction,
        ContractorGetDriverAction contractorGetDriverAction,
        ContractorCreateDriverAction contractorCreateDriverAction,
        IMapper mapper)
    {
        _contractorListDriversAction = contractorListDriversAction;
        _contractorGetDriverAction = contractorGetDriverAction;
        _contractorCreateDriverAction = contractorCreateDriverAction;
        _mapper = mapper;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Create([FromForm] ContractorCreateDriverRequest request)
    {
        var contractorId = ClaimsHelper.GetUserId(User);
        await _contractorCreateDriverAction.ExecuteAsync(request.FromRequest(contractorId));
        return Ok(new { message = "drivers.created_successfully" });
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<DriverResource>> Get(long id)
    {
        var contractorId = ClaimsHelper.GetUserId(User);
        return Ok(DriverResource.FromDto(
            await _contractorGetDriverAction.ExecuteAsync(new ContractorGetDriverRequest { Id = id }.FromRequest(contractorId))));
    }

    [HttpGet]
    public async Task<ActionResult> List([FromQuery] ContractorListDriversRequest request)
    {
        var contractorId = ClaimsHelper.GetUserId(User);
        return Ok((await _contractorListDriversAction.ExecuteAsync(request.FromRequest(contractorId)))
            .ToResourceCollection<DriverDto, DriverResource>(dto => DriverResource.FromDto(dto)));
    }
}
