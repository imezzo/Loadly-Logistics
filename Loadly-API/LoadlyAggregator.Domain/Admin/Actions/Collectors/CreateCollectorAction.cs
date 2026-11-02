using LoadlyAggregator.Domain.Admin.DTOs.Collectors;
using LoadlyAggregator.Domain.Admin.Services;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Collectors;

public class CreateCollectorAction
{
    private readonly ICollectorRepository _collectorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICollectorNotificationService _collectorNotificationService;
    private readonly IResetPasswordUrlService _resetPasswordUrlService;
    private readonly IPermissionService _permissionService;

    public CreateCollectorAction(
        ICollectorRepository collectorRepository,
        IUnitOfWork unitOfWork,
        ICollectorNotificationService collectorNotificationService,
        IResetPasswordUrlService resetPasswordUrlService,
        IPermissionService permissionService)
    {
        _collectorRepository = collectorRepository;
        _unitOfWork = unitOfWork;
        _collectorNotificationService = collectorNotificationService;
        _resetPasswordUrlService = resetPasswordUrlService;
        _permissionService = permissionService;
    }
    public async Task ExecuteAsync(CreateCollectorDto dto)
    {
        var collector = new Common.Entities.Collector
        {
            FullName = dto.FullName,
            UserName = dto.UserName.ToLower(),
            Email = string.IsNullOrWhiteSpace(dto.Email) ? string.Empty : dto.Email.ToLower(),
            Phone = dto.Phone,
            Status = (UserStatus)dto.Status,
            Password = string.Empty,
            EmailVerified = false,
            PhoneVerified = false,
            MfaChannel = (CommunicationChannel)dto.MfaChannel
        };
        await _collectorRepository.AddAsync(collector);
        await _unitOfWork.SaveChangesAsync();

        await _permissionService.AssignRolesIfProvidedAsync(
            collector,
            dto.RoleId.HasValue ? [dto.RoleId.Value] : null);

        var setPasswordUrl = $"{_resetPasswordUrlService.GetBaseUrl("collector")}/auth/forget-password";
        await _collectorNotificationService.SendCollectorWelcomeSmsAsync(collector, setPasswordUrl);
    }
}
