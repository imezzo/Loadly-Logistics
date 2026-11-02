using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class ReactivateContractRequest
{
    public const string Permission = "reactivate_contract";

    [FromRoute]
    public long Id { get; set; }

    public string? Notes { get; set; }

    public ReactivateContractDto FromRequest() => new()
    {
        ContractId = Id,
        Notes = Notes
    };
}

public class ReactivateContractRequestValidator : AbstractValidator<ReactivateContractRequest>
{
    public ReactivateContractRequestValidator()
    {
        RuleFor(x => x.Id)
            .IsRequiredId("Contract ID");
    }
}
