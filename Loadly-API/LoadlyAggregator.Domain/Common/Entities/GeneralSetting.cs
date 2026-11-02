using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

public class GeneralSetting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
