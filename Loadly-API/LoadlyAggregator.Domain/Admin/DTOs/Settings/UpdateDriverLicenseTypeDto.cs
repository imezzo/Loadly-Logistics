using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class UpdateDriverLicenseTypeDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
}
