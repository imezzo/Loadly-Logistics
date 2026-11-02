namespace LoadlyAggregator.Domain.Common.Entities;

public class ModelHasPermission
{
    public long PermissionId { get; set; }
    public string ModelType { get; set; } = string.Empty;
    public long ModelId { get; set; }

    public virtual Permission Permission { get; set; } = null!;
}
