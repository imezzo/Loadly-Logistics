using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.VehicleTypes;

public static class VehicleTypeTagValidation
{
    public static async Task ValidateAsync(
        IVehicleTypeRepository vehicleTypeRepository,
        IReadOnlyList<long> tagIds)
    {
        var distinctIds = tagIds.Distinct().ToList();
        var tags = await vehicleTypeRepository.GetTagsByIdsAsync(distinctIds);

        if (tags.Count != distinctIds.Count)
        {
            throw new ValidationException("vehicle_types.tags_not_found");
        }

        var pricingCount = tags.Count(t => t.IsPricing);
        if (pricingCount is not 1)
        {
            throw new ValidationException(pricingCount == 0
                ? "vehicle_types.at_least_one_pricing_tag"
                : "vehicle_types.at_most_one_pricing_tag");
        }
    }
}
