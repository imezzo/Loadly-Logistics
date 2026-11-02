using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Shared.Time;

namespace LoadlyAggregator.Domain.Admin.Helpers;

internal static class ContractSlotPersistenceHelper
{
    public static void EnsureUniqueVehicleAssignments(IReadOnlyList<ContractSlotItemDto> slots)
    {
        var seen = new HashSet<long>();
        foreach (var slot in slots)
        {
            if (slot.Status != ContractSlotStatus.ACTIVE)
                continue;
            if (slot.VehicleId is not { } vehicleId)
                continue;
            if (!seen.Add(vehicleId))
                throw new LogicalException("contracts.duplicate_slot_vehicle");
        }
    }

    public static bool ShouldReleaseVehicle(long? previousVehicleId, ContractSlotItemDto item) =>
        previousVehicleId.HasValue
        && (item.Status == ContractSlotStatus.ARCHIVED || previousVehicleId != item.VehicleId);

    public static async Task ReleaseVehicleFromContractAsync(
        long contractId,
        long? vehicleId,
        IVehicleRepository vehicleRepository)
    {
        if (vehicleId is not { } id)
            return;

        var vehicle = await vehicleRepository.GetByIdAsync(id);
        if (vehicle?.ContractId == contractId)
        {
            vehicle.ContractId = null;
            vehicleRepository.Update(vehicle);
        }
    }

    public static async Task ArchiveActiveSlotForVehicleAsync(
        long vehicleId,
        IContractSlotRepository contractSlotRepository)
    {
        var activeSlot = await contractSlotRepository.GetActiveByVehicleIdAsync(vehicleId);
        if (activeSlot is null)
            return;

        MarkArchived(activeSlot);
        contractSlotRepository.Update(activeSlot);
    }

    public static async Task AssignVehicleToContractAsync(
        long contractId,
        ContractSlotItemDto item,
        IVehicleRepository vehicleRepository,
        IContractSlotRepository contractSlotRepository,
        IReadOnlyList<ContractSlotItemDto>? batchItems = null,
        IReadOnlySet<long>? archivedSlotIds = null)
    {
        if (item.Status != ContractSlotStatus.ACTIVE || item.VehicleId is not { } vehicleId)
            return;

        var activeSlot = await contractSlotRepository.GetActiveByVehicleIdAsync(vehicleId, item.Id);
        if (activeSlot is not null
            && !IsVehicleBeingReleasedInBatch(activeSlot, batchItems)
            && archivedSlotIds?.Contains(activeSlot.Id) != true)
        {
            if (activeSlot.ContractId != contractId)
                throw new LogicalException("contracts.vehicle_already_assigned");
            throw new LogicalException("contracts.duplicate_slot_vehicle");
        }

        var vehicle = await vehicleRepository.GetByIdOrThrowAsync(vehicleId);
        if (vehicle.ContractId.HasValue && vehicle.ContractId != contractId)
            throw new LogicalException("contracts.vehicle_already_assigned");
        vehicle.ContractId = contractId;
        vehicleRepository.Update(vehicle);
    }

    private static bool IsVehicleBeingReleasedInBatch(
        ContractSlot activeSlot,
        IReadOnlyList<ContractSlotItemDto>? batchItems)
    {
        if (batchItems is null)
            return false;

        var item = batchItems.FirstOrDefault(x => x.Id == activeSlot.Id);
        return item is not null
            && (item.Status == ContractSlotStatus.ARCHIVED || item.VehicleId != activeSlot.VehicleId);
    }

    public static void MarkArchived(ContractSlot entity, DateTime? archivedAt = null)
    {
        if (entity.Status == ContractSlotStatus.ARCHIVED)
            return;

        entity.Status = ContractSlotStatus.ARCHIVED;
        entity.ArchivedAt = ContractSlotArchiveHelper.ResolveArchivedAt(entity, archivedAt);
    }

    public static ContractSlot Create(
        ContractSlotItemDto item,
        long contractId,
        long? addedBy,
        IReadOnlyDictionary<long, ContractSlot> existingById,
        IReadOnlyList<ContractSlotItemDto> batchItems)
    {
        ChainToUnreplacedArchivedVehicleSlot(item, existingById, batchItems);
        ValidateReplacedSlotReference(item, contractId, existingById, batchItems);
        return ApplyFields(new ContractSlot { ContractId = contractId }, item, addedBy, isNew: true);
    }

    /// <summary>
    /// A new slot that reuses a vehicle from an archived slot continues that slot's chain when no slot
    /// already points at it through <see cref="ContractSlot.ReplacedSlotId"/>.
    /// </summary>
    private static void ChainToUnreplacedArchivedVehicleSlot(
        ContractSlotItemDto item,
        IReadOnlyDictionary<long, ContractSlot> existingById,
        IReadOnlyList<ContractSlotItemDto> batchItems)
    {
        if (item.ReplacedSlotId.HasValue
            || item.Status != ContractSlotStatus.ACTIVE
            || item.VehicleId is not long vehicleId)
        {
            return;
        }

        var predecessor = existingById.Values
            .Where(slot => slot.VehicleId == vehicleId && IsArchivedSlot(slot, batchItems))
            .OrderByDescending(slot => ResolveArchivedAt(slot, batchItems))
            .ThenByDescending(slot => slot.Id)
            .FirstOrDefault();

        if (predecessor is null || HasReplacement(predecessor.Id, existingById, batchItems))
        {
            return;
        }

        item.ReplacedSlotId = predecessor.Id;
    }

    private static bool IsArchivedSlot(ContractSlot slot, IReadOnlyList<ContractSlotItemDto> batchItems)
    {
        var batchItem = batchItems.FirstOrDefault(item => item.Id == slot.Id);
        if (batchItem is not null)
            return batchItem.Status == ContractSlotStatus.ARCHIVED;

        return slot.Status == ContractSlotStatus.ARCHIVED;
    }

    private static DateTime ResolveArchivedAt(ContractSlot slot, IReadOnlyList<ContractSlotItemDto> batchItems)
    {
        if (slot.ArchivedAt.HasValue)
            return slot.ArchivedAt.Value;

        var batchItem = batchItems.FirstOrDefault(item => item.Id == slot.Id);
        return batchItem?.ArchivedAt ?? DateTime.MinValue;
    }

    private static bool HasReplacement(
        long slotId,
        IReadOnlyDictionary<long, ContractSlot> existingById,
        IReadOnlyList<ContractSlotItemDto> batchItems) =>
        existingById.Values.Any(slot => slot.ReplacedSlotId == slotId)
        || batchItems.Any(item => item.ReplacedSlotId == slotId);

    public static void ValidateReplacedSlotReference(
        ContractSlotItemDto item,
        long contractId,
        IReadOnlyDictionary<long, ContractSlot> existingById,
        IReadOnlyList<ContractSlotItemDto> batchItems)
    {
        if (item.ReplacedSlotId is not long replacedSlotId)
            return;

        var batchItem = batchItems.FirstOrDefault(x => x.Id == replacedSlotId);
        if (batchItem is not null)
        {
            if (batchItem.Status != ContractSlotStatus.ARCHIVED)
                throw new ValidationException("contracts.replaced_slot_must_be_archived");

            return;
        }

        if (!existingById.TryGetValue(replacedSlotId, out var replacedSlot) || replacedSlot.ContractId != contractId)
            throw new NotFoundException("contracts.replaced_slot_not_found", replacedSlotId);

        if (replacedSlot.Status != ContractSlotStatus.ARCHIVED)
            throw new ValidationException("contracts.replaced_slot_must_be_archived");
    }

    public static async Task UpdateAsync(
        ContractSlot entity,
        ContractSlotItemDto item,
        long contractId,
        long? addedBy,
        ITripRepository tripRepository,
        IInvoiceRepository invoiceRepository)
    {
        await ValidateSlotUpdateAsync(entity, item, contractId, tripRepository, invoiceRepository);
        ApplyFields(entity, item, addedBy, isNew: false);
    }

    private static async Task ValidateSlotUpdateAsync(
        ContractSlot entity,
        ContractSlotItemDto item,
        long contractId,
        ITripRepository tripRepository,
        IInvoiceRepository invoiceRepository)
    {
        ValidateSlotUpdate(entity, item);
        await ValidateCommercialTermsUpdateAsync(entity, item, contractId, tripRepository, invoiceRepository);

        if (entity.AssignedAt == item.AssignedAt)
            return;

        if (await tripRepository.HasCompletedTripsForContractSlotAsync(entity.Id))
            throw new ValidationException("contracts.slot_assigned_at_locked_completed_trips");

        var affectedStart = entity.AssignedAt < item.AssignedAt ? entity.AssignedAt : item.AssignedAt;
        var affectedEnd = entity.AssignedAt > item.AssignedAt ? entity.AssignedAt : item.AssignedAt;
        if (await invoiceRepository.HasOverlappingTripsInvoiceForContractAsync(
                contractId,
                EgyptTime.ToCalendarDate(affectedStart),
                EgyptTime.ToCalendarDate(affectedEnd)))
        {
            throw new ValidationException("contracts.slot_assigned_at_locked_invoiced_period");
        }
    }

    private static async Task ValidateCommercialTermsUpdateAsync(
        ContractSlot entity,
        ContractSlotItemDto item,
        long contractId,
        ITripRepository tripRepository,
        IInvoiceRepository invoiceRepository)
    {
        if (!HasBillingCommercialTermsChanged(entity, item))
            return;

        if (entity.Status == ContractSlotStatus.ARCHIVED)
            throw new ValidationException("contracts.archived_slot_commercial_terms_immutable");

        if (await tripRepository.HasCompletedTripsForContractSlotAsync(entity.Id))
            throw new ValidationException("contracts.slot_commercial_terms_change_requires_new_slot");

        if (await invoiceRepository.HasOverlappingTripsInvoiceForContractAsync(
                contractId,
                EgyptTime.ToCalendarDate(entity.AssignedAt),
                EgyptTime.ToCalendarDate(DateTime.UtcNow)))
        {
            throw new ValidationException("contracts.slot_commercial_terms_locked_invoiced_period");
        }
    }

    private static bool HasBillingCommercialTermsChanged(ContractSlot entity, ContractSlotItemDto item) =>
        entity.DailyValue != item.DailyValue
        || entity.ExtraWorkingDayFees != item.ExtraWorkingDayFees
        || entity.MaxKms != item.MaxKms
        || entity.OverConsumption != item.OverConsumption
        || entity.MonthlyRental != item.MonthlyRental;

    private static void ValidateSlotUpdate(ContractSlot entity, ContractSlotItemDto item)
    {
        if (entity.Status == ContractSlotStatus.ARCHIVED)
        {
            if (item.Status == ContractSlotStatus.ACTIVE)
                throw new ValidationException("contracts.archived_slot_cannot_be_reactivated");

            if (item.VehicleId != entity.VehicleId || item.AssignedAt != entity.AssignedAt)
                throw new ValidationException("contracts.archived_slot_assignment_immutable");

            if (item.ReplacedSlotId.HasValue && item.ReplacedSlotId != entity.ReplacedSlotId)
                throw new ValidationException("contracts.replaced_slot_immutable");

            if (entity.ArchivedAt.HasValue
                && item.ArchivedAt.HasValue
                && item.ArchivedAt != entity.ArchivedAt)
                throw new ValidationException("contracts.archived_at_immutable");
        }

        if (!entity.ReplacedSlotId.HasValue && item.ReplacedSlotId.HasValue)
            throw new ValidationException("contracts.replaced_slot_immutable");

        if (item.ReplacedSlotId.HasValue && item.ReplacedSlotId != entity.ReplacedSlotId)
            throw new ValidationException("contracts.replaced_slot_immutable");

        if (entity.Status == ContractSlotStatus.ACTIVE
            && entity.VehicleId is { } previousVehicleId
            && item.VehicleId is { } nextVehicleId
            && previousVehicleId != nextVehicleId)
        {
            throw new ValidationException("contracts.slot_vehicle_change_requires_new_slot");
        }
    }

    private static ContractSlot ApplyFields(
        ContractSlot entity,
        ContractSlotItemDto item,
        long? addedBy,
        bool isNew)
    {
        var previousStatus = entity.Status;
        entity.VehicleTypeId = item.VehicleTypeId;
        entity.VehicleId = item.VehicleId;
        entity.AssignedAt = isNew ? DateTime.UtcNow : entity.AssignedAt;
        entity.Status = item.Status;
        ApplyArchiveTimestamp(entity, previousStatus, item.Status, item.ArchivedAt);
        entity.ConsumptionRate = item.ConsumptionRate;
        entity.TripsPerMonth = item.TripsPerMonth;
        entity.TripsPerDay = item.TripsPerDay;
        entity.MaxKms = item.MaxKms;
        entity.MonthlyRental = item.MonthlyRental;
        entity.DailyValue = item.DailyValue;
        entity.OverConsumption = item.OverConsumption;
        entity.MinBillableWeightTons = item.MinBillableWeightTons;
        entity.MaxPayloadTons = item.MaxPayloadTons;
        entity.OverWeightPricePerTon = item.OverWeightPricePerTon;
        entity.ContractorOverWeightPricePerTon = item.ContractorOverWeightPricePerTon;
        entity.ExtraWorkingDayFees = item.ExtraWorkingDayFees;
        entity.AddedBy = addedBy;
        if (isNew)
            entity.ReplacedSlotId = item.ReplacedSlotId;

        return entity;
    }

    private static void ApplyArchiveTimestamp(
        ContractSlot entity,
        ContractSlotStatus previousStatus,
        ContractSlotStatus newStatus,
        DateTime? requestedArchivedAt)
    {
        if (newStatus != ContractSlotStatus.ARCHIVED)
            return;

        if (previousStatus == ContractSlotStatus.ARCHIVED)
        {
            if (!entity.ArchivedAt.HasValue && requestedArchivedAt.HasValue)
                entity.ArchivedAt = ContractSlotArchiveHelper.ResolveArchivedAt(entity, requestedArchivedAt);

            return;
        }

        entity.ArchivedAt = ContractSlotArchiveHelper.ResolveArchivedAt(entity, requestedArchivedAt);
    }
}
