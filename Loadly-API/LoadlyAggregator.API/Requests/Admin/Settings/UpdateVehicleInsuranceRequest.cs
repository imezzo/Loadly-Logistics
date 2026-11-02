using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings;

public class UpdateVehicleInsuranceRequest
{
    public const string Permission = "edit_vehicle_insurance";

    [FromRoute]
    public long Id { get; set; }
    public string PolicyNo { get; set; } = string.Empty;
    public string InsuranceProvider { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }

    public UpdateVehicleInsuranceDto FromRequest() => new()
    {
        Id = Id,
        PolicyNo = PolicyNo,
        InsuranceProvider = InsuranceProvider,
        StartDate = StartDate,
        EndDate = EndDate,
        Amount = Amount
    };
}

public class UpdateVehicleInsuranceRequestValidator : AbstractValidator<UpdateVehicleInsuranceRequest>
{
    public UpdateVehicleInsuranceRequestValidator(IVehicleInsuranceRepository vehicleInsuranceRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle insurance ID");
        RuleFor(x => x.PolicyNo)
            .IsRequiredString(100, "Policy number")
            .IsUnique(async (policyNo, excludeId) =>
            {
                var existing = await vehicleInsuranceRepository.GetByPolicyNoAsync(policyNo);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "vehicle_insurances.policy_no_already_exists");
        RuleFor(x => x.Amount)
            .IsGreaterThanZero("Amount");
    }
}
