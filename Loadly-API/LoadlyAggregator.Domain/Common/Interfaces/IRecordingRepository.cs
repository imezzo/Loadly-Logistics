using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Interfaces;

public interface IRecordingRepository
{
    Task AddAsync(Recording recording);
    Task<IEnumerable<Recording>> GetTimelineByRecordableAsync(
        long recordableId,
        IEnumerable<RecordableType> recordableTypes,
        int? limit = null);
    Task<int> SaveChangesAsync();
}
