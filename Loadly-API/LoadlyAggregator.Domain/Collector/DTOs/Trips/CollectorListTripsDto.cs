using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Collector.DTOs.Trips;

public class CollectorListTripsDto : BasePagedRequest
{
    public long? ContractId { get; set; }
    public string? ContractReferenceNumber { get; set; }
    public long? CompanyId { get; set; }
    public string? CompanyName { get; set; }
    public string? DriverName { get; set; }
    public string? Plate { get; set; }
    public long? VehicleId { get; set; }
    public long? VehicleTypeId { get; set; }
    public long? DriverId { get; set; }
    public List<long>? ContractorIds { get; set; }
    public bool? HasContractor { get; set; }
    public TripStatus? Status { get; set; }
    public List<TripDocumentStatus>? TripDocumentStatuses { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
}
