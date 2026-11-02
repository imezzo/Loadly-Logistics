using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class PortDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? PortCode { get; set; }
    public int Type { get; set; }
    public int Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreatePortDto
{
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? PortCode { get; set; }
    public PortType Type { get; set; }
    public PortStatus Status { get; set; }
}

public class UpdatePortDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? PortCode { get; set; }
    public PortType Type { get; set; }
    public PortStatus Status { get; set; }
}

public class GetPortDto
{
    public long Id { get; set; }
}

public class DeletePortDto
{
    public long Id { get; set; }
}

public class ListPortsDto : BasePagedRequest
{
    public PortType? Type { get; set; }
    public PortStatus? Status { get; set; }
}
