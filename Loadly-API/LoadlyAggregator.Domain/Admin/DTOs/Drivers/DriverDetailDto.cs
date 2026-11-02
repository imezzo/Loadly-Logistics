namespace LoadlyAggregator.Domain.Admin.DTOs.Drivers;

public class DriverDetailDto
{
    public long Id { get; set; }
    public long DriverId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public long? LicenseTypeId { get; set; }
    public DateTime? LicenseIssueDate { get; set; }
    public string? LicenseNo { get; set; }
    public DateTime? LicenseExpirationDate { get; set; }
    public string? VehicleTypeIds { get; set; }
    public int? Age { get; set; }
    public int? LicenseCategory { get; set; }
    public List<int>? SpokenLanguages { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
