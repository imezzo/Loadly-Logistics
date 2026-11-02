using LoadlyAggregator.Domain.Admin.DTOs.Admins;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Admins;

public class UpdateAdminAction(
    IAdminRepository adminRepository,
    IUnitOfWork unitOfWork,
    IPermissionService permissionService)
{
    private readonly IAdminRepository _adminRepository = adminRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPermissionService _permissionService = permissionService;
    public async Task ExecuteAsync(UpdateAdminDto dto)
    {
        var admin = await _adminRepository.GetByIdAsync(dto.Id) ?? throw new NotFoundException("admins.not_found_with_id", dto.Id);

        admin.FullName = dto.FullName;
        admin.UserName = dto.UserName.ToLower();
        admin.Email = dto.Email.ToLower();
        admin.Phone = dto.Phone;
        admin.Type = AdminType.ADMIN;
        admin.MfaChannel = (CommunicationChannel)dto.MfaChannel;
        admin.Status = (UserStatus)dto.Status;
        await _adminRepository.UpdateAsync(admin);
        await _unitOfWork.SaveChangesAsync();

        await _permissionService.UpdateRolesAsync(admin, dto.RoleIds);
    }
}
