using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Vehicles;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.API.Requests.Admin.Vehicles;

public class UpdateVehicleRequest
{
    public const string Permission = "edit_vehicle";

    [FromRoute]
    public long Id { get; set; }
    public string PlateNo { get; set; } = string.Empty;
    public string PlateLetters { get; set; } = string.Empty;
    public int? MakeYear { get; set; }
    public long? InsuranceId { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public decimal? InitialOdometerReading { get; set; }
    public string? VinNo { get; set; }
    public long? ColorId { get; set; }
    public long? CapacityUnitId { get; set; }
    public decimal? TankCapacity { get; set; }
    public string? VehicleTypeSettings { get; set; }
    public decimal? Capacity { get; set; }
    public long? FuelTypeId { get; set; }
    public string? GovernorateCode { get; set; }
    public string? InsuranceDetails { get; set; }
    public string? TemperatureRangeSettng { get; set; }
    public string? CoolingUnitSpecification { get; set; }
    public string? TemperatureMonotoringCapability { get; set; }
    public double? MaximumLoadHeight { get; set; }
    public double? InternalHeight { get; set; }
    public double? InternalWidth { get; set; }
    public double? InternalLength { get; set; }
    public long? BrandModelID { get; set; }
    public long? ContractorId { get; set; }
    public long? SectorId { get; set; }
    public long VehicleTypeId { get; set; }

    public long? HeadVehicleId { get; set; }
    public List<long> TailVehicleIds { get; set; } = new List<long>() { };
    public LoadingMechanismType? LoadingMechanismType { get; set; }
    public List<UpdateVehicleDocumentItem>? Documents { get; set; }
    public VehicleStatus? Status { get; set; }

    public UpdateVehicleDto FromRequest() => new()
    {
        Id = Id,
        PlateNo = PlateNo,
        PlateLetters = PlateLetters,
        MakeYear = MakeYear,
        InsuranceId = InsuranceId,
        PurchaseDate = PurchaseDate,
        InitialOdometerReading = InitialOdometerReading,
        VinNo = VinNo,
        ColorId = ColorId,
        CapacityUnitId = CapacityUnitId,
        TankCapacity = TankCapacity,
        VehicleTypeSettings = VehicleTypeSettings,
        Capacity = Capacity,
        FuelTypeId = FuelTypeId,
        GovernorateCode = GovernorateCode,
        InsuranceDetails = InsuranceDetails,
        TemperatureRangeSettng = TemperatureRangeSettng,
        CoolingUnitSpecification = CoolingUnitSpecification,
        TemperatureMonotoringCapability = TemperatureMonotoringCapability,
        MaximumLoadHeight = MaximumLoadHeight,
        InternalHeight = InternalHeight,
        InternalWidth = InternalWidth,
        InternalLength = InternalLength,
        BrandModelID = BrandModelID,
        VehicleTypeId = VehicleTypeId,
        HeadVehicleId = HeadVehicleId,
        TailVehicleIds = TailVehicleIds,
        ContractorId = ContractorId,
        SectorId = SectorId,
        LoadingMechanismType = LoadingMechanismType,
        Documents = Documents?.Select(d => new UpdateVehicleDocumentItemDto
        {
            Id = d.Id,
            DocumentId = d.DocumentId,
            File = d.File,
            ExpiryDate = d.ExpiryDate,
            Status = d.Status
        }).ToList(),
        Status = Status.HasValue ? (int)Status.Value : null
    };
}

public class UpdateVehicleDocumentItem
{
    public long? Id { get; set; }
    public long DocumentId { get; set; }
    public FileUpload? File { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public int Status { get; set; }
}

public class UpdateVehicleRequestValidator : AbstractValidator<UpdateVehicleRequest>
{
    public UpdateVehicleRequestValidator(IVehicleRepository vehicleRepository, IFuelTypeRepository fuelTypeRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Vehicle ID");

        RuleFor(x => x.PlateNo)
            .IsRequiredString(4, 3, "Plate number")
            .Matches(@"^\d+$").WithMessage("validation.plate_number_digits_only");

        RuleFor(x => x.PlateLetters)
         .IsRequiredString(5, "Plate letters")
         .Matches(@"^([A-Za-z\u0621-\u064A]\s){1,2}[A-Za-z\u0621-\u064A]$")
         .WithMessage("validation.plate_letters_format");

        RuleFor(x => x.MakeYear)
            .IsInclusiveRange(1900, AppUtcTime.UtcCalendarYear(TimeProvider.System) + 1, "Make year")
            .When(x => x.MakeYear.HasValue);
        RuleFor(x => x.VinNo)
            .MustAsync(async (request, vinNo, cancellationToken) =>
            {
                if (string.IsNullOrWhiteSpace(vinNo))
                    return true;
                var existing = await vehicleRepository.GetByVinNoAsync(vinNo);
                return existing == null || existing.Id == request.Id;
            })
            .WithMessage((request, vinNo) => $"validation.vehicle_vin_already_exists|{vinNo}")
            .When(x => !string.IsNullOrWhiteSpace(x.VinNo));
        RuleFor(x => x.TankCapacity)
            .IsGreaterThanZero("Tank capacity");
        RuleFor(x => x.Capacity)
            .IsGreaterThanZero("Capacity");
        RuleFor(x => x.PurchaseDate)
           .IsPastDate("Purchase date");
        RuleFor(x => x.VehicleTypeId)
            .IsRequiredId("Vehicle type ID");
        RuleFor(x => x.ContractorId)
            .IsOptionalId("Contractor ID")
            .When(x => x.ContractorId.HasValue);
        RuleFor(x => x.SectorId)
            .IsOptionalId("Sector ID")
            .When(x => x.SectorId.HasValue);
        RuleFor(x => x.TailVehicleIds)
            .Must(ids => ids.Distinct().Count() <= 1)
            .WithMessage("vehicles.head_vehicle_max_tail_exceeded");
    }
}
