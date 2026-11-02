using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class ContainerDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateContainerDto
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public ContainerStatus Status { get; set; }
}

public class UpdateContainerDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public ContainerStatus Status { get; set; }
}

public class GetContainerDto
{
    public long Id { get; set; }
}

public class DeleteContainerDto
{
    public long Id { get; set; }
}

public class ListContainersDto : BasePagedRequest
{
    public ContainerStatus? Status { get; set; }
}
