using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Vehicles;

public class ListVehicleAssignmentHistoriesAction
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleAssignmentHistoryRepository _assignmentHistoryRepository;

    public ListVehicleAssignmentHistoriesAction(
        IVehicleRepository vehicleRepository,
        IVehicleAssignmentHistoryRepository assignmentHistoryRepository)
    {
        _vehicleRepository = vehicleRepository;
        _assignmentHistoryRepository = assignmentHistoryRepository;
    }

    public async Task<ListingDto<VehicleAssignmentHistoryDto>> ExecuteAsync(ListVehicleAssignmentHistoriesDto dto)
    {
        _ = await _vehicleRepository.GetByIdOrThrowAsync(dto.VehicleId);

        var (items, totalCount) = await _assignmentHistoryRepository.ListAsync(
            dto.VehicleId,
            dto.Source,
            dto.CreatedFrom,
            dto.CreatedTo,
            dto.Page,
            dto.PageSize);

        return new ListingDto<VehicleAssignmentHistoryDto>
        {
            Data = items.Select(VehicleAssignmentHistoryDto.FromEntity).ToList(),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
