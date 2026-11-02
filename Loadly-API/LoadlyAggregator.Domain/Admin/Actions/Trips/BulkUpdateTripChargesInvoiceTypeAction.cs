using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Trips;

public class BulkUpdateTripChargesInvoiceTypeAction
{
    private readonly ITripRepository _tripRepository;
    private readonly ITripChargeRepository _tripChargeRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BulkUpdateTripChargesInvoiceTypeAction(
        ITripRepository tripRepository,
        ITripChargeRepository tripChargeRepository,
        IUnitOfWork unitOfWork)
    {
        _tripRepository = tripRepository;
        _tripChargeRepository = tripChargeRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(BulkUpdateTripChargesInvoiceTypeDto dto)
    {
        var tripIds = dto.TripIds.Distinct().ToList();

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var tripsById = (await _tripRepository.GetByIdsAsync(tripIds))
                .ToDictionary(t => t.Id);

            foreach (var tripId in tripIds)
            {
                if (!tripsById.TryGetValue(tripId, out Trip? trip))
                    throw new NotFoundException("trips.not_found_with_id", tripId);

                if (trip.Status != TripStatus.Completed)
                    throw new ValidationException(TripMessageKeys.TripChargesUpdateOnlyWhenCompleted);

                TripValidator.ValidateTripChargesNotInvoiced(trip);
            }

            await _tripChargeRepository.BulkUpdateInvoiceTypeByChargeTypeAsync(
                tripIds,
                dto.Charges.Select(c => (c.ChargeType, c.InvoiceType)).ToList());
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
