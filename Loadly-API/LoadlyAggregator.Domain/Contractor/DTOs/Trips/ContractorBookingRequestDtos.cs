using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Contractor.DTOs.Trips;

public class ContractorListBookingRequestsDto : BasePagedRequest
{
    public long ContractorId { get; set; }
    public TripBookingRequestStatus? Status { get; set; }
    public long? TripId { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public long? VehicleTypeId { get; set; }
}

public class ContractorGetBookingRequestDto
{
    public long Id { get; set; }
    public long ContractorId { get; set; }
}
