using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;

namespace LoadlyAggregator.API.Requests.Admin.Invoices;

public class GetBankTransferRequest
{
    public const string Permission = "view_bank_transfer";

    public long Id { get; set; }

    public GetBankTransferDto FromRequest() => new()
    {
        Id = Id
    };
}

public class GetBankTransferRequestValidator : AbstractValidator<GetBankTransferRequest>
{
    public GetBankTransferRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Bank transfer ID");
    }
}
