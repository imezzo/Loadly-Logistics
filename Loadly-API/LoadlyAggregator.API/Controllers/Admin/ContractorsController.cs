using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Contractors;
using LoadlyAggregator.API.Resources.Admin.Contractors;
using LoadlyAggregator.Domain.Admin.Actions.Contractors;
using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/contractors")]
[Authorize]
public class ContractorsController : ControllerBase
{
    private readonly CreateContractorAction _createContractorAction;
    private readonly GetContractorAction _getContractorAction;
    private readonly ListContractorsAction _listContractorsAction;
    private readonly UpdateContractorAction _updateContractorAction;
    private readonly DeleteContractorAction _deleteContractorAction;
    private readonly GetContractorDashboardStatisticsAction _getContractorDashboardStatisticsAction;
    private readonly IMapper _mapper;

    public ContractorsController(
        CreateContractorAction createContractorAction,
        GetContractorAction getContractorAction,
        ListContractorsAction listContractorsAction,
        UpdateContractorAction updateContractorAction,
        DeleteContractorAction deleteContractorAction,
        GetContractorDashboardStatisticsAction getContractorDashboardStatisticsAction,
        IMapper mapper)
    {
        _createContractorAction = createContractorAction;
        _listContractorsAction = listContractorsAction;
        _getContractorAction = getContractorAction;
        _updateContractorAction = updateContractorAction;
        _deleteContractorAction = deleteContractorAction;
        _getContractorDashboardStatisticsAction = getContractorDashboardStatisticsAction;
        _mapper = mapper;
    }

    [HttpPost]
    [HasPermission(CreateContractorRequest.Permission)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Create([FromForm] CreateContractorRequest request)
    {
        await _createContractorAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "contractors.created_successfully" });
    }

    [HttpGet("statistics")]
    [HasPermission(GetContractorDashboardStatisticsRequest.Permission)]
    public async Task<ActionResult<ContractorDashboardStatisticsResource>> Statistics(
        [FromQuery] GetContractorDashboardStatisticsRequest request) =>
        Ok(_mapper.Map<ContractorDashboardStatisticsResource>(
            await _getContractorDashboardStatisticsAction.ExecuteAsync(request.FromRequest())));

    [HttpGet("{id}")]
    [HasPermission(GetContractorRequest.Permission)]
    public async Task<ActionResult<ContractorResource>> Get(long id) =>
        Ok(ContractorResource.FromDto(await _getContractorAction.ExecuteAsync(new GetContractorRequest { Id = id }.FromRequest())));

    [HttpGet]
    [HasPermission(ListContractorsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListContractorsRequest request) =>
        Ok((await _listContractorsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<ContractorDto, ContractorResource>(dto => ContractorResource.FromDto(dto)));

    [HttpPut("{id}")]
    [HasPermission(UpdateContractorRequest.Permission)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Update(long id, [FromForm] UpdateContractorRequest request)
    {
        request.Id = id;
        await _updateContractorAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "contractors.updated_successfully" });
    }

    [HttpDelete("{id}")]
    [HasPermission(DeleteContractorRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteContractorAction.ExecuteAsync(new DeleteContractorRequest { Id = id }.FromRequest());
        return Ok(new { message = "contractors.deleted_successfully" });
    }
}
