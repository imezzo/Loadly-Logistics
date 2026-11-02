using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class ListVehicleBrandModelsDto : BasePagedRequest
{
    public long? VehicleBrandId { get; set; }
}
