using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class DriverDetail : BaseEntity
{
    public long DriverId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public long? LicenseTypeId { get; set; }
    public DateTime? LicenseIssueDate { get; set; }
    public string? LicenseNo { get; set; }
    public DateTime? LicenseExpirationDate { get; set; }
    public string VehicleTypeIds { get; set; } = string.Empty;
    public int? Age { get; set; }
    public LicenseCategory? LicenseCategory { get; set; }
    public List<SpokenLanguage>? SpokenLanguages { get; set; }

    // EmergencyContact fields
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }

    public virtual Driver Driver { get; set; } = null!;
}
