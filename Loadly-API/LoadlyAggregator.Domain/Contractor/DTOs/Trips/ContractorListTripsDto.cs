using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Contractor.DTOs.Trips;

public class ContractorListTripsDto : BasePagedRequest
{
    public long ContractorId { get; set; }
    public long? VehicleTypeId { get; set; }
    public long? DriverId { get; set; }
    public TripStatus? Status { get; set; }
    public List<TripStatus>? Statuses { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public bool? OpenInviteOnly { get; set; }
    public bool? HasContractor { get; set; }
    public bool? IsContractorPaid { get; set; }
}
