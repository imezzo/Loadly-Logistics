using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Domain.Common.Entities;

public class ModelHasRole
{
    public long RoleId { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public long ModelId { get; set; }

    public virtual Role Role { get; set; } = null!;
}
