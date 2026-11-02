namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IVehicleInsuranceRepository : IRepository<Common.Entities.VehicleInsurance>
{
    Task<Common.Entities.VehicleInsurance?> GetByPolicyNoAsync(string policyNo);

    Task<(List<Common.Entities.VehicleInsurance> VehicleInsurances, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize);

    Task<Common.Entities.VehicleInsurance> GetByIdOrThrowAsync(long id);
}
