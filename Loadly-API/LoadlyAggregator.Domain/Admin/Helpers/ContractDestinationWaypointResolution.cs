using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Helpers;

public static class ContractDestinationWaypointResolution
{
    public static async Task<long> ResolveWaypointIdAsync(
        ContractDestinationWaypointItemDto waypoint,
        IWaypointRepository waypointRepository,
        long? addedBy)
    {
        if (waypoint.Id is { } existingId && existingId > 0)
        {
            var existingWaypoint = await waypointRepository.GetByIdOrThrowAsync(existingId);

            if (string.IsNullOrWhiteSpace(waypoint.AddressName))
                throw new ValidationException("contracts.contract_route_waypoint_address_required");

            existingWaypoint.GooglePlaceId = string.IsNullOrWhiteSpace(waypoint.GooglePlaceId) ? null : waypoint.GooglePlaceId.Trim();
            existingWaypoint.Latitude = waypoint.Latitude;
            existingWaypoint.Longitude = waypoint.Longitude;
            existingWaypoint.AddressName = waypoint.AddressName.Trim();
            existingWaypoint.AddedBy = addedBy;
            waypointRepository.Update(existingWaypoint);
            return existingId;
        }

        if (string.IsNullOrWhiteSpace(waypoint.AddressName))
            throw new ValidationException("contracts.contract_route_waypoint_address_required");

        var entity = new Waypoint
        {
            GooglePlaceId = string.IsNullOrWhiteSpace(waypoint.GooglePlaceId) ? null : waypoint.GooglePlaceId.Trim(),
            Latitude = waypoint.Latitude,
            Longitude = waypoint.Longitude,
            AddressName = waypoint.AddressName.Trim(),
            AddedBy = addedBy
        };

        await waypointRepository.AddAsync(entity);
        await waypointRepository.SaveChangesAsync();
        return entity.Id;
    }
}
