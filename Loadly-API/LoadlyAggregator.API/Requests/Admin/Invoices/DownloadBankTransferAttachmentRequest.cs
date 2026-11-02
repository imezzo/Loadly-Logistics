using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;

namespace LoadlyAggregator.API.Requests.Admin.Invoices;

public class DownloadBankTransferAttachmentRequest
{
    public const string Permission = "view_bank_transfer";

    public long Id { get; set; }

    public DownloadBankTransferAttachmentDto FromRequest() => new()
    {
        Id = Id
    };
}

public class DownloadBankTransferAttachmentRequestValidator : AbstractValidator<DownloadBankTransferAttachmentRequest>
{
    public DownloadBankTransferAttachmentRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Bank transfer ID");
    }
}
