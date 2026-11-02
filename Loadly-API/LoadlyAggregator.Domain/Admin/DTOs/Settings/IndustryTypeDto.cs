using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class IndustryTypeDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateIndustryTypeDto
{
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
}

public class UpdateIndustryTypeDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
}

public class GetIndustryTypeDto
{
    public long Id { get; set; }
}

public class ListIndustryTypesDto : BasePagedRequest
{
}

public class DeleteIndustryTypeDto
{
    public long Id { get; set; }
}
