using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.API.Extensions;

/// <summary>
/// Extension methods for Resource collections (similar to Laravel's Resource::collection())
/// </summary>
public static class ResourceExtensions
{
    /// <summary>
    /// Transforms a ListingDto of DTOs to ListingDto of resources
    /// </summary>
    public static ListingDto<TResource> ToResourceCollection<TDto, TResource>(
        this ListingDto<TDto> listingDto,
        Func<TDto, TResource> resourceFactory) =>
        new ListingDto<TResource>
        {
            Data = listingDto.Data.Select(resourceFactory).ToList(),
            Additional = listingDto.Additional,
            Pagination = listingDto.Pagination
        };
}
