using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

public class Admin : BaseUserEntity
{
    public AdminType Type { get; set; } = AdminType.ADMIN;
    public override string GuardName => "admin";
}
