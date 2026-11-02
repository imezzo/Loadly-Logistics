using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;

namespace LoadlyAggregator.Domain.Admin.Actions.Auth;

public class AdminDisableMfaAction : IAction<AdminDisableMfaDto, bool>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminDisableMfaAction(IAdminRepository adminRepository, IUnitOfWork unitOfWork)
    {
        _adminRepository = adminRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ExecuteAsync(AdminDisableMfaDto dto)
    {
        var admin = await _adminRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(admin, "Admin");

        admin!.MfaChannel = CommunicationChannel.None;
        await _adminRepository.UpdateAsync(admin);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
