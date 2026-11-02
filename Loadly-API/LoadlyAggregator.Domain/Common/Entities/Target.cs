using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class Target : BaseEntity
{
    public TargetMonth Month { get; set; }
    public int Year { get; set; }
    public TargetType TargetType { get; set; }
    public decimal TargetValue { get; set; }
}
