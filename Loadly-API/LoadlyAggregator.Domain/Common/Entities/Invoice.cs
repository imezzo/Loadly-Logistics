using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public int? Sequence { get; set; }
    public InvoiceType InvoiceType { get; set; }
    public long? ContractId { get; set; }
    public long? LeadId { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateOnly? IssueDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal SlotChargesSubTotal { get; set; }
    public decimal TripsSubTotal { get; set; }
    public long? ContractBillingSnapshotId { get; set; }
    public DateTime? SlotChargesCalculatedAt { get; set; }
    public DateTime? TripChargesCalculatedAt { get; set; }
    public decimal SlotWorkingDaysAmount { get; set; }
    public decimal SlotExtraWorkingDaysAmount { get; set; }
    public decimal SlotExtraKmsAmount { get; set; }
    public decimal SlotExtraTripsAmount { get; set; }
    public int SlotWorkingDaysCount { get; set; }
    public int SlotExtraWorkingDaysCount { get; set; }
    public int SlotExtraTripsCount { get; set; }
    public decimal SlotExtraKmsCount { get; set; }
    public long? TaxId { get; set; }
    public long? DiscountId { get; set; }
    public TaxType? TaxType { get; set; }
    public decimal? TaxValue { get; set; }
    public decimal TaxAmount { get; set; }
    public DiscountType? DiscountType { get; set; }
    public decimal? DiscountValue { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? TaxedInvoiceAttachment { get; set; }
    public InvoiceStatus Status { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public long? AddedBy { get; set; }
    public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    public ICollection<Trip> ClaimInvoiceTrips { get; set; } = new List<Trip>();
    public virtual ICollection<InvoicePayment> Payments { get; set; } = new List<InvoicePayment>();
    public virtual InvoiceDetail? InvoiceDetail { get; set; }
    public virtual ContractBillingSnapshot? ContractBillingSnapshot { get; set; }
    public virtual Contract? Contract { get; set; }
    public virtual Lead? Lead { get; set; }
    public virtual Tax? Tax { get; set; }
    public virtual Discount? Discount { get; set; }
}
