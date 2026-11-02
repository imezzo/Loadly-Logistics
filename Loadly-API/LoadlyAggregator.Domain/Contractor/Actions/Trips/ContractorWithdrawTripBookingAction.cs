using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Trips;

public class ContractorWithdrawTripBookingAction
{
    private readonly ITripBookingRequestRepository _bookingRequestRepository;

    public ContractorWithdrawTripBookingAction(ITripBookingRequestRepository bookingRequestRepository)
    {
        _bookingRequestRepository = bookingRequestRepository;
    }

    public async Task ExecuteAsync(ContractorWithdrawTripBookingDto dto)
    {
        var request = await _bookingRequestRepository.GetPendingByTripAndContractorAsync(dto.TripId, dto.ContractorId)
            ?? throw new NotFoundException(TripMessageKeys.BookingRequestNotFound);

        if (request.Status != TripBookingRequestStatus.Pending)
            throw new ValidationException(TripMessageKeys.BookingWithdrawOnlyPending);

        request.Status = TripBookingRequestStatus.Withdrawn;
        _bookingRequestRepository.Update(request);
        await _bookingRequestRepository.SaveChangesAsync();
    }
}
