using System.Text.Json.Serialization;
using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class CalculateContractDestinationCostsRequest
{
    public const string Permission = "view_contract";

    public ContractType ContractType { get; set; }

    [JsonPropertyName("monthly_working_days")]
    public int? MonthlyWorkingDays { get; set; }

    public List<CalculateContractDestinationItemRequest> Destinations { get; set; } = new();

    [JsonPropertyName("vehicle_types")]
    public List<CalculateContractVehicleTypeItemRequest> VehicleTypes { get; set; } = new();

    public CalculateContractDestinationCostsRequestDto FromRequest() => new()
    {
        ContractType = ContractType,
        MonthlyWorkingDays = MonthlyWorkingDays,
        Destinations = Destinations.Select(d => new CalculateContractDestinationItemDto
        {
            Kms = d.Kms,
            NumberOfDays = d.NumberOfDays,
            NameAr = d.NameAr,
            NameEn = d.NameEn
        }).ToList(),
        VehicleTypes = VehicleTypes.Select(v => new CalculateContractVehicleTypeInputDto
        {
            VehicleTypeId = v.VehicleTypeId,
            ConsumptionRate = v.ConsumptionRate,
            MonthlyRental = v.MonthlyRental,
            TripsPerMonth = v.TripsPerMonth
        }).ToList()
    };
}

public class CalculateContractDestinationItemRequest
{
    public decimal? Kms { get; set; }
    public int? NumberOfDays { get; set; }

    [JsonPropertyName("name_ar")]
    public string NameAr { get; set; } = string.Empty;

    [JsonPropertyName("name_en")]
    public string NameEn { get; set; } = string.Empty;
}

public class CalculateContractVehicleTypeItemRequest
{
    public long VehicleTypeId { get; set; }
    public decimal ConsumptionRate { get; set; }
    public decimal MonthlyRental { get; set; }
    public int? TripsPerMonth { get; set; }
}

public class CalculateContractDestinationCostsRequestValidator : AbstractValidator<CalculateContractDestinationCostsRequest>
{
    public CalculateContractDestinationCostsRequestValidator()
    {
        RuleFor(x => x.ContractType)
            .IsInEnum().WithMessage("validation.invalid_contract_type");

        RuleFor(x => x.MonthlyWorkingDays)
            .NotNull().WithMessage("validation.field_is_required|Monthly working days")
            .IsGreaterThanZero("Monthly working days")
            .When(x => x.ContractType is ContractType.Included or ContractType.Excluded);

        RuleFor(x => x.Destinations)
            .NotEmpty()
            .WithMessage("validation.at_least_one_destination_required");

        RuleForEach(x => x.Destinations).ChildRules(d =>
        {
            d.RuleFor(x => x.Kms)
                .GreaterThan(0)
                .When(x => x.Kms.HasValue)
                .WithMessage("validation.destination_kms_gt_zero");
            d.RuleFor(x => x.NumberOfDays)
                .InclusiveBetween(1, 366)
                .When(x => x.NumberOfDays.HasValue);
            d.RuleFor(x => x.NameAr).IsRequiredArabicName();
            d.RuleFor(x => x.NameEn).IsRequiredEnglishName();
        });

        RuleFor(x => x.VehicleTypes)
            .NotEmpty()
            .WithMessage("validation.at_least_one_vehicle_type_required");

        RuleForEach(x => x.VehicleTypes).ChildRules(v =>
        {
            v.RuleFor(x => x.VehicleTypeId)
                .GreaterThan(0)
                .WithMessage("validation.vehicle_type_id_gt_zero");
            v.RuleFor(x => x.ConsumptionRate)
                .GreaterThan(0)
                .WithMessage("validation.consumption_rate_gt_zero");
            v.RuleFor(x => x.MonthlyRental)
                .GreaterThanOrEqualTo(0)
                .WithMessage("validation.monthly_rental_gte_zero");
            v.RuleFor(x => x.TripsPerMonth)
                .GreaterThan(0)
                .When(x => x.TripsPerMonth.HasValue)
                .WithMessage("validation.trips_per_month_gt_zero");
        });
    }
}
