using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings;

public class CreateVehicleInsuranceRequest
{
    public const string Permission = "create_vehicle_insurance";

    public string PolicyNo { get; set; } = string.Empty;
    public string InsuranceProvider { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }

    public CreateVehicleInsuranceDto FromRequest() => new()
    {
        PolicyNo = PolicyNo,
        InsuranceProvider = InsuranceProvider,
        StartDate = StartDate,
        EndDate = EndDate,
        Amount = Amount
    };
}

public class CreateVehicleInsuranceRequestValidator : AbstractValidator<CreateVehicleInsuranceRequest>
{
    public CreateVehicleInsuranceRequestValidator(IVehicleInsuranceRepository vehicleInsuranceRepository)
    {
        RuleFor(x => x.PolicyNo)
            .IsRequiredString(100, "Policy number")
            .IsUnique(async policyNo => await vehicleInsuranceRepository.GetByPolicyNoAsync(policyNo) == null, _ => "vehicle_insurances.policy_no_already_exists");
        RuleFor(x => x.InsuranceProvider)
            .IsRequiredString(255, "Insurance provider");
        RuleFor(x => x.StartDate)
            .IsRequiredDate("Start date");
        RuleFor(x => x.EndDate)
            .IsRequiredDate("End date")
            .IsAfterStartDate(x => x.StartDate);
        RuleFor(x => x.Amount)
            .IsGreaterThanZero("Amount");
    }
}
