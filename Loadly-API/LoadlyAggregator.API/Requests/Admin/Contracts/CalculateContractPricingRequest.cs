using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Contracts;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class CalculateContractPricingRequest
{
    public const string Permission = "calculate_contract_pricing";

    public long ContractId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }

    public CalculateContractPricingRequestDto FromRequest() => new()
    {
        ContractId = ContractId,
        FromDate = FromDate,
        ToDate = ToDate
    };
}

public class CalculateContractPricingRequestValidator : AbstractValidator<CalculateContractPricingRequest>
{
    public CalculateContractPricingRequestValidator()
    {
        RuleFor(x => x.ContractId)
            .IsRequiredId("Contract ID");
        RuleFor(x => x.ToDate)
            .IsGreaterThanOrEqualToDate(x => x.FromDate, "To date", "From date")
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue);
    }
}
