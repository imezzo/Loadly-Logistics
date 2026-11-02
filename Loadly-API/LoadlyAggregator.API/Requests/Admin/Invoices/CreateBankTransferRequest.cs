using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Admin.Invoices;

public class CreateBankTransferRequest
{
    public const string Permission = "create_bank_transfer";

    public decimal Amount { get; set; }
    public DateTime? IssueDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? ExternalPaymentReferenceNumber { get; set; }
    public long PayerId { get; set; }
    public string PayerType { get; set; } = string.Empty;
    public FileUpload? Attachment { get; set; }

    public CreateBankTransferDto FromRequest() => new()
    {
        Amount = Amount,
        IssueDate = IssueDate,
        PaymentMethod = PaymentMethod,
        ExternalPaymentReferenceNumber = ExternalPaymentReferenceNumber,
        PayerId = PayerId,
        PayerType = PayerType,
        Attachment = Attachment
    };
}

public class CreateBankTransferRequestValidator : AbstractValidator<CreateBankTransferRequest>
{
    public CreateBankTransferRequestValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("validation.amount_gt_zero");
        RuleFor(x => x.IssueDate).IsPastOrPresent("Issue date");
        RuleFor(x => x.PaymentMethod).IsInEnum().WithMessage("validation.invalid_payment_method");
        RuleFor(x => x.PayerId).GreaterThan(0).WithMessage("validation.payer_id_required");
        RuleFor(x => x.PayerType)
            .NotEmpty().WithMessage("validation.payer_type_required")
            .Must(t => BankTransferPayerTypes.All.Contains(t))
            .WithMessage("validation.invalid_payer_type");
        RuleFor(x => x.ExternalPaymentReferenceNumber)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.ExternalPaymentReferenceNumber));
        RuleFor(x => x.Attachment)
            .IsValidFileType(DocumentFileTypeValidator.AllowedMimeTypes, "Attachment type is not allowed. Allowed types: images and documents.")
            .When(x => x.Attachment != null && x.Attachment.Length > 0);
    }
}
