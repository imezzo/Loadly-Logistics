namespace LoadlyAggregator.Domain.Common.Helpers;

/// <summary>
/// Load window for one contract slot. <paramref name="AssignedAtUtc"/> bounds data owned by the slot itself
/// (trips), while <paramref name="ChainAssignedAtUtc"/> is the earliest assignment in the slot's replacement
/// chain and bounds data describing the vehicle across the chain (facility visits), so a replacement slot
/// still sees a facility stay that began before it was assigned.
/// </summary>
public readonly record struct ContractSlotBillingLoadCriteria(
    long SlotId,
    long? VehicleId,
    DateTime AssignedAtUtc,
    DateTime? ArchivedAtUtc,
    DateTime ChainAssignedAtUtc);
