using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Contractor.Vehicles;
using LoadlyAggregator.API.Resources.Admin.Vehicles;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Contractor.Actions.Vehicles;

namespace LoadlyAggregator.API.Controllers.Contractor.Vehicles;

[ApiController]
[Route("api/contractor/vehicles")]
[Authorize]
public class ContractorVehiclesController : ControllerBase
{
    private readonly ContractorListVehiclesAction _contractorListVehiclesAction;
    private readonly ContractorGetVehicleAction _contractorGetVehicleAction;
    private readonly IMapper _mapper;

    public ContractorVehiclesController(
        ContractorListVehiclesAction contractorListVehiclesAction,
        ContractorGetVehicleAction contractorGetVehicleAction,
        IMapper mapper)
    {
        _contractorListVehiclesAction = contractorListVehiclesAction;
        _contractorGetVehicleAction = contractorGetVehicleAction;
        _mapper = mapper;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<VehicleResource>> Get(long id)
    {
        var contractorId = ClaimsHelper.GetUserId(User);
        return Ok(_mapper.Map<VehicleResource>(
            await _contractorGetVehicleAction.ExecuteAsync(new ContractorGetVehicleRequest { Id = id }.FromRequest(contractorId))));
    }

    [HttpGet]
    public async Task<ActionResult> List([FromQuery] ContractorListVehiclesRequest request)
    {
        var contractorId = ClaimsHelper.GetUserId(User);
        return Ok((await _contractorListVehiclesAction.ExecuteAsync(request.FromRequest(contractorId)))
            .ToResourceCollection<VehicleDto, VehicleResource>(dto => _mapper.Map<VehicleResource>(dto)));
    }
}
