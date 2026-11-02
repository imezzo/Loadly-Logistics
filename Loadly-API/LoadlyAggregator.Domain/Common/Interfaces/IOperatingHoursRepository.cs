namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IOperatingHoursRepository : IRepository<Common.Entities.OperatingHour>
{
    Task<Common.Entities.OperatingHour?> GetByNameAsync(string nameEn);
    Task<Common.Entities.OperatingHour> GetByIdOrThrowAsync(long id);
    Task<(List<Common.Entities.OperatingHour> OperatingHours, int TotalCount)> ListAsync(
        string? search,
        int page,
        int pageSize);
}
