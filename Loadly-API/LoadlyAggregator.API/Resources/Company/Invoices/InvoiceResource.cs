using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Company.DTOs.Invoices;
using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.API.Resources.Company.Invoices;

public class InvoiceResource
{
    public long Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int InvoiceType { get; set; }
    public string InvoiceTypeName { get; set; } = string.Empty;
    public long ContractId { get; set; }
    public ContractRelationResource? Contract { get; set; }
    public long? LeadId { get; set; }
    public LeadRelationResource? Lead { get; set; }
    public long CompanyId { get; set; }
    public CompanyRelationResource? Company { get; set; }
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
    public int Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int TripCount { get; set; }
    public string? ContractReferenceNumber { get; set; }
    public InvoiceTaxRelationResource? Tax { get; set; }
    public InvoiceDiscountRelationResource? Discount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public bool IsOverdue { get; set; }
    public int? DaysOverdue { get; set; }
    public List<InvoicePaymentRelationResource> InvoicePayments { get; set; } = new();
    public List<TripRelationResource> Trips { get; set; } = new();

    public static InvoiceResource FromDto(InvoiceDto dto)
    {
        var resource = new InvoiceResource
        {
            Id = dto.Id,
            InvoiceNumber = dto.InvoiceNumber,
            InvoiceType = (int)dto.InvoiceType,
            InvoiceTypeName = dto.InvoiceType.ToString(),
            ContractId = dto.ContractId,
            LeadId = dto.LeadId,
            CompanyId = dto.CompanyId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IssueDate = dto.IssueDate,
            DueDate = dto.DueDate,
            SubTotal = dto.SubTotal,
            TripsSubTotal = dto.TripsSubTotal,
            TaxAmount = dto.TaxAmount,
            DiscountAmount = dto.DiscountAmount,
            TotalAmount = dto.TotalAmount,
            TaxedInvoiceAttachment = dto.TaxedInvoiceAttachment,
            FileUrl = dto.FileUrl,
            Status = (int)dto.Status,
            StatusName = dto.Status.ToString(),
            CancelledAt = dto.CancelledAt,
            CancellationReason = dto.CancellationReason,
            AddedBy = dto.AddedBy,
            CreatedAt = dto.CreatedAt,
            CreatedDate = dto.CreatedDate,
            UpdatedAt = dto.UpdatedAt,
            TripCount = dto.TripCount,
            ContractReferenceNumber = dto.ContractReferenceNumber,
            PaidAmount = dto.PaidAmount,
            RemainingAmount = dto.RemainingAmount,
            IsOverdue = dto.IsOverdue,
            DaysOverdue = dto.DaysOverdue
        };
        if (dto.Contract != null)
        {
            resource.Contract = new ContractRelationResource
            {
                Id = dto.Contract.Id,
                ReferenceNumber = dto.Contract.ReferenceNumber,
                NameEn = dto.Contract.NameEn,
                NameAr = dto.Contract.NameAr,
                ContractType = (int)dto.Contract.ContractType,
                ContractTypeName = dto.Contract.ContractType.ToLocalizedDisplay(),
                ConsumptionType = (int)dto.Contract.ConsumptionType
            };
        }
        if (dto.Lead != null)
        {
            resource.Lead = new LeadRelationResource
            {
                Id = dto.Lead.Id,
                FullName = dto.Lead.FullName,
                Phone = dto.Lead.Phone,
                Email = dto.Lead.Email
            };
        }
        if (dto.Company != null)
        {
            resource.Company = new CompanyRelationResource
            {
                Id = dto.Company.Id,
                NameEn = dto.Company.NameEn,
                NameAr = dto.Company.NameAr,
                Phone = dto.Company.Phone,
                Email = dto.Company.Email,
                RegistrationNumber = dto.Company.RegistrationNumber,
                FullName = dto.Company.FullName
            };
        }
        if (dto.Tax != null)
        {
            resource.Tax = new InvoiceTaxRelationResource
            {
                Id = dto.Tax.Id,
                NameEn = dto.Tax.NameEn,
                NameAr = dto.Tax.NameAr,
                Value = dto.Tax.Value,
                Type = dto.Tax.Type
            };
        }
        if (dto.Discount != null)
        {
            resource.Discount = new InvoiceDiscountRelationResource
            {
                Id = dto.Discount.Id,
                NameEn = dto.Discount.NameEn,
                NameAr = dto.Discount.NameAr,
                Value = dto.Discount.Value,
                Type = dto.Discount.Type
            };
        }
        if (dto.InvoicePayments != null)
        {
            resource.InvoicePayments = dto.InvoicePayments.Select(p => new InvoicePaymentRelationResource
            {
                Id = p.Id,
                InvoiceId = p.InvoiceId,
                BankTransferId = p.BankTransferId,
                Amount = p.Amount,
                AddedBy = p.AddedBy,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            }).ToList();
        }
        if (dto.Trips != null)
            resource.Trips = dto.Trips.Select(TripRelationResource.FromDto).ToList();
        return resource;
    }
}

public class InvoiceTaxRelationResource
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public decimal Value { get; set; }
    public TaxType Type { get; set; }
}

public class InvoiceDiscountRelationResource
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public decimal Value { get; set; }
    public DiscountType Type { get; set; }
}
