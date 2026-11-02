using System.Linq;
using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Invoices;

public class GetInvoiceAction
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IContractRepository _contractRepository;
    private readonly ITripRepository _tripRepository;
    private readonly IVehicleFacilityRepository _facilityRepository;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;

    public GetInvoiceAction(
        IInvoiceRepository invoiceRepository,
        IContractRepository contractRepository,
        ITripRepository tripRepository,
        IVehicleFacilityRepository facilityRepository,
        IMediaService mediaService,
        IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _contractRepository = contractRepository;
        _tripRepository = tripRepository;
        _facilityRepository = facilityRepository;
        _mediaService = mediaService;
        _mapper = mapper;
    }

    public async Task<InvoiceDto> ExecuteAsync(GetInvoiceDto dto)
    {
        var invoice = await _invoiceRepository.GetByIdOrThrowAsync(dto.Id);
        var result = _mapper.Map<InvoiceDto>(invoice);
        var paidAmount = invoice.Payments?.Sum(p => p.Amount) ?? 0m;
        result.PaidAmount = paidAmount;
        result.RemainingAmount = invoice.TotalAmount - paidAmount;
        var taxedInvoiceAttachmentPath = result.TaxedInvoiceAttachment;
        if (!string.IsNullOrWhiteSpace(taxedInvoiceAttachmentPath))
        {
            result.FileUrl = _mediaService.Read(taxedInvoiceAttachmentPath);
        }

        result.SlotCharges = InvoiceSlotChargesDto.FromInvoice(invoice);
        if (result.SlotCharges is not null && invoice.ContractId is long contractId)
        {
            var contract = await _contractRepository.GetByIdOrThrowForSlotBillingAsync(contractId);
            result.SlotCharges.Vehicles = ContractSlotBillingVehiclesMapper.MapVehicles(
                IncludedContractSlotChargesComputer.ResolveTargetSlots(
                    contract,
                    null,
                    invoice.StartDate,
                    invoice.EndDate));
            await CalendarDayFacilityNameHelper.ApplyAsync(result.SlotCharges, _facilityRepository);
            await ContractBillingCalendarDayTripCosts.ApplyAsync(result.SlotCharges, contract, _tripRepository);
        }

        return result;
    }
}
