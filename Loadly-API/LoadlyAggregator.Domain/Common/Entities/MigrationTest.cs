using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

/// <summary>
/// Dummy table used solely to verify migrations run successfully on production.
/// Can be removed once migration pipeline is verified.
/// </summary>
public class MigrationTest : BaseEntity
{
    public string Value { get; set; } = string.Empty;
}
