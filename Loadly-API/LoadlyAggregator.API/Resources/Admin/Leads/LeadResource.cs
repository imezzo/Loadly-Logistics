using LoadlyAggregator.Domain.Admin.DTOs.Leads;

namespace LoadlyAggregator.API.Resources.Admin.Leads;

public class LeadResource
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public long? AddedBy { get; set; }
    public int TotalCompletedTrips { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static LeadResource FromDto(LeadDto dto) =>
        new LeadResource
        {
            Id = dto.Id,
            FullName = dto.FullName,
            Phone = dto.Phone,
            Email = dto.Email,
            AddedBy = dto.AddedBy,
            TotalCompletedTrips = dto.TotalCompletedTrips,
            CreatedAt = dto.CreatedAt,
            UpdatedAt = dto.UpdatedAt
        };
}
