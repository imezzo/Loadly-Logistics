using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Company.DTOs.Trips;

public class CompanyListTripsDto : BasePagedRequest
{
    public long CompanyId { get; set; }
    public long? ContractId { get; set; }
    public long? VehicleId { get; set; }
    public long? VehicleTypeId { get; set; }
    public long? DriverId { get; set; }
    public TripStatus? Status { get; set; }
    public OperationalDriverStatus? CurrentDriverStatus { get; set; }
    public List<TripDocumentStatus>? TripDocumentStatuses { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
}
