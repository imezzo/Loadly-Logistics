using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IContractRepository : IRepository<Common.Entities.Contract>
{
    Task<Common.Entities.Contract?> GetByNameEnAsync(string nameEn);
    Task<Common.Entities.Contract?> GetByNameArAsync(string nameAr);
    Task<List<Common.Entities.Contract>> GetByCompanyIdAsync(long companyId);
    Task<List<Common.Entities.Contract>> GetByContractTypeAsync(ContractType contractType);

    Task<(List<Common.Entities.Contract> Contracts, int TotalCount)> ListAsync(
        string? search,
        long? contractId,
        long? companyId,
        ContractType? contractType,
        long? salesRepId,
        ContractStatus? status,
        string? referenceNumber,
        int page,
        int pageSize);

    Task<(List<Common.Entities.Contract> Contracts, int TotalCount)> ListWithChildrenAsync(
        string? search,
        long? contractId,
        long? companyId,
        ContractType? contractType,
        long? salesRepId,
        ContractStatus? status,
        string? referenceNumber,
        int page,
        int pageSize);

    Task<Common.Entities.Contract> GetByIdOrThrowAsync(long id);

    /// <summary>Read-only contract with slots (and vehicle plates) for slot charge calculation/export.</summary>
    Task<Common.Entities.Contract> GetByIdOrThrowForSlotBillingAsync(long id, bool includeContractVehicleTypes = true);

    Task<int?> GetMaxReferenceSequenceForYearAsync(int year);

    /// <summary>Lightweight contract consumption type (and company/status for company authorization).</summary>
    Task<(long CompanyId, ContractStatus Status, ContractConsumptionTypes ConsumptionType)?> GetContractConsumptionTypeAsync(long contractId);
}
