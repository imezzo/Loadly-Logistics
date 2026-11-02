using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class VehicleDetail : BaseEntity
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

    public LoadingMechanismType? LoadingMechanismType { get; set; }


    public virtual Vehicle Vehicle { get; set; } = null!;
}
