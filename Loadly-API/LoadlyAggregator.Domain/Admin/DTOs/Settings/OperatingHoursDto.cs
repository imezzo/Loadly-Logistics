using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class OperatingHoursDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateOperatingHoursDto
{
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
}

public class UpdateOperatingHoursDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
}

public class GetOperatingHoursDto
{
    public long Id { get; set; }
}

public class ListOperatingHoursDto : BasePagedRequest
{
}

public class DeleteOperatingHoursDto
{
    public long Id { get; set; }
}
