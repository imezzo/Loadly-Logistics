using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Requests.Admin.Invoices;
using LoadlyAggregator.API.Resources.Admin.Invoices;
using LoadlyAggregator.Domain.Admin.Actions.Invoices;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Admin;

[ApiController]
[Route("api/admin/invoices")]
[Authorize]
public class InvoicesController : ControllerBase
{
    private readonly CreateInvoiceAction _createInvoiceAction;
    private readonly LinkInvoicePaymentsAction _linkInvoicePaymentsAction;
    private readonly CancelInvoiceAction _cancelInvoiceAction;
    private readonly SetInvoicePaidAction _setInvoicePaidAction;
    private readonly GetInvoiceAction _getInvoiceAction;
    private readonly ListInvoicesAction _listInvoicesAction;
    private readonly GetInvoiceStatisticsAction _getInvoiceStatisticsAction;
    private readonly DownloadTaxedInvoiceAttachmentAction _downloadTaxedInvoiceAttachmentAction;
    private readonly ExportInvoicesExcelAction _exportInvoicesExcelAction;

    public InvoicesController(
        CreateInvoiceAction createInvoiceAction,
        LinkInvoicePaymentsAction linkInvoicePaymentsAction,
        CancelInvoiceAction cancelInvoiceAction,
        SetInvoicePaidAction setInvoicePaidAction,
        GetInvoiceAction getInvoiceAction,
        ListInvoicesAction listInvoicesAction,
        GetInvoiceStatisticsAction getInvoiceStatisticsAction,
        DownloadTaxedInvoiceAttachmentAction downloadTaxedInvoiceAttachmentAction,
        ExportInvoicesExcelAction exportInvoicesExcelAction)
    {
        _createInvoiceAction = createInvoiceAction;
        _linkInvoicePaymentsAction = linkInvoicePaymentsAction;
        _cancelInvoiceAction = cancelInvoiceAction;
        _setInvoicePaidAction = setInvoicePaidAction;
        _getInvoiceAction = getInvoiceAction;
        _listInvoicesAction = listInvoicesAction;
        _getInvoiceStatisticsAction = getInvoiceStatisticsAction;
        _downloadTaxedInvoiceAttachmentAction = downloadTaxedInvoiceAttachmentAction;
        _exportInvoicesExcelAction = exportInvoicesExcelAction;
    }

    [HttpPost]
    [HasPermission(CreateInvoiceRequest.Permission)]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult> Create([FromForm] CreateInvoiceRequest request)
    {
        await _createInvoiceAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "invoices.created_successfully" });
    }


    [HttpPut("{id:long}/link-payments")]
    [HasPermission(LinkInvoicePaymentsRequest.Permission)]
    public async Task<ActionResult> LinkPayments(long id, [FromBody] LinkInvoicePaymentsRequest request)
    {
        await _linkInvoicePaymentsAction.ExecuteAsync(request.FromRequest(id));
        return Ok(new { message = "invoices.payments_linked_successfully" });
    }

    [HttpPost("{id:long}/cancel")]
    [HasPermission(CancelInvoiceRequest.Permission)]
    public async Task<ActionResult> Cancel(long id, [FromBody] CancelInvoiceRequest request)
    {
        request.Id = id;
        await _cancelInvoiceAction.ExecuteAsync(request.FromRequest());
        return Ok(new { message = "invoices.cancelled_successfully" });
    }

    [HttpPost("{id:long}/set-paid")]
    [HasPermission(SetInvoicePaidRequest.Permission)]
    public async Task<ActionResult> SetPaid(long id)
    {
        await _setInvoicePaidAction.ExecuteAsync(new SetInvoicePaidRequest { Id = id }.FromRequest());
        return Ok(new { message = "invoices.paid_successfully" });
    }

    [HttpGet("statistics")]
    [HasPermission(ListInvoicesRequest.Permission)]
    public async Task<ActionResult<InvoiceStatisticsResource>> Statistics() =>
        Ok(InvoiceStatisticsResource.FromDto(await _getInvoiceStatisticsAction.ExecuteAsync()));

    [HttpGet("excel")]
    [HasPermission(ExportInvoicesExcelRequest.Permission)]
    public async Task<IActionResult> ExportExcel([FromQuery] ExportInvoicesExcelRequest request)
    {
        InvoicesExcelFileResource resource = InvoicesExcelFileResource.FromDto(
            await _exportInvoicesExcelAction.ExecuteAsync(request.FromRequest()));
        return File(resource.Content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", resource.FileName);
    }

    [HttpGet("{id}")]
    [HasPermission(GetInvoiceRequest.Permission)]
    public async Task<ActionResult<InvoiceResource>> Get(long id) =>
        Ok(InvoiceResource.FromDto(await _getInvoiceAction.ExecuteAsync(new GetInvoiceRequest { Id = id }.FromRequest())));

    [HttpGet("{id}/download")]
    [HasPermission(DownloadTaxedInvoiceAttachmentRequest.Permission)]
    public async Task<IActionResult> Download(long id) =>
        Redirect(await _downloadTaxedInvoiceAttachmentAction.ExecuteAsync(
            new DownloadTaxedInvoiceAttachmentRequest { Id = id }.FromRequest()));

    [HttpGet]
    [HasPermission(ListInvoicesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListInvoicesRequest request) =>
        Ok((await _listInvoicesAction.ExecuteAsync(request.FromRequest()))
            .ToResourceCollection<Domain.Admin.DTOs.Invoices.InvoiceDto, InvoiceResource>(dto => InvoiceResource.FromDto(dto)));
}
