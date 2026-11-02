using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilities;

public class ListVehicleFacilitiesDto : BasePagedRequest
{
    public VehicleFacilityStatus? Status { get; set; }
    public VehicleFacilityType? FacilityType { get; set; }
    public List<long>? CompanyIds { get; set; }
}
