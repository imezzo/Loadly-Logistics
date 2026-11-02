using System.Linq;
using AutoMapper;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Invoices;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Company.Actions.Invoices;

public class ListInvoicesAction
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;

    public ListInvoicesAction(IInvoiceRepository invoiceRepository, IMediaService mediaService, IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _mediaService = mediaService;
        _mapper = mapper;
    }

    public async Task<ListingDto<InvoiceDto>> ExecuteAsync(ListInvoicesDto dto)
    {
        var (invoices, totalCount) = await _invoiceRepository.ListAsync(
            dto.Search,
            dto.CompanyId,
            dto.ContractId,
            dto.Status,
            dto.InvoiceType,
            dto.StartDateFrom,
            dto.StartDateTo,
            dto.IsOverdue,
            dto.IsLeadBilling,
            dto.Page,
            dto.PageSize);

        var data = _mapper.Map<List<InvoiceDto>>(invoices);
        for (var i = 0; i < invoices.Count; i++)
        {
            var paidAmount = invoices[i].Payments?.Sum(p => p.Amount) ?? 0m;
            data[i].PaidAmount = paidAmount;
            data[i].RemainingAmount = invoices[i].TotalAmount - paidAmount;

            var taxedInvoiceAttachmentPath = data[i].TaxedInvoiceAttachment;
            if (!string.IsNullOrWhiteSpace(taxedInvoiceAttachmentPath))
            {
                data[i].FileUrl = _mediaService.Read(taxedInvoiceAttachmentPath);
            }
        }

        return new ListingDto<InvoiceDto>
        {
            Data = data,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
