using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ITripBookingRequestRepository : IRepository<TripBookingRequest>
{
    Task<TripBookingRequest> GetByIdOrThrowAsync(long id);
    Task<TripBookingRequest?> GetByTripAndContractorAsync(long tripId, long contractorId);
    Task<TripBookingRequest?> GetByRoundAndContractorAsync(long roundId, long contractorId);
    Task<TripBookingRequest?> GetPendingByTripAndContractorAsync(long tripId, long contractorId);
    Task<TripBookingRequest?> GetPendingByRoundAndContractorAsync(long roundId, long contractorId);
    Task<List<TripBookingRequest>> ListByTripIdAsync(long tripId);
    Task<List<TripBookingRequest>> ListPendingByTripIdAsync(long tripId);
    Task<(List<TripBookingRequest> Items, int TotalCount)> ListByContractorIdAsync(
        long contractorId,
        TripBookingRequestStatus? status,
        long? tripId,
        DateTime? startDateFrom,
        DateTime? startDateTo,
        long? vehicleTypeId,
        string? search,
        int page,
        int pageSize);
    Task RejectPendingByTripIdAsync(long tripId, long? reviewedByAdminId, DateTime reviewedAt, long? exceptRequestId = null);
}
