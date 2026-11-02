using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Resources.Admin.Vehicles;

public class VehicleDetailResource
{
    public long VehicleId { get; set; }
    public string? GovernorateCode { get; set; }
    public string? InsuranceDetails { get; set; }
    public string? TemperatureRangeSettng { get; set; }
    public string? CoolingUnitSpecification { get; set; }
    public string? TemperatureMonotoringCapability { get; set; }
    public decimal? MaximumLoadHeight { get; set; }
    public decimal? InternalHeight { get; set; }
    public decimal? InternalWidth { get; set; }
    public decimal? InternalLength { get; set; }
    public int? LoadingMechanismType { get; set; }
    public static VehicleDetailResource FromDto(VehicleDetailDto dto) =>
        new VehicleDetailResource
        {
            VehicleId = dto.VehicleId,
            GovernorateCode = dto.GovernorateCode,
            InsuranceDetails = dto.InsuranceDetails,
            TemperatureRangeSettng = dto.TemperatureRangeSettng,
            CoolingUnitSpecification = dto.CoolingUnitSpecification,
            TemperatureMonotoringCapability = dto.TemperatureMonotoringCapability,
            MaximumLoadHeight = dto.MaximumLoadHeight,
            InternalHeight = dto.InternalHeight,
            InternalWidth = dto.InternalWidth,
            InternalLength = dto.InternalLength,
            LoadingMechanismType = dto.LoadingMechanismType.HasValue ? (int)dto.LoadingMechanismType.Value : null
        };
}
