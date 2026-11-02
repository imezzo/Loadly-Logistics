using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.Leads;

public class LeadDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public long? AddedBy { get; set; }
    public int TotalCompletedTrips { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateLeadDto
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
}

public class UpdateLeadDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
}

public class GetLeadDto
{
    public long Id { get; set; }
}

public class ListLeadsDto : BasePagedRequest
{
}

public class DeleteLeadDto
{
    public long Id { get; set; }
}

public class GetLeadByPhoneDto
{
    public string Phone { get; set; } = string.Empty;
}
