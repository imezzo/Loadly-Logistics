using LoadlyAggregator.Domain.Admin.DTOs.Collectors;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Collectors;

public class DeleteCollectorAction
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteCollectorAction(
        ICollectorRepository collectorRepository,
        IUnitOfWork unitOfWork)
    {
        _collectorRepository = collectorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(DeleteCollectorDto dto)
    {
        var collector = await _collectorRepository.GetByIdOrThrowAsync(dto.Id);
        _collectorRepository.Remove(collector);
        await _unitOfWork.SaveChangesAsync();
    }
}
