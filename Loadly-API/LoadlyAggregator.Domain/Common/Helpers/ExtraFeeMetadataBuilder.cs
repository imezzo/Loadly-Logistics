using LoadlyAggregator.Domain.Common.DTOs;

namespace LoadlyAggregator.Domain.Common.Helpers;

/// <summary>
/// Builds the full metadata for all 7 extra fee types. Delegates to <see cref="ExtraFeeMetadataRegistry"/> (single source of truth).
/// Used by GET /api/lookups/extra-fee-types.
/// </summary>
public static class ExtraFeeMetadataBuilder
{
    /// <summary>
    /// Returns metadata for all extra fee types in enum order (typeId 1–7).
    /// </summary>
    public static List<ExtraFeeTypeMetaDto> Build() => ExtraFeeMetadataRegistry.Build();
}
