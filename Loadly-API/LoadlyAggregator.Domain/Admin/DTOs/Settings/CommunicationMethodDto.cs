using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class CommunicationMethodDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateCommunicationMethodDto
{
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
}

public class UpdateCommunicationMethodDto
{
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
}

public class GetCommunicationMethodDto
{
    public long Id { get; set; }
}

public class ListCommunicationMethodsDto : BasePagedRequest
{
}

public class DeleteCommunicationMethodDto
{
    public long Id { get; set; }
}
