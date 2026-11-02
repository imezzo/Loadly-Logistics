using LoadlyAggregator.Domain.Common.BaseClasses;

namespace LoadlyAggregator.Domain.Common.Entities;

public class Collector : BaseUserEntity
{
    public override string GuardName => "collector";
}
