using LoadlyAggregator.Domain.Admin.DTOs.Admins;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Admins;

public class DeleteAdminAction
{
    private readonly IAdminRepository _adminRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAdminAction(
        IAdminRepository adminRepository,
        IUnitOfWork unitOfWork)
    {
        _adminRepository = adminRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(DeleteAdminDto dto)
    {
        var admin = await _adminRepository.GetByIdOrThrowAsync(dto.Id);
        _adminRepository.Remove(admin);
        await _unitOfWork.SaveChangesAsync();
    }
}
