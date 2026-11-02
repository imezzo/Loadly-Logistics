using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.ContractorPayouts;
using LoadlyAggregator.API.Resources.Admin.ContractorPayouts;
using LoadlyAggregator.Domain.Admin.Actions.ContractorPayouts;
using LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/contractor-payouts")]
[Authorize]
public class ContractorPayoutsController : ControllerBase
{
    private readonly CreateContractorPayoutAction _createContractorPayoutAction;
    private readonly SetContractorPayoutPaidAction _setContractorPayoutPaidAction;
    private readonly BulkSetContractorPayoutsPaidAction _bulkSetContractorPayoutsPaidAction;
    private readonly SetContractorPayoutCancelledAction _setContractorPayoutCancelledAction;
    private readonly GetContractorPayoutAction _getContractorPayoutAction;
    private readonly ListContractorPayoutsAction _listContractorPayoutsAction;
    private readonly IMapper _mapper;

    public ContractorPayoutsController(
        CreateContractorPayoutAction createContractorPayoutAction,
        SetContractorPayoutPaidAction setContractorPayoutPaidAction,
        BulkSetContractorPayoutsPaidAction bulkSetContractorPayoutsPaidAction,
        SetContractorPayoutCancelledAction setContractorPayoutCancelledAction,
        GetContractorPayoutAction getContractorPayoutAction,
        ListContractorPayoutsAction listContractorPayoutsAction,
        IMapper mapper)
    {
        _createContractorPayoutAction = createContractorPayoutAction;
        _setContractorPayoutPaidAction = setContractorPayoutPaidAction;
        _bulkSetContractorPayoutsPaidAction = bulkSetContractorPayoutsPaidAction;
        _setContractorPayoutCancelledAction = setContractorPayoutCancelledAction;
        _getContractorPayoutAction = getContractorPayoutAction;
        _listContractorPayoutsAction = listContractorPayoutsAction;
        _mapper = mapper;
    }

    [HttpGet]
    [HasPermission(ListContractorPayoutsRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListContractorPayoutsRequest request) =>
        Ok((await _listContractorPayoutsAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<ContractorPayoutDto, ContractorPayoutResource>(
                dto => _mapper.Map<ContractorPayoutResource>(dto)));

    [HttpGet("{id:long}")]
    [HasPermission(GetContractorPayoutRequest.Permission)]
    public async Task<ActionResult<ContractorPayoutResource>> Get(long id) =>
        Ok(_mapper.Map<ContractorPayoutResource>(
            await _getContractorPayoutAction.ExecuteAsync(new GetContractorPayoutRequest { Id = id }.FromRequest())));

    [HttpPost]
    [HasPermission(CreateContractorPayoutRequest.Permission)]
    public async Task<ActionResult> Create([FromBody] CreateContractorPayoutRequest request)
    {
        var id = await _createContractorPayoutAction.ExecuteAsync(request.FromRequest());
        return Ok(new { id, message = "contractor_payouts.created_successfully" });
    }

    [HttpPatch("bulk-set-paid")]
    [HasPermission(BulkSetContractorPayoutsPaidRequest.Permission)]
    public async Task<ActionResult> BulkSetPaid([FromBody] BulkSetContractorPayoutsPaidRequest request)
    {
        await _bulkSetContractorPayoutsPaidAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "contractor_payouts.bulk_paid_successfully" });
    }

    [HttpPatch("{id:long}/set-paid")]
    [HasPermission(SetContractorPayoutPaidRequest.Permission)]
    public async Task<ActionResult> SetPaid(long id)
    {
        await _setContractorPayoutPaidAction.ExecuteAsync(id);
        return Ok(new { message = "contractor_payouts.paid_successfully" });
    }

    [HttpPatch("{id:long}/cancel")]
    [HasPermission(SetContractorPayoutCancelledRequest.Permission)]
    public async Task<ActionResult> SetCancelled(long id, [FromBody] SetContractorPayoutCancelledRequest request)
    {
        await _setContractorPayoutCancelledAction.ExecuteAsync(request.FromRequest(id));
        return Ok(new { message = "contractor_payouts.cancelled_successfully" });
    }
}
