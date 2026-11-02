using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class ServiceRequirementDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateServiceRequirementDto
{
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
}

public class UpdateServiceRequirementDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
}

public class GetServiceRequirementDto
{
    public long Id { get; set; }
}

public class ListServiceRequirementsDto : BasePagedRequest
{
}

public class DeleteServiceRequirementDto
{
    public long Id { get; set; }
}
