using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Invoices;

public class CreateInvoiceRequest
{
    public const string Permission = "create_invoice";

    public InvoiceType InvoiceType { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public List<long> TripIds { get; set; } = [];
    public long? ContractBillingSnapshotId { get; set; }
    public bool ExcludeExtraKmsFromMainInvoice { get; set; }
    public long ContractId { get; set; }
    public long? LeadId { get; set; }
    public long? TaxId { get; set; }
    public long? DiscountId { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly IssueDate { get; set; }
    public FileUpload? TaxedInvoiceAttachment { get; set; }
    public InvoiceStatus? Status { get; set; }
    public List<InvoicePaymentAllocationDto>? Payments { get; set; }

    public CreateInvoiceDto FromRequest() => new()
    {
        InvoiceType = InvoiceType,
        StartDate = StartDate,
        EndDate = EndDate,
        TripIds = TripIds,
        ContractBillingSnapshotId = ContractBillingSnapshotId,
        ExcludeExtraKmsFromMainInvoice = ExcludeExtraKmsFromMainInvoice,
        ContractId = ContractId,
        LeadId = LeadId,
        TaxId = TaxId,
        DiscountId = DiscountId,
        DueDate = DueDate,
        IssueDate = IssueDate,
        TaxedInvoiceAttachment = TaxedInvoiceAttachment,
        Status = Status,
        Payments = Payments
    };
}

public class CreateInvoiceRequestValidator : AbstractValidator<CreateInvoiceRequest>
{
    public CreateInvoiceRequestValidator()
    {
        RuleFor(x => x.StartDate).Must(date => date != default).WithMessage("validation.field_is_required|Start date");
        RuleFor(x => x.EndDate).Must(date => date != default).WithMessage("validation.field_is_required|End date");
        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .WithMessage("validation.end_date_gte_start_date");
        RuleFor(x => x.IssueDate).Must(date => date != default).WithMessage("validation.field_is_required|Issue date");
        RuleFor(x => x.DueDate).Must(date => date != default).WithMessage("validation.field_is_required|Due date");
        RuleFor(x => x.DueDate)
            .GreaterThanOrEqualTo(x => x.IssueDate)
            .WithMessage("validation.field_must_be_after_or_equal|Due date|Issue date");
        RuleFor(x => x.TripIds)
            .Must(tripIds => tripIds.Count > 0)
            .When(x => x.LeadId is > 0
                || (x.ContractBillingSnapshotId is not > 0 && x.InvoiceType != InvoiceType.Trips))
            .WithMessage("validation.collection_at_least_one|TripIds");
        RuleFor(x => x.ContractBillingSnapshotId).IsOptionalId("Contract billing snapshot ID");
        RuleForEach(x => x.TripIds)
            .IsRequiredId("Trip ID");
        RuleFor(x => x)
            .Must(x => x.ContractId > 0 ^ x.LeadId is > 0)
            .WithMessage("invoices.contract_or_lead_required_xor");
        RuleFor(x => x.ContractId)
            .IsRequiredId("Contract ID")
            .When(x => x.LeadId is not > 0);
        RuleFor(x => x.LeadId)
            .IsOptionalId("Lead ID")
            .When(x => x.ContractId <= 0);
        RuleFor(x => x.TaxId).IsOptionalId("Tax ID");
        RuleFor(x => x.DiscountId).IsOptionalId("Discount ID");
        RuleFor(x => x.Status)
            .IsValidEnum<CreateInvoiceRequest, InvoiceStatus>("Status")
            .When(x => x.Status.HasValue);
        RuleFor(x => x.Status)
            .NotEqual(InvoiceStatus.Cancelled)
            .When(x => x.Status.HasValue)
            .WithMessage("invoices.status_cancelled_not_allowed_here");
        RuleFor(x => x.Status)
            .NotNull()
            .When(x => x.Payments is null || !x.Payments.Any())
            .WithMessage("invoices.status_required_without_payments");
        this.ApplyInvoicePaymentRules(x => x.Payments, x => x.Status);
    }
}
