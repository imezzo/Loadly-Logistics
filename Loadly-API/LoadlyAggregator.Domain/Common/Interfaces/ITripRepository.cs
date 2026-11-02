using System.Collections.Generic;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Export;
using LoadlyAggregator.Domain.Common.Models;
namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface ITripRepository : IRepository<Trip>
{
    IQueryable<Trip> QueryWithoutIncludes();

    Task<(IEnumerable<Trip> Trips, int TotalCount)> SearchAsync(
        long? contractId = null,
        long? vehicleId = null,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null,
        int page = 1,
        int pageSize = 10);
    Task<IEnumerable<Trip>> GetByContractIdAsync(long contractId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<IEnumerable<Trip>> GetByVehicleIdsAsync(List<long> vehicleIds);
    Task<int> UpdateDriverForVehicleTripsAsync(long vehicleId, long? newDriverId, DateTime minStartDateInclusive);
    Task<IReadOnlyList<Trip>> GetTripsByVehicleAndStatusesAsync(long vehicleId, IEnumerable<TripStatus> statuses, DateTime minStartDateInclusive);
    Task<(List<Trip> Trips, int TotalCount)> ListAsync(
        long? contractId,
        string? contractReferenceNumber,
        long? companyId,
        List<long>? matchingCompanyIds,
        string? companyName,
        string? driverName,
        string? plate,
        long? vehicleId,
        long? vehicleTypeId,
        List<long>? sectorIds,
        long? driverId,
        IEnumerable<long>? leadIds,
        bool? isExternalTrip,
        List<long>? contractorIds,
        bool? hasContractor,
        int? status,
        IEnumerable<TripDocumentStatus>? tripDocumentStatuses,
        int? overNightCostBearer,
        int? dispatchingMode,
        IEnumerable<int>? statuses,
        DateTime? startDateFrom,
        DateTime? startDateTo,
        OperationalDriverStatus? currentDriverStatus,
        TripLifecycleActor? lifecycleActor,
        string? search,
        int? page,
        int? pageSize,
        bool? isBackfill = null,
        List<long>? destinationIds = null,
        long? invitedContractorId = null,
        bool? openInviteOnly = null,
        bool? isPushedToContractors = null,
        bool? hasContractorInvoice = null,
        bool? isContractorPaid = null,
        bool? isCreatedByCompany = null,
        long? contractVehicleTypeId = null,
        ContractType? contractTypeId = null,
        DateOnly? financialDateFrom = null,
        DateOnly? financialDateTo = null);

    IAsyncEnumerable<IReadOnlyList<TripExportRow>> EnumerateForExportAsync(
        TripExportCriteria criteria,
        IReadOnlyCollection<string> columns,
        CancellationToken cancellationToken = default);

    Task<Trip> GetByIdOrThrowAsync(long id);
    Task<IReadOnlyList<Trip>> GetByIdsAsync(IReadOnlyList<long> ids);
    Task<IReadOnlyDictionary<long, DateTime?>> GetStartDatesByIdsAsync(IReadOnlyList<long> ids);
    Task<IReadOnlyDictionary<long, (DateTime? StartDate, bool HasOvernight, decimal OvernightAmount, decimal OvernightCompanyShare, bool HasFuel, decimal FuelAmount, decimal FuelDistanceKm)>> GetCalendarBillingFactsByIdsAsync(IReadOnlyList<long> ids);
    Task<IReadOnlyDictionary<long, (decimal Amount, decimal CompanyShare)>> GetOvernightChargesByIdsAsync(IReadOnlyList<long> ids);
    Task<IReadOnlyDictionary<long, (decimal Amount, decimal DistanceKm)>> GetFuelChargesByIdsAsync(IReadOnlyList<long> ids);
    Task<int> BulkUpdateDocumentStatusAsync(IReadOnlyList<long> tripIds, long collectorId);
    Task<int> BulkUpdateDocumentStatusToFinanceTeamAsync(IReadOnlyList<long> tripIds, long financeCollectorId);
    Task<Trip?> GetByIdForChargeUpdateAsync(long id);

    Task<IReadOnlyList<Trip>> ListActiveTripsWithLocationAsync(
        long? companyId,
        string? contractReferenceNumber,
        long? driverId,
        long? vehicleId);

    Task<IReadOnlyList<Trip>> GetForContractSlotBillingAsync(
        long contractSlotId,
        long vehicleId,
        long contractId,
        DateTime assignedAtUtc,
        DateTime? archivedAtUtc,
        DateOnly periodStart,
        DateOnly periodEnd);

    Task<IReadOnlyDictionary<long, IReadOnlyList<Trip>>> GetForContractSlotBillingBySlotsAsync(
        long contractId,
        IReadOnlyList<ContractSlotBillingLoadCriteria> slots,
        DateOnly periodStart,
        DateOnly periodEnd);

    Task<bool> HasCompletedTripsForContractSlotAsync(long contractSlotId);

    Task<IReadOnlyDictionary<TripStatus, int>> GetTripStatusCountsAsync(
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null);

    Task<IReadOnlyList<(long SectorId, TripStatus Status, int Count)>> GetTripStatusCountsBySectorAsync(
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null);

    Task<IReadOnlyDictionary<TripStatus, int>> GetTripStatusCountsForCompanyAsync(
        long companyId,
        DateTime? startDateFrom = null,
        DateTime? startDateTo = null);

    Task<IReadOnlyList<(int Month, int TripsCount, decimal TotalCost)>> GetMonthlyCompletedTripAggregatesAsync(int year, int? month = null);

    Task<IReadOnlyList<(int Year, int Month, int TripsCount, decimal TotalCost)>> GetMonthlyCompletedTripAggregatesForYearsAsync(
        IEnumerable<int> years);

    Task<IReadOnlyList<(int Month, int TripsCount, decimal TotalKilometers)>> GetMonthlyCompletedTripKilometerAggregatesForCompanyAsync(
        long companyId,
        int year,
        int? month = null);

    Task<(decimal LastSevenDays, decimal LastThirtyDays, decimal LastThreeMonths)> GetCompanyCompletedTripCostTotalsForRollingPeriodsAsync(
        long companyId,
        DateTime utcNow);

    Task<int> GetAssignedVehicleTripsCountInDateRangeAsync(DateTime from, DateTime to);

    Task<int> GetAssignedDriverTripsCountInDateRangeAsync(DateTime from, DateTime to);

    Task<decimal> GetAverageTripDurationHoursAsync();

    Task<int> GetActiveContractorTripsCountAsync();

    Task<IReadOnlyDictionary<TripStatus, int>> GetTripStatusCountsForContractorAsync(long contractorId);

    Task<(int CompletedPaidTrips, decimal CompletedPaidTripsTotalAmount, int CompletedUnpaidTrips, decimal CompletedUnpaidTripsTotalAmount)>
        GetContractorCompletedTripPaymentTotalsAsync(long contractorId);

    Task<(int OpenInviteTrips, int PendingBookingRequestTrips)> GetContractorInviteTripCountsAsync(long contractorId);

    Task<int> GetTripCountInDateRangeAsync(DateTime fromInclusive, DateTime toExclusive);

    Task<IReadOnlyList<(DateTime Date, int Count)>> GetDailyActiveContractorTripCountsAsync(
        DateTime fromInclusive,
        DateTime toExclusive);

    Task<int> GetCompanyTripCountInDateRangeAsync(long companyId, DateTime fromInclusive, DateTime toExclusive);

    Task<(
        int AllTimeTrips,
        decimal AllTimeMargin,
        int CurrentMonthTrips,
        decimal CurrentMonthMargin,
        int PreviousMonthTrips,
        decimal PreviousMonthMargin)> GetContractorDashboardMetricsAsync(
        long? contractorId,
        DateTime currentMonthStartUtc);

    Task<decimal> GetCompanyCompletedTripCostTotalAsync(long companyId, DateTime? fromInclusive = null, DateTime? toExclusive = null);

    Task<decimal> GetCompanyCompletedTripKilometersTotalAsync(long companyId, DateTime? fromInclusive = null, DateTime? toExclusive = null);

    Task<IReadOnlyList<(long VehicleTypeId, string NameEn, string NameAr, decimal Amount)>> GetCompanyCompletedTripCostByVehicleTypeAsync(
        long companyId,
        DateTime? fromInclusive = null,
        DateTime? toExclusive = null);

    Task<(IReadOnlyList<(long Id, string NameEn, string NameAr, string Phone, int Driver, int Dispatcher, int Assisted, int Total)> Rows, int TotalCount)> GetDriverCompletedTripsByLifecycleActorAsync(
        long? driverId = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        int page = 1,
        int pageSize = 10);
}
