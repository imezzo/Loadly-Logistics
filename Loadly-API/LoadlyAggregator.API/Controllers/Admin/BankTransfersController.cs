using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Invoices;
using LoadlyAggregator.API.Resources.Admin.Invoices;
using LoadlyAggregator.Domain.Admin.Actions.Invoices;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/bank-transfers")]
[Authorize]
public class BankTransfersController : ControllerBase
{
    private readonly CreateBankTransferAction _createAction;
    private readonly GetBankTransferAction _getAction;
    private readonly ListBankTransfersAction _listAction;
    private readonly UpdateBankTransferAction _updateAction;
    private readonly DeleteBankTransferAction _deleteAction;
    private readonly DownloadBankTransferAttachmentAction _downloadAttachmentAction;
    private readonly IMapper _mapper;

    public BankTransfersController(
        CreateBankTransferAction createAction,
        GetBankTransferAction getAction,
        ListBankTransfersAction listAction,
        UpdateBankTransferAction updateAction,
        DeleteBankTransferAction deleteAction,
        DownloadBankTransferAttachmentAction downloadAttachmentAction,
        IMapper mapper)
    {
        _createAction = createAction;
        _getAction = getAction;
        _listAction = listAction;
        _updateAction = updateAction;
        _deleteAction = deleteAction;
        _downloadAttachmentAction = downloadAttachmentAction;
        _mapper = mapper;
    }

    [HttpGet]
    [HasPermission(ListBankTransfersRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListBankTransfersRequest request) =>
        Ok((await _listAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<BankTransferDto, BankTransferResource>(dto => _mapper.Map<BankTransferResource>(dto)));

    [HttpPost]
    [HasPermission(CreateBankTransferRequest.Permission)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Create([FromForm] CreateBankTransferRequest request)
    {
        await _createAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "bank_transfers.created_successfully" });
    }

    [HttpGet("{id}")]
    [HasPermission(GetBankTransferRequest.Permission)]
    public async Task<ActionResult<BankTransferResource>> Get(long id) =>
        Ok(_mapper.Map<BankTransferResource>(
            await _getAction.ExecuteAsync(new GetBankTransferRequest { Id = id }.FromRequest())));

    [HttpPut("{id}")]
    [HasPermission(UpdateBankTransferRequest.Permission)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Update(long id, [FromForm] UpdateBankTransferRequest request)
    {
        await _updateAction.ExecuteAsync(request.FromRequest(id));
        return Ok(new { message = "bank_transfers.updated_successfully" });
    }

    [HttpGet("{id}/download")]
    [HasPermission(DownloadBankTransferAttachmentRequest.Permission)]
    public async Task<IActionResult> Download(long id) =>
        Redirect(await _downloadAttachmentAction.ExecuteAsync(
            new DownloadBankTransferAttachmentRequest { Id = id }.FromRequest()));

    [HttpDelete("{id}")]
    [HasPermission(DeleteBankTransferRequest.Permission)]
    public async Task<ActionResult> Delete(long id)
    {
        await _deleteAction.ExecuteAsync(new DeleteBankTransferRequest { Id = id }.FromRequest());
        return Ok(new { message = "bank_transfers.deleted_successfully" });
    }
}
