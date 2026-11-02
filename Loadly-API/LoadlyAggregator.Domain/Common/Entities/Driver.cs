using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class Driver : BaseUserEntity
{
    public override string GuardName => "driver";
    public string FullNameAr { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string ProfilePicture { get; set; } = string.Empty;
    public bool BiometricFingerprint { get; set; } = false;
    public DriverWorkingStatus? WorkingStatus { get; set; }
    public long? ContractorId { get; set; }

    public virtual Contractor? Contractor { get; set; }
    public virtual DriverLocation? DriverLocation { get; set; }
    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
    public virtual DriverDetail? DriverDetail { get; set; }
    public virtual ICollection<DriverDocument> DriverDocuments { get; set; } = new List<DriverDocument>();
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
