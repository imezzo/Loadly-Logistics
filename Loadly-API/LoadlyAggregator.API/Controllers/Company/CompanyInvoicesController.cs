using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Extensions;
using LoadlyAggregator.API.Helpers;
using LoadlyAggregator.API.Requests.Company.Invoices;
using LoadlyAggregator.API.Resources.Company.Invoices;
using LoadlyAggregator.Domain.Company.Actions.Invoices;
using LoadlyAggregator.Domain.Company.DTOs.Invoices;
using LoadlyAggregator.Infrastructure.Authorization;

namespace LoadlyAggregator.API.Controllers.Company;

[ApiController]
[Route("api/company/invoices")]
[Authorize]
public class CompanyInvoicesController : ControllerBase
{
    private readonly GetInvoiceAction _getInvoiceAction;
    private readonly ListInvoicesAction _listInvoicesAction;
    private readonly DownloadTaxedInvoiceAttachmentAction _downloadTaxedInvoiceAttachmentAction;

    public CompanyInvoicesController(
        GetInvoiceAction getInvoiceAction,
        ListInvoicesAction listInvoicesAction,
        DownloadTaxedInvoiceAttachmentAction downloadTaxedInvoiceAttachmentAction)
    {
        _getInvoiceAction = getInvoiceAction;
        _listInvoicesAction = listInvoicesAction;
        _downloadTaxedInvoiceAttachmentAction = downloadTaxedInvoiceAttachmentAction;
    }

    [HttpGet("{id}")]
    // [HasPermission(GetInvoiceRequest.Permission)]
    public async Task<ActionResult<InvoiceResource>> Get(long id)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        return Ok(InvoiceResource.FromDto(await _getInvoiceAction.ExecuteAsync(new GetInvoiceRequest { Id = id }.FromRequest(), companyId)));
    }

    [HttpGet("{id}/download")]
    // [HasPermission(DownloadTaxedInvoiceAttachmentRequest.Permission)]
    public async Task<IActionResult> Download(long id)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        return Redirect(await _downloadTaxedInvoiceAttachmentAction.ExecuteAsync(
            new DownloadTaxedInvoiceAttachmentRequest { Id = id }.FromRequest(), companyId));
    }

    [HttpGet]
    // [HasPermission(ListInvoicesRequest.Permission)]
    public async Task<ActionResult> List([FromQuery] ListInvoicesRequest request)
    {
        var companyId = ClaimsHelper.GetCompanyId(User);
        return Ok((await _listInvoicesAction.ExecuteAsync(request.FromRequest(companyId)))
            .ToResourceCollection<InvoiceDto, InvoiceResource>(dto => InvoiceResource.FromDto(dto)));
    }
}
