using LoadlyAggregator.Domain.Contractor.DTOs.Trips;

namespace LoadlyAggregator.API.Resources.Contractor.Trips;

public class ContractorTripInvitationStatisticsResource
{
    public int OpenInvitationTrips { get; set; }
    public int PendingBookingRequestTrips { get; set; }

    public static ContractorTripInvitationStatisticsResource FromDto(ContractorTripInvitationStatisticsDto dto) => new()
    {
        OpenInvitationTrips = dto.OpenInvitationTrips,
        PendingBookingRequestTrips = dto.PendingBookingRequestTrips
    };
}
