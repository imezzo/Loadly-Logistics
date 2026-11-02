using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Company.Contracts;
using LoadlyAggregator.API.Resources.Admin.Contracts;
using LoadlyAggregator.API.Resources.Company.Contracts;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Company.Actions.Contracts;

namespace LoadlyAggregator.API.Controllers.Company;

[ApiController]
[Route("api/company/contracts")]
[Authorize]
public class CompanyContractsController : ControllerBase
{
    private readonly CompanyGetContractAction _companyGetContractAction;
    private readonly CompanyGetContractConsumptionTypeAction _companyGetContractConsumptionTypeAction;
    private readonly CompanyGetContractDestinationAction _companyGetContractDestinationAction;
    private readonly CompanyListContractsAction _companyListContractsAction;
    private readonly IMapper _mapper;

    public CompanyContractsController(
        CompanyGetContractAction companyGetContractAction,
        CompanyGetContractConsumptionTypeAction companyGetContractConsumptionTypeAction,
        CompanyGetContractDestinationAction companyGetContractDestinationAction,
        CompanyListContractsAction companyListContractsAction,
        IMapper mapper)
    {
        _companyGetContractAction = companyGetContractAction;
        _companyGetContractConsumptionTypeAction = companyGetContractConsumptionTypeAction;
        _companyGetContractDestinationAction = companyGetContractDestinationAction;
        _companyListContractsAction = companyListContractsAction;
        _mapper = mapper;
    }

    [HttpGet("destinations/{destinationId}")]
    // [HasPermission(CompanyGetContractDestinationRequest.Permission)]
    public async Task<ActionResult<ContractDestinationResource>> GetDestination(long destinationId)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        return Ok(_mapper.Map<ContractDestinationResource>(await _companyGetContractDestinationAction.ExecuteAsync(
            new CompanyGetContractDestinationRequest { DestinationId = destinationId }.FromRequest(companyId))));
    }

    [HttpGet("{id:long}/consumption-type")]
    public async Task<ActionResult<CompanyContractConsumptionTypeResource>> GetConsumptionType(long id)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        return Ok(CompanyContractConsumptionTypeResource.FromDto(await _companyGetContractConsumptionTypeAction.ExecuteAsync(
            new CompanyGetContractRequest { Id = id }.FromRequest(companyId))));
    }

    [HttpGet("{id}")]
    // [HasPermission(CompanyGetContractRequest.Permission)]
    public async Task<ActionResult<ContractResource>> Get(long id)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        return Ok(_mapper.Map<ContractResource>(await _companyGetContractAction.ExecuteAsync(
            new CompanyGetContractRequest { Id = id }.FromRequest(companyId))));
    }

    [HttpGet]
    // [HasPermission(CompanyListContractsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] CompanyListContractsRequest request)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        return Ok((await _companyListContractsAction.ExecuteAsync(request.FromRequest(companyId)))
            .ToResourceCollection<ContractDto, ContractResource>(dto => _mapper.Map<ContractResource>(dto)));
    }
}
