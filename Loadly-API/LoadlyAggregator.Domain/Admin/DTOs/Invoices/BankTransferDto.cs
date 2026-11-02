using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.Domain.Admin.DTOs.Invoices;

public class BankTransferDto
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime? IssueDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ExternalPaymentReferenceNumber { get; set; }
    public string? Attachment { get; set; }
    public string? FileUrl { get; set; }
    public long? PayerId { get; set; }
    public string? PayerType { get; set; }
    public CompanyRelationDto? Company { get; set; }
    public LeadRelationDto? Lead { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int InvoicePaymentCount { get; set; }
    public decimal AllocatedAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string DisplayLabelEn { get; set; } = string.Empty;
    public string DisplayLabelAr { get; set; } = string.Empty;
    public List<InvoicePaymentRelationDto> InvoicePayments { get; set; } = new();
}

public class CreateBankTransferDto
{
    public decimal Amount { get; set; }
    public DateTime? IssueDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ExternalPaymentReferenceNumber { get; set; }
    public long PayerId { get; set; }
    public string PayerType { get; set; } = string.Empty;
    public FileUpload? Attachment { get; set; }
}

public class UpdateBankTransferDto
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime? IssueDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ExternalPaymentReferenceNumber { get; set; }
    public long PayerId { get; set; }
    public string PayerType { get; set; } = string.Empty;
    public FileUpload? Attachment { get; set; }
}

public class GetBankTransferDto
{
    public long Id { get; set; }
}

public class ListBankTransfersDto : BasePagedRequest
{
    public PaymentMethod? PaymentMethod { get; set; }
    public string? PayerType { get; set; }
    public List<long>? PayerIds { get; set; }
}

public class DeleteBankTransferDto
{
    public long Id { get; set; }
}

public class DownloadBankTransferAttachmentDto
{
    public long Id { get; set; }
}
