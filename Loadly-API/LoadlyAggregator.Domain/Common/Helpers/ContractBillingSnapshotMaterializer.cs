using System.Text.Json;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class ContractBillingSnapshotMaterializer
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public static ContractBillingSnapshot FromCalculation(
        long contractId,
        ContractType contractType,
        DateOnly periodStart,
        DateOnly periodEnd,
        CalculateIncludedContractSlotChargesResultDto calculation,
        ContractBillingSnapshotStatus status = ContractBillingSnapshotStatus.Draft,
        string? name = null)
    {
        var snapshot = new ContractBillingSnapshot
        {
            ContractId = contractId,
            Name = name ?? string.Empty,
            PeriodStart = periodStart,
            PeriodEnd = periodEnd,
            ContractType = contractType,
            Status = status,
            EngineVersion = ContractBillingSnapshotEngineVersion.Current,
            SubTotal = calculation.TotalAmount,
            WorkingDaysAmount = calculation.TotalWorkingDaysAmount,
            ExtraWorkingDaysAmount = calculation.TotalExtraWorkingDaysAmount,
            ExtraKmsAmount = calculation.TotalExtraKmsAmount,
            ExtraTripsAmount = calculation.TotalExtraTripsAmount,
            WorkingDaysCount = calculation.TotalWorkingDaysCount,
            ExtraWorkingDaysCount = calculation.TotalExtraWorkingDaysCount,
            ExtraTripsCount = calculation.TotalExtraTripsCount
        };

        foreach (var calendar in calculation.Calendar)
        {
            foreach (var day in calendar.Days)
            {
                snapshot.CalendarDays.Add(new ContractBillingSnapshotCalendarDay
                {
                    Snapshot = snapshot,
                    ContractSlotId = calendar.SlotId,
                    Date = day.Date,
                    Disposition = day.Disposition,
                    DispositionSource = ContractBillingSnapshotDispositionSource.Computed,
                    TripIds = day.TripIds.ToArray(),
                    TripCountSource = ContractBillingSnapshotDispositionSource.Computed,
                    WorkingDayIndex = day.WorkingDayIndex,
                    IsExtra = day.IsExtra,
                    FacilityType = day.FacilityType,
                    FacilityId = day.FacilityId
                });
            }
        }

        foreach (var line in calculation.Lines)
        {
            snapshot.ChargeLines.Add(new ContractBillingSnapshotChargeLine
            {
                Snapshot = snapshot,
                ContractSlotId = line.ContractSlotId > 0 ? line.ContractSlotId : null,
                Date = line.Date,
                ChargeKind = (ContractSlotChargeKind)line.ChargeKind,
                Amount = line.Amount
            });
        }

        AttachKmRollups(snapshot, calculation.Kms, calculation.KmPool, calculation.TotalExtraKmsCount);

        EnsureIntegrity(snapshot);
        return snapshot;
    }

    private static void AttachKmRollups(
        ContractBillingSnapshot snapshot,
        IReadOnlyList<IncludedContractSlotKmSummaryDto> kms,
        IncludedContractKmPoolDto? kmPool,
        decimal extraKmsCount)
    {
        snapshot.ExtraKmsCount = extraKmsCount;
        snapshot.KmPoolJson = kmPool is null
            ? null
            : JsonSerializer.Serialize(kmPool, JsonOptions);

        foreach (var km in kms)
        {
            snapshot.KmEvidence.Add(new ContractBillingSnapshotKmEvidence
            {
                Snapshot = snapshot,
                SnapshotId = snapshot.Id,
                ContractSlotId = km.SlotId,
                IsPooled = false,
                Consumption = km.Consumption,
                MaxKms = null,
                OverageKm = km.OverageKm,
                OverConsumptionRate = km.OverConsumptionRate,
                Amount = km.SuggestedOverageAmount
            });
        }

        if (kmPool is { } pool)
        {
            snapshot.KmEvidence.Add(new ContractBillingSnapshotKmEvidence
            {
                Snapshot = snapshot,
                SnapshotId = snapshot.Id,
                ContractSlotId = null,
                IsPooled = true,
                Consumption = pool.TotalConsumption,
                MaxKms = pool.TotalMaxKms,
                OverageKm = pool.OverageKm,
                OverConsumptionRate = pool.OverConsumptionRate,
                Amount = pool.SuggestedOverageAmount
            });
        }
    }

    public static void ReplaceChildren(
        ContractBillingSnapshot snapshot,
        CalculateIncludedContractSlotChargesResultDto calculation)
    {
        snapshot.SubTotal = calculation.TotalAmount;
        snapshot.WorkingDaysAmount = calculation.TotalWorkingDaysAmount;
        snapshot.ExtraWorkingDaysAmount = calculation.TotalExtraWorkingDaysAmount;
        snapshot.ExtraKmsAmount = calculation.TotalExtraKmsAmount;
        snapshot.ExtraTripsAmount = calculation.TotalExtraTripsAmount;
        snapshot.WorkingDaysCount = calculation.TotalWorkingDaysCount;
        snapshot.ExtraWorkingDaysCount = calculation.TotalExtraWorkingDaysCount;
        snapshot.ExtraTripsCount = calculation.TotalExtraTripsCount;
        snapshot.ExtraKmsCount = calculation.TotalExtraKmsCount;
        snapshot.KmPoolJson = calculation.KmPool is null
            ? null
            : JsonSerializer.Serialize(calculation.KmPool, JsonOptions);
        var now = DateTime.UtcNow;
        foreach (var child in snapshot.CalendarDays.Where(d => d.DeletedAt == null).ToList())
            child.SetDeletedAt(now);
        foreach (var child in snapshot.ChargeLines.Where(d => d.DeletedAt == null).ToList())
            child.SetDeletedAt(now);
        foreach (var child in snapshot.KmEvidence.Where(d => d.DeletedAt == null).ToList())
            child.SetDeletedAt(now);

        var fresh = FromCalculation(
            snapshot.ContractId,
            snapshot.ContractType,
            snapshot.PeriodStart,
            snapshot.PeriodEnd,
            calculation,
            snapshot.Status);

        foreach (var day in fresh.CalendarDays)
        {
            day.Snapshot = snapshot;
            day.SnapshotId = snapshot.Id;
            snapshot.CalendarDays.Add(day);
        }

        foreach (var line in fresh.ChargeLines)
        {
            line.Snapshot = snapshot;
            line.SnapshotId = snapshot.Id;
            snapshot.ChargeLines.Add(line);
        }

        foreach (var km in fresh.KmEvidence)
        {
            km.Snapshot = snapshot;
            km.SnapshotId = snapshot.Id;
            snapshot.KmEvidence.Add(km);
        }

        EnsureIntegrity(snapshot);
    }

    public static void EnsureIntegrity(ContractBillingSnapshot snapshot)
    {
        var linesTotal = snapshot.ChargeLines
            .Where(line => line.DeletedAt == null)
            .Sum(line => line.Amount);
        if (linesTotal != snapshot.SubTotal)
        {
            throw new LogicalException(
                "contract_billing_snapshot.charge_lines_mismatch",
                linesTotal,
                snapshot.SubTotal);
        }
    }
}
