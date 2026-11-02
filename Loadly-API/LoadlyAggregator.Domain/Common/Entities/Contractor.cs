using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class Contractor : BaseUserEntity
{
    public override string GuardName => "contractor";
    public string? NationalId { get; set; }
    public string? SecondPhoneNumber { get; set; }
    public string? Address { get; set; }
    public ContractorStatus ContractorStatus { get; set; } = ContractorStatus.INACTIVE;
    public DateTime? ApprovedAt { get; set; }
    public long? ApprovedBy { get; set; }

    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();
    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
    public virtual ICollection<ContractorPayout> Payouts { get; set; } = new List<ContractorPayout>();
    public virtual ICollection<ContractorDocument> ContractorDocuments { get; set; } = new List<ContractorDocument>();
}
