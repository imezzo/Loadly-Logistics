using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Contractors;

public class DeleteContractorAction
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteContractorAction(IContractorRepository contractorRepository, IUnitOfWork unitOfWork)
    {
        _contractorRepository = contractorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(DeleteContractorDto dto)
    {
        var contractor = await _contractorRepository.GetByIdOrThrowAsync(dto.Id);
        _contractorRepository.Remove(contractor);
        await _unitOfWork.SaveChangesAsync();
    }
}
