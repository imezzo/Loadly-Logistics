using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Admin.Actions.Auth;



public class AdminUpdateProfileAction : IAction<AdminUpdateProfileDto, object>
{
    private readonly IAdminRepository _adminRepository;
    private readonly IPermissionService _permissionService;
    public AdminUpdateProfileAction(IAdminRepository adminRepository, IPermissionService permissionService)
    {
        _adminRepository = adminRepository;
        _permissionService = permissionService;
    }
    public async Task<object> ExecuteAsync(AdminUpdateProfileDto dto)
    {
        ValidateInput(dto);
        var admin = await _adminRepository.GetByIdAsync(dto.UserId)
            ?? throw new NotFoundException("admins.not_found");
        await ValidateUniqueFieldsAsync(admin, dto);
        admin.FullName = dto.FullName;
        admin.UserName = dto.UserName.ToLower();
        admin.Email = dto.Email.ToLower();
        admin.Phone = dto.Phone;
        await _adminRepository.UpdateAsync(admin);
        return new
        {
            Id = admin.Id,
            FullName = admin.FullName,
            UserName = admin.UserName,
            Email = admin.Email,
            Phone = admin.Phone,
            UserType = "admin",
            MfaChannel = (int)admin.MfaChannel,
            EmailVerified = admin.EmailVerified,
            PhoneVerified = admin.PhoneVerified,
            Status = (int)admin.Status,
            Type = (int)admin.Type,
            Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(admin),
            CreatedAt = admin.CreatedAt,
            UpdatedAt = admin.UpdatedAt
        };
    }

    private async Task ValidateUniqueFieldsAsync(Domain.Common.Entities.Admin admin, AdminUpdateProfileDto dto)
    {
        await (admin.UserName != dto.UserName
            ? ValidateUsernameUniqueAsync(dto.UserName, dto.UserId)
            : Task.CompletedTask);
        if (admin.Email != dto.Email)
        {
            admin.EmailVerified = false;
        }

        await (admin.Phone != dto.Phone
            ? ValidatePhoneUniqueAsync(dto.Phone, dto.UserId, admin)
            : Task.CompletedTask);
    }

    private async Task ValidateUsernameUniqueAsync(string username, long userId)
    {
        var existingAdminByUsername = await _adminRepository.GetByUserNameAsync(username);
        _ = existingAdminByUsername == null || existingAdminByUsername.Id == userId
            ? true
            : throw new ValidationException("auth.username_already_taken");
    }

    private async Task ValidatePhoneUniqueAsync(string phone, long userId, Domain.Common.Entities.Admin admin)
    {
        var existingAdminByPhone = await _adminRepository.GetByPhoneAsync(phone);
        _ = existingAdminByPhone == null || existingAdminByPhone.Id == userId
            ? true
            : throw new ValidationException("auth.phone_already_taken");
        admin.PhoneVerified = false;
    }

    private static void ValidateInput(AdminUpdateProfileDto dto)
    {
        _ = !string.IsNullOrWhiteSpace(dto.FullName)
            ? true
            : throw new ValidationException("auth.full_name_required");
        _ = !string.IsNullOrWhiteSpace(dto.UserName)
            ? true
            : throw new ValidationException("auth.username_required");
        _ = !string.IsNullOrWhiteSpace(dto.Email)
            ? true
            : throw new ValidationException("auth.email_required");
        _ = !string.IsNullOrWhiteSpace(dto.Phone)
            ? true
            : throw new ValidationException("auth.phone_required");
        _ = System.Text.RegularExpressions.Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")
            ? true
            : throw new ValidationException("auth.invalid_email_format");
    }
}
