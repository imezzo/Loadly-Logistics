using LoadlyAggregator.Domain.Collector.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Collector.Actions.Auth;

public class CollectorUpdateProfileAction : IAction<CollectorUpdateProfileDto, object>
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly IPermissionService _permissionService;

    public CollectorUpdateProfileAction(ICollectorRepository collectorRepository, IPermissionService permissionService)
    {
        _collectorRepository = collectorRepository;
        _permissionService = permissionService;
    }

    public async Task<object> ExecuteAsync(CollectorUpdateProfileDto dto)
    {
        ValidateInput(dto);
        var collector = await _collectorRepository.GetByIdAsync(dto.UserId)
            ?? throw new NotFoundException("collectors.not_found");
        var normalizedEmail = string.IsNullOrWhiteSpace(dto.Email) ? string.Empty : dto.Email.ToLower();
        await ValidateUniqueFieldsAsync(collector, dto, normalizedEmail);

        collector.FullName = dto.FullName;
        collector.UserName = dto.UserName.ToLower();
        collector.Email = normalizedEmail;
        collector.Phone = dto.Phone;
        await _collectorRepository.UpdateAsync(collector);

        return new
        {
            Id = collector.Id,
            FullName = collector.FullName,
            UserName = collector.UserName,
            Email = collector.Email,
            Phone = collector.Phone,
            UserType = "collector",
            MfaChannel = (int)collector.MfaChannel,
            EmailVerified = collector.EmailVerified,
            PhoneVerified = collector.PhoneVerified,
            Status = (int)collector.Status,
            Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(collector),
            CreatedAt = collector.CreatedAt,
            UpdatedAt = collector.UpdatedAt
        };
    }

    private async Task ValidateUniqueFieldsAsync(Domain.Common.Entities.Collector collector, CollectorUpdateProfileDto dto, string normalizedEmail)
    {
        await (collector.UserName != dto.UserName
            ? ValidateUsernameUniqueAsync(dto.UserName, dto.UserId)
            : Task.CompletedTask);

        if (collector.Email != normalizedEmail)
        {
            collector.EmailVerified = false;
        }

        await (collector.Phone != dto.Phone
            ? ValidatePhoneUniqueAsync(dto.Phone, dto.UserId, collector)
            : Task.CompletedTask);
    }

    private async Task ValidateUsernameUniqueAsync(string username, long userId)
    {
        var existingCollectorByUsername = await _collectorRepository.GetByUserNameAsync(username);
        _ = existingCollectorByUsername == null || existingCollectorByUsername.Id == userId
            ? true
            : throw new ValidationException("auth.username_already_taken");
    }

    private async Task ValidatePhoneUniqueAsync(string phone, long userId, Domain.Common.Entities.Collector collector)
    {
        var existingCollectorByPhone = await _collectorRepository.GetByPhoneAsync(phone);
        _ = existingCollectorByPhone == null || existingCollectorByPhone.Id == userId
            ? true
            : throw new ValidationException("auth.phone_already_taken");
        collector.PhoneVerified = false;
    }

    private static void ValidateInput(CollectorUpdateProfileDto dto)
    {
        _ = !string.IsNullOrWhiteSpace(dto.FullName)
            ? true
            : throw new ValidationException("auth.full_name_required");
        _ = !string.IsNullOrWhiteSpace(dto.UserName)
            ? true
            : throw new ValidationException("auth.username_required");
        _ = !string.IsNullOrWhiteSpace(dto.Phone)
            ? true
            : throw new ValidationException("auth.phone_required");
    }
}
