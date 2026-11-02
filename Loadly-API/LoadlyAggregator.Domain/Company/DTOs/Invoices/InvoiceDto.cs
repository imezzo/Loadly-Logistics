using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Company.DTOs.Invoices;

public class InvoiceDto
{
    public long Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public InvoiceType InvoiceType { get; set; }
    public long ContractId { get; set; }
    public ContractRelationDto? Contract { get; set; }
    public long? LeadId { get; set; }
    public LeadRelationDto? Lead { get; set; }
    public long CompanyId { get; set; }
    public CompanyRelationDto? Company { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateOnly? IssueDate { get; set; }
    public DateOnly? DueDate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal TripsSubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? TaxedInvoiceAttachment { get; set; }
    public string? FileUrl { get; set; }
    public InvoiceStatus Status { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int TripCount { get; set; }
    public string? ContractReferenceNumber { get; set; }
    public InvoiceTaxRelationDto? Tax { get; set; }
    public InvoiceDiscountRelationDto? Discount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public List<InvoicePaymentRelationDto> InvoicePayments { get; set; } = new();
    public List<TripRelationDto> Trips { get; set; } = new();

    public bool IsOverdue => DueDate is { } due
        && due < EgyptTime.ToCalendarDate(DateTime.UtcNow)
        && Status is not (InvoiceStatus.Paid or InvoiceStatus.Cancelled);

    public int? DaysOverdue => IsOverdue && DueDate is { } due
        ? EgyptTime.ToCalendarDate(DateTime.UtcNow).DayNumber - due.DayNumber
        : null;
}

public class InvoiceTaxRelationDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public decimal Value { get; set; }
    public TaxType Type { get; set; }
}

public class InvoiceDiscountRelationDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public decimal Value { get; set; }
    public DiscountType Type { get; set; }
}
