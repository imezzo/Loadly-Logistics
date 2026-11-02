using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;

namespace LoadlyAggregator.API.Requests.Admin.Invoices;

public class DeleteBankTransferRequest
{
    public const string Permission = "delete_bank_transfer";

    public long Id { get; set; }

    public DeleteBankTransferDto FromRequest() => new()
    {
        Id = Id
    };
}

public class DeleteBankTransferRequestValidator : AbstractValidator<DeleteBankTransferRequest>
{
    public DeleteBankTransferRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Bank transfer ID");
    }
}
