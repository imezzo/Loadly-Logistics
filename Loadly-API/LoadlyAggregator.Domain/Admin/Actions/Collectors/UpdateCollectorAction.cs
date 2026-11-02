using LoadlyAggregator.Domain.Admin.Actions.Collectors;
using LoadlyAggregator.Domain.Admin.DTOs.Collectors;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Collectors;

public class UpdateCollectorAction(
    ICollectorRepository collectorRepository,
    IUnitOfWork unitOfWork,
    IPermissionService permissionService)
{
    private readonly ICollectorRepository _collectorRepository = collectorRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IPermissionService _permissionService = permissionService;

    public async Task ExecuteAsync(UpdateCollectorDto dto)
    {
        var collector = await _collectorRepository.GetByIdAsync(dto.Id)
            ?? throw new NotFoundException("collectors.not_found_with_id", dto.Id);

        collector.FullName = dto.FullName;
        collector.UserName = dto.UserName.ToLower();
        collector.Email = string.IsNullOrWhiteSpace(dto.Email) ? string.Empty : dto.Email.ToLower();
        collector.Phone = dto.Phone;
        collector.MfaChannel = (CommunicationChannel)dto.MfaChannel;
        collector.Status = (UserStatus)dto.Status;
        await _collectorRepository.UpdateAsync(collector);
        await _unitOfWork.SaveChangesAsync();

        await _permissionService.UpdateRolesAsync(
            collector,
            dto.RoleId.HasValue ? [dto.RoleId.Value] : null);
    }
}
