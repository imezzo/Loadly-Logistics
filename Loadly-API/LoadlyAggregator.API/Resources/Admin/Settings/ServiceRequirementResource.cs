using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

/// <summary>
/// Resource for ServiceRequirement API responses (both listing and single get)
/// </summary>
public class ServiceRequirementResource
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? NameEn { get; set; }

    public static ServiceRequirementResource FromDto(ServiceRequirementDto dto) =>
        new ServiceRequirementResource
        {
            Id = dto.Id,
            Name = dto.NameEn,
            NameAr = dto.NameAr,
            NameEn = dto.NameEn
        };
}
