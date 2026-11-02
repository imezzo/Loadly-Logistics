using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.Actions.Drivers;
using LoadlyAggregator.Domain.Contractor.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Contractor.Actions.Trips;

public class ContractorChangeTripDriverAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripBookingRequestRepository _bookingRequestRepository;
    private readonly IContractorRepository _contractorRepository;
    private readonly IDriverRepository _driverRepository;
    private readonly ContractorCreateDriverAction _createDriverAction;

    public ContractorChangeTripDriverAction(
        ITripRepository tripRepository,
        ITripBookingRequestRepository bookingRequestRepository,
        IContractorRepository contractorRepository,
        IDriverRepository driverRepository,
        ContractorCreateDriverAction createDriverAction)
    {
        _tripRepository = tripRepository;
        _bookingRequestRepository = bookingRequestRepository;
        _contractorRepository = contractorRepository;
        _driverRepository = driverRepository;
        _createDriverAction = createDriverAction;
    }

    public async Task ExecuteAsync(ContractorChangeTripDriverDto dto)
    {
        var contractor = await _contractorRepository.GetByIdOrThrowAsync(dto.ContractorId);
        if (contractor.ContractorStatus != ContractorStatus.ACTIVE)
            throw new ValidationException("contractors.not_active");

        var trip = await _tripRepository.GetByIdOrThrowAsync(dto.TripId);
        if (trip.ContractorId != dto.ContractorId)
            throw new ForbiddenException(TripMessageKeys.NotAuthorized);

        var approvedBooking = (await _bookingRequestRepository.FindAsync(r =>
                r.TripId == dto.TripId
                && r.ContractorId == dto.ContractorId
                && r.Status == TripBookingRequestStatus.Approved))
            .FirstOrDefault();
        if (approvedBooking is null)
            throw new ValidationException(TripMessageKeys.BookingRequestNotApproved);

        if (trip.Status is TripStatus.InProgress or TripStatus.Completed or TripStatus.Cancelled)
            throw new ValidationException(TripMessageKeys.TripDriverCannotChangeWhenClosed);

        long driverId;
        if (dto.DriverId is > 0)
        {
            var driver = await _driverRepository.GetByIdAsync(dto.DriverId.Value)
                ?? throw new NotFoundException("drivers.not_found_with_id", dto.DriverId.Value);
            if (driver.ContractorId != dto.ContractorId)
                throw new ValidationException(TripMessageKeys.BookingDriverNotOwned);
            driverId = driver.Id;
        }
        else if (dto.Driver is not null)
        {
            dto.Driver.ContractorId = dto.ContractorId;
            driverId = await _createDriverAction.ExecuteAsync(dto.Driver);
        }
        else
        {
            throw new ValidationException(TripMessageKeys.BookingDriverRequired);
        }

        trip.DriverId = driverId;
        TripUpdateHelper.ApplyStatus(
            trip,
            trip.ContractorId,
            driverId,
            trip.VehicleId,
            trip.StartDate ?? default,
            trip.EndDate ?? default);
        _tripRepository.Update(trip);
        await _tripRepository.SaveChangesAsync();
    }
}
