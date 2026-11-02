using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

/// <summary>
/// Resource for DriverLicenseType API responses (both listing and single get)
/// </summary>
public class DriverLicenseTypeResource
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public UserStatus Status { get; set; }

    public static DriverLicenseTypeResource FromDto(DriverLicenseTypeDto dto) =>
        new DriverLicenseTypeResource
        {
            Id = dto.Id,
            Name = dto.NameEn,
            NameAr = dto.NameAr,
            NameEn = dto.NameEn,
            Status = dto.Status
        };
}
