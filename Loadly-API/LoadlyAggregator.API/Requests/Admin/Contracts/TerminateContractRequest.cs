using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class TerminateContractRequest
{
    public const string Permission = "terminate_contract";

    [FromRoute]
    public long Id { get; set; }

    public string? Notes { get; set; }

    public TerminateContractDto FromRequest() => new()
    {
        ContractId = Id,
        Notes = Notes
    };
}

public class TerminateContractRequestValidator : AbstractValidator<TerminateContractRequest>
{
    public TerminateContractRequestValidator()
    {
        RuleFor(x => x.Id)
            .IsRequiredId("Contract ID");
    }
}
