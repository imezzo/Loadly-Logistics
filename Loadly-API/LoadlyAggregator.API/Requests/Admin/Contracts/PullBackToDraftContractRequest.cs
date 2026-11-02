using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class PullBackToDraftContractRequest
{
    public const string Permission = "pull_back_to_draft_contract";

    [FromRoute]
    public long Id { get; set; }

    public PullBackToDraftContractDto FromRequest() => new()
    {
        ContractId = Id
    };
}

public class PullBackToDraftContractRequestValidator : AbstractValidator<PullBackToDraftContractRequest>
{
    public PullBackToDraftContractRequestValidator()
    {
        RuleFor(x => x.Id)
            .IsRequiredId("Contract ID");
    }
}
