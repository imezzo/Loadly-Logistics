using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class DeactivateContractRequest
{
    public const string Permission = "deactivate_contract";

    [FromRoute]
    public long Id { get; set; }

    public DeactivationReason Reason { get; set; }
    /// <summary>Optional. Defaults to UTC now when not provided.</summary>
    public DateTime? EffectiveDate { get; set; }
    public string? Notes { get; set; }

    public DeactivateContractDto FromRequest() => new()
    {
        ContractId = Id,
        Reason = Reason,
        EffectiveDate = EffectiveDate ?? AppUtcTime.UtcNow(TimeProvider.System),
        Notes = Notes
    };
}

public class DeactivateContractRequestValidator : AbstractValidator<DeactivateContractRequest>
{
    public DeactivateContractRequestValidator()
    {
        RuleFor(x => x.Id)
            .IsRequiredId("Contract ID");

        RuleFor(x => x.Reason)
            .IsInEnum().WithMessage("validation.deactivation_reason_invalid");

    }
}
