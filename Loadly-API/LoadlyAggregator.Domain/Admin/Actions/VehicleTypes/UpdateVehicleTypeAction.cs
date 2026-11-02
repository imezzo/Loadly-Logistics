using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleTypes;

public class UpdateVehicleTypeAction
{
    private readonly IVehicleTypeRepository _vehicleTypeRepository;
    private readonly ICurrentUserService _currentUserService;

    public UpdateVehicleTypeAction(
        IVehicleTypeRepository vehicleTypeRepository,
        ICurrentUserService currentUserService)
    {
        _vehicleTypeRepository = vehicleTypeRepository;
        _currentUserService = currentUserService;
    }

    public async Task ExecuteAsync(UpdateVehicleTypeDto dto)
    {
        await VehicleTypeTagValidation.ValidateAsync(_vehicleTypeRepository, dto.TagIds);

        var vehicleType = await _vehicleTypeRepository.GetByIdOrThrowAsync(dto.Id);

        vehicleType.NameAr = dto.NameAr;
        vehicleType.NameEn = dto.NameEn;
        vehicleType.Category = dto.Category;
        vehicleType.Status = dto.Status;
        vehicleType.VehicleUnitType = dto.VehicleUnitType;
        await _vehicleTypeRepository.UpdateAsync(vehicleType);

        await SyncTagsAsync(vehicleType.Id, dto.TagIds, _currentUserService.GetCurrentUserId());
        await _vehicleTypeRepository.SaveChangesAsync();
    }

    private async Task SyncTagsAsync(long vehicleTypeId, IReadOnlyList<long> tagIds, long? addedBy)
    {
        var requestedIds = tagIds.Distinct().ToHashSet();
        var existing = await _vehicleTypeRepository.GetTagsByVehicleTypeIdAsync(vehicleTypeId);
        var existingTagIds = existing.Select(e => e.TagId).ToHashSet();

        var toRemove = existing.Where(e => !requestedIds.Contains(e.TagId)).ToList();
        if (toRemove.Count > 0)
        {
            _vehicleTypeRepository.RemoveVehicleTypeTags(toRemove);
        }

        foreach (var tagId in requestedIds.Except(existingTagIds))
        {
            await _vehicleTypeRepository.AddVehicleTypeTagAsync(new VehicleTypeTag
            {
                VehicleTypeId = vehicleTypeId,
                TagId = tagId,
                AddedBy = addedBy
            });
        }
    }
}
