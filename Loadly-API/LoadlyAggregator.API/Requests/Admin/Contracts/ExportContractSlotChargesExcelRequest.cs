using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class ExportContractSlotChargesExcelRequest
{
    public const string Permission = CalculateContractSlotChargesRequest.Permission;

    public long ContractId { get; set; }
    public DateOnly PeriodStart { get; set; }
    public DateOnly PeriodEnd { get; set; }
    public long? ContractSlotId { get; set; }

    public ExportContractSlotChargesExcelDto FromRequest() => new()
    {
        ContractId = ContractId,
        ContractSlotId = ContractSlotId,
        PeriodStart = PeriodStart,
        PeriodEnd = PeriodEnd
    };
}

public class ExportContractSlotChargesExcelRequestValidator
    : AbstractValidator<ExportContractSlotChargesExcelRequest>
{
    public ExportContractSlotChargesExcelRequestValidator()
    {
        RuleFor(x => x.ContractId).IsRequiredId("Contract ID");
        RuleFor(x => x.PeriodStart).Must(date => date != default).WithMessage("validation.field_is_required|Period start");
        RuleFor(x => x.PeriodEnd).Must(date => date != default).WithMessage("validation.field_is_required|Period end");
        RuleFor(x => x.PeriodEnd)
            .GreaterThanOrEqualTo(x => x.PeriodStart)
            .WithMessage("validation.end_date_gte_start_date");
        RuleFor(x => x.ContractSlotId).IsOptionalId("Contract slot");
    }
}
