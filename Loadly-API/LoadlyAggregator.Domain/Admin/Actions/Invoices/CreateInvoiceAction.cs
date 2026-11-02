using LoadlyAggregator.Domain.Admin.Actions.Contracts;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Admin.Services;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Invoices;

public class CreateInvoiceAction
{
    private readonly ITripRepository _tripRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IInvoiceDetailRepository _invoiceDetailRepository;
    private readonly IContractRepository _contractRepository;
    private readonly IContractBillingSnapshotRepository _billingSnapshotRepository;
    private readonly ILeadRepository _leadRepository;
    private readonly ITaxRepository _taxRepository;
    private readonly IDiscountRepository _discountRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediaService _mediaService;
    private readonly InvoicePaymentAllocator _allocator;
    private readonly CalculateContractSlotChargesAction _calculateContractSlotChargesAction;

    public CreateInvoiceAction(
        ITripRepository tripRepository,
        IInvoiceRepository invoiceRepository,
        IInvoiceDetailRepository invoiceDetailRepository,
        IContractRepository contractRepository,
        IContractBillingSnapshotRepository billingSnapshotRepository,
        ILeadRepository leadRepository,
        ITaxRepository taxRepository,
        IDiscountRepository discountRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IMediaService mediaService,
        InvoicePaymentAllocator allocator,
        CalculateContractSlotChargesAction calculateContractSlotChargesAction)
    {
        _tripRepository = tripRepository;
        _invoiceRepository = invoiceRepository;
        _invoiceDetailRepository = invoiceDetailRepository;
        _contractRepository = contractRepository;
        _billingSnapshotRepository = billingSnapshotRepository;
        _leadRepository = leadRepository;
        _taxRepository = taxRepository;
        _discountRepository = discountRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _mediaService = mediaService;
        _allocator = allocator;
        _calculateContractSlotChargesAction = calculateContractSlotChargesAction;
    }

    public async Task ExecuteAsync(CreateInvoiceDto dto)
    {
        var startDate = dto.StartDate;
        var endDate = dto.EndDate;
        if (startDate > endDate)
            throw new ValidationException("validation.end_date_gte_start_date");

        var tripIds = dto.TripIds.Distinct().ToList();

        var isLeadBilling = dto.LeadId is > 0;

        Contract? contract = null;
        if (isLeadBilling)
            _ = await _leadRepository.GetByIdOrThrowAsync(dto.LeadId!.Value);
        else
            contract = await _contractRepository.GetByIdOrThrowAsync(dto.ContractId);

        var isMonthlyContract = MonthlySlotInvoicePolicy.IsMonthlyContract(contract, isLeadBilling);
        var usesMonthlyBillingSnapshot = MonthlySlotInvoicePolicy.UsesBillingSnapshot(contract, isLeadBilling);

        if (isMonthlyContract && dto.ContractBillingSnapshotId is not > 0)
            throw new ValidationException("invoices.contract_billing_snapshot_id_required");

        if (usesMonthlyBillingSnapshot
            && await _invoiceRepository.HasOverlappingInvoiceForContractAsync(
                dto.ContractId,
                startDate,
                endDate,
                dto.InvoiceType))
        {
            throw new ValidationException(dto.InvoiceType == InvoiceType.Trips
                ? "invoices.overlapping_trips_invoice_for_period"
                : "invoices.overlapping_claims_invoice_for_period");
        }

        ContractBillingSnapshot? billingSnapshot = null;
        IReadOnlyList<Trip> calendarTrips = [];
        decimal slotChargesSubTotal = 0m;
        List<Trip> trips;

        if (usesMonthlyBillingSnapshot && dto.ContractBillingSnapshotId is long snapshotId)
        {
            contract = await _contractRepository.GetByIdOrThrowForSlotBillingAsync(dto.ContractId);
            billingSnapshot = await ResolveMonthlyBillingSnapshotAsync(dto, contract, startDate, endDate);
            ApplyExtraKmRoutingFromRequest(billingSnapshot, dto);

            ContractBillingSnapshotInvoicingProgress.EnsureCanCreateInvoice(
                billingSnapshot,
                dto.InvoiceType);

            tripIds = ContractBillingSnapshotReader.CollectDistinctTripIds(billingSnapshot);
            var calendarTripIds = tripIds;
            calendarTrips = calendarTripIds.Count == 0
                ? []
                : await _tripRepository.GetByIdsAsync(calendarTripIds);

            var fetchedTrips = calendarTrips.ToDictionary(t => t.Id);
            trips = tripIds.Count == 0
                ? []
                : InvoiceTripSelection.ResolveEligibleTrips(
                    tripIds,
                    fetchedTrips,
                    dto.InvoiceType,
                    isLeadBilling,
                    dto.ContractId,
                    dto.LeadId);

            ContractBillingSnapshotInvoicingProgress.EnsureInvoiceTypeStillRequired(
                billingSnapshot,
                dto.InvoiceType,
                calendarTrips);

            slotChargesSubTotal = ContractBillingSnapshotSlotChargeRollups
                .ForInvoiceType(billingSnapshot, dto.InvoiceType)
                .SubTotal;
        }
        else
        {
            var fetchedTrips = tripIds.Count == 0
                ? new Dictionary<long, Trip>()
                : (await _tripRepository.GetByIdsAsync(tripIds)).ToDictionary(t => t.Id);
            trips = InvoiceTripSelection.ResolveEligibleTrips(
                tripIds,
                fetchedTrips,
                dto.InvoiceType,
                isLeadBilling,
                dto.ContractId,
                dto.LeadId);
        }

        Tax? tax = null;
        Discount? discount = null;
        if (dto.TaxId.HasValue)
            tax = await _taxRepository.GetByIdOrThrowAsync(dto.TaxId.Value);
        if (dto.DiscountId.HasValue)
            discount = await _discountRepository.GetByIdOrThrowAsync(dto.DiscountId.Value);

        var addedBy = _currentUserService.GetCurrentUserId();
        var year = endDate.Year;

        decimal tripSubTotal = trips.Sum(t => TripChargeInvoiceRouting.SumBillableForInvoiceType(t, dto.InvoiceType));
        decimal subTotal = slotChargesSubTotal + tripSubTotal;
        InvoiceSubTotalComposition.EnsureValid(slotChargesSubTotal, tripSubTotal, subTotal);

        if (subTotal <= 0)
        {
            var noBillableAmount = billingSnapshot is not null
                || (trips.Count == 0 && !isMonthlyContract);
            if (noBillableAmount)
            {
                throw new LogicalException(billingSnapshot is not null
                    ? "invoices.no_billable_amount_for_period"
                    : "invoices.no_uninvoiced_trips_found");
            }
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            if (billingSnapshot is { Id: > 0 })
            {
                billingSnapshot = await _billingSnapshotRepository.GetByIdWithDetailsOrThrowAsync(
                    billingSnapshot.Id);
                ApplyExtraKmRoutingFromRequest(billingSnapshot, dto);
                ContractBillingSnapshotInvoicingProgress.EnsureCanCreateInvoice(
                    billingSnapshot,
                    dto.InvoiceType);
                ContractBillingSnapshotInvoicingProgress.EnsureInvoiceTypeStillRequired(
                    billingSnapshot,
                    dto.InvoiceType,
                    calendarTrips);

                slotChargesSubTotal = ContractBillingSnapshotSlotChargeRollups
                    .ForInvoiceType(billingSnapshot, dto.InvoiceType)
                    .SubTotal;
                subTotal = slotChargesSubTotal + tripSubTotal;
                InvoiceSubTotalComposition.EnsureValid(slotChargesSubTotal, tripSubTotal, subTotal);

                if (subTotal <= 0)
                    throw new LogicalException("invoices.no_billable_amount_for_period");
            }

            var (discountAmount, taxAmount, totalAmount) = InvoiceTotalsCalculator.CalculateAll(
                subTotal,
                tax?.Type,
                tax?.Value,
                discount?.Type,
                discount?.Value);

            var nextSequence = await _invoiceRepository.AllocateNextInvoiceSequenceForYearAsync(year);
            var invoice = new Invoice
            {
                InvoiceNumber = $"INV-{year}-{nextSequence:D6}",
                Sequence = nextSequence,
                InvoiceType = dto.InvoiceType,
                ContractId = isLeadBilling ? null : dto.ContractId,
                LeadId = isLeadBilling ? dto.LeadId : null,
                StartDate = startDate,
                EndDate = endDate,
                IssueDate = dto.IssueDate,
                DueDate = dto.DueDate,
                SubTotal = subTotal,
                SlotChargesSubTotal = slotChargesSubTotal,
                TripsSubTotal = tripSubTotal,
                TaxId = dto.TaxId,
                DiscountId = dto.DiscountId,
                TaxType = tax?.Type,
                TaxValue = tax?.Value,
                TaxAmount = taxAmount,
                DiscountType = discount?.Type,
                DiscountValue = discount?.Value,
                DiscountAmount = discountAmount,
                TotalAmount = totalAmount,
                Status = InvoiceStatus.UnPaid,
                AddedBy = addedBy
            };
            await _invoiceRepository.AddAsync(invoice);

            if (trips.Count > 0)
            {
                foreach (var trip in trips)
                {
                    if (dto.InvoiceType == InvoiceType.Trips)
                        trip.Invoice = invoice;
                    else
                        trip.ClaimInvoice = invoice;
                }
            }

            await _unitOfWork.SaveChangesAsync();

            var invoiceDetail = new InvoiceDetail
            {
                InvoiceId = invoice.Id,
                AddedBy = addedBy
            };

            await _invoiceDetailRepository.AddAsync(invoiceDetail);
            
            if (billingSnapshot is not null)
            {
                if (billingSnapshot.Id == 0)
                    await _billingSnapshotRepository.AddAsync(billingSnapshot);

                ContractBillingSnapshotLifecycle.StageInvoiceAgainstSnapshot(billingSnapshot, invoice);
            }

            InvoiceBillingSnapshotApplier.ApplyTripRollupsTo(invoice, trips);

            InvoiceSubTotalComposition.EnsureValid(invoice);

            if (trips.Count > 0)
                await _tripRepository.UpdateRangeAsync(trips);

            var hasAttachment = dto.TaxedInvoiceAttachment is { Length: > 0 };
            var hasPayments = dto.Payments is { Count: > 0 };

            if (hasAttachment || hasPayments)
            {
                if (hasAttachment)
                {
                    var directoryPath = $"invoice/{invoice.Id}";
                    var fileName = await _mediaService.UploadAsync(dto.TaxedInvoiceAttachment!, directoryPath);
                    invoice.TaxedInvoiceAttachment = $"{directoryPath}/{fileName}";
                }

                if (hasPayments)
                {
                    invoice.Contract = contract;
                    var allocations = dto.Payments!
                        .Select(p => (p.BankTransferId, p.Amount))
                        .ToList();
                    await _allocator.ApplyBatchAsync(invoice, allocations, addedBy);
                }
                else if (dto.Status.HasValue)
                {
                    invoice.Status = dto.Status.Value;
                }
            }
            else
            {
                if (dto.Status.HasValue)
                    invoice.Status = dto.Status.Value;
            }

            if (invoice.Status == InvoiceStatus.Paid)
            {
                invoiceDetail.PaidBy = addedBy;
                invoiceDetail.PaidAt = DateTime.UtcNow;
            }

            await _unitOfWork.SaveChangesAsync();

            if (billingSnapshot is not null)
            {
                ContractBillingSnapshotLifecycle.LinkPersistedInvoice(
                    billingSnapshot,
                    invoice,
                    calendarTrips);
                await _unitOfWork.SaveChangesAsync();
            }

            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private async Task<ContractBillingSnapshot> ResolveMonthlyBillingSnapshotAsync(
        CreateInvoiceDto dto,
        Contract contract,
        DateOnly periodStart,
        DateOnly periodEnd)
    {
        if (dto.ContractBillingSnapshotId is long snapshotId)
        {
            var snapshot = await _billingSnapshotRepository.GetByIdWithDetailsOrThrowAsync(snapshotId);
            ContractBillingSnapshotLifecycle.EnsureMatchesInvoiceRequest(snapshot, dto.ContractId, periodStart, periodEnd);
            if (snapshot.Status == ContractBillingSnapshotStatus.Finalized)
                throw new ValidationException("contract_billing_snapshot.already_finalized");

            if (snapshot.Status == ContractBillingSnapshotStatus.Draft)
            {
                var draftCalculation = ContractBillingSnapshotReader.ToCalculationDto(snapshot);
                ContractSlotInvoiceReadinessValidator.ValidateForTripsInvoice(
                    contract,
                    periodStart,
                    periodEnd,
                    draftCalculation);
            }

            return snapshot;
        }

        var calculation = await _calculateContractSlotChargesAction.ExecuteAsync(new CalculateContractSlotChargesDto
        {
            ContractId = dto.ContractId,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd
        });
        ContractSlotInvoiceReadinessValidator.ValidateForTripsInvoice(contract, periodStart, periodEnd, calculation);

        return ContractBillingSnapshotMaterializer.FromCalculation(
            dto.ContractId,
            contract.ContractType,
            periodStart,
            periodEnd,
            calculation,
            ContractBillingSnapshotStatus.Draft,
            ContractBillingSnapshotNameFormatter.BuildDefault(contract));
    }

    private static void ApplyExtraKmRoutingFromRequest(
        ContractBillingSnapshot snapshot,
        CreateInvoiceDto dto)
    {
        if (snapshot.ExtraKmsAmount <= 0m)
            return;

        if (snapshot.ExtraKmOverageInvoiceType is not null)
            return;

        var requestedRoute = dto.ExcludeExtraKmsFromMainInvoice
            ? InvoiceType.Claims
            : InvoiceType.Trips;
        ContractSlotChargeInvoiceRouting.LockExtraKmRoutingIfNeeded(snapshot, requestedRoute);
    }
}
