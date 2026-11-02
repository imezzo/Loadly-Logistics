using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Admin.Invoices;

public class ListBankTransfersRequest : BasePagedRequest
{
    public const string Permission = "view_bank_transfer";

    public PaymentMethod? PaymentMethod { get; set; }
    public string? PayerType { get; set; }
    public List<long>? PayerIds { get; set; }

    public ListBankTransfersDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        PaymentMethod = PaymentMethod,
        PayerType = PayerType,
        PayerIds = PayerIds
    };
}

public class ListBankTransfersRequestValidator : AbstractValidator<ListBankTransfersRequest>
{
    public ListBankTransfersRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.PaymentMethod)
            .IsValidEnum<ListBankTransfersRequest, PaymentMethod>("PaymentMethod")
            .When(x => x.PaymentMethod.HasValue);

        When(x => !string.IsNullOrWhiteSpace(x.PayerType), () =>
        {
            RuleFor(x => x.PayerType)
                .Must(t => BankTransferPayerTypes.All.Contains(t!))
                .WithMessage("validation.invalid_payer_type");
        });

        When(x => x.PayerIds is { Count: > 0 }, () =>
        {
            RuleFor(x => x.PayerType)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("validation.payer_type_required")
                .Must(t => BankTransferPayerTypes.All.Contains(t!))
                .WithMessage("validation.invalid_payer_type");

            RuleForEach(x => x.PayerIds)
                .GreaterThan(0L).WithMessage("validation.entity_must_be_greater_than_zero|Payer ID");
        });
    }
}
