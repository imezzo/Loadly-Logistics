using System.Linq;
using AutoMapper;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Invoices;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Invoices;

public class GetInvoiceAction
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;

    public GetInvoiceAction(IInvoiceRepository invoiceRepository, IMediaService mediaService, IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _mediaService = mediaService;
        _mapper = mapper;
    }

    public async Task<InvoiceDto> ExecuteAsync(GetInvoiceDto dto, long companyId)
    {
        var invoice = await _invoiceRepository.GetByIdOrThrowAsync(dto.Id);
        if (invoice.Contract?.CompanyId != companyId)
        {
            throw new ForbiddenException("invoices.access_denied");
        }

        var result = _mapper.Map<InvoiceDto>(invoice);
        var paidAmount = invoice.Payments?.Sum(p => p.Amount) ?? 0m;
        result.PaidAmount = paidAmount;
        result.RemainingAmount = invoice.TotalAmount - paidAmount;
        var taxedInvoiceAttachmentPath = result.TaxedInvoiceAttachment;
        if (!string.IsNullOrWhiteSpace(taxedInvoiceAttachmentPath))
        {
            result.FileUrl = _mediaService.Read(taxedInvoiceAttachmentPath);
        }

        return result;
    }
}
