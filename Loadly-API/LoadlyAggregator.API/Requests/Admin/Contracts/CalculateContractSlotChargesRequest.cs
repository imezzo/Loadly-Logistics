using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class CalculateContractSlotChargesRequest
{
    public const string Permission = "view_contract";

    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public long? ContractSlotId { get; set; }

    public CalculateContractSlotChargesDto FromRequest(long contractId) => new()
    {
        ContractId = contractId,
        ContractSlotId = ContractSlotId,
        PeriodStart = PeriodStart,
        PeriodEnd = PeriodEnd
    };
}

public class CalculateContractSlotChargesRequestValidator
    : AbstractValidator<CalculateContractSlotChargesRequest>
{
    public CalculateContractSlotChargesRequestValidator()
    {
        RuleFor(x => x.PeriodStart).Must(date => date != default).WithMessage("validation.field_is_required|Period start");
        RuleFor(x => x.PeriodEnd).Must(date => date != default).WithMessage("validation.field_is_required|Period end");
        RuleFor(x => x.PeriodEnd)
            .GreaterThanOrEqualTo(x => x.PeriodStart)
            .WithMessage("validation.end_date_gte_start_date");
        RuleFor(x => x.ContractSlotId).IsOptionalId("Contract slot");
    }
}
