using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Auth;

public class AdminEnableMfaAction : IAction<AdminEnableMfaDto, bool>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AdminEnableMfaAction(IAdminRepository adminRepository, IUnitOfWork unitOfWork)
    {
        _adminRepository = adminRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> ExecuteAsync(AdminEnableMfaDto dto)
    {
        _ = dto.Channel == CommunicationChannel.Sms
            ? true
            : throw new ValidationException("validation.admin_channel_must_be_sms");

        var admin = await _adminRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(admin, "Admin");
        UserValidator.ValidateChannelVerified(CommunicationChannel.Sms, admin!.EmailVerified, admin.PhoneVerified);

        admin.MfaChannel = CommunicationChannel.Sms;
        await _adminRepository.UpdateAsync(admin);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
