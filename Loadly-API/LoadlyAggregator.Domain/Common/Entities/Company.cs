using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;
namespace LoadlyAggregator.Domain.Common.Entities;

public class Company : BaseUserEntity
{
    public long? CompanyId { get; set; }
    public CompanyType Type { get; set; } = CompanyType.COMPANY;
    public string ProfilePicture { get; set; } = string.Empty;
    public bool BiometricFingerprint { get; set; } = false;
    public override string GuardName => "company";

    public virtual Company? ParentCompany { get; set; }
    public virtual CompanyDetail? CompanyDetail { get; set; }
    public virtual ICollection<CompanyDocument> CompanyDocuments { get; set; } = new List<CompanyDocument>();
}
