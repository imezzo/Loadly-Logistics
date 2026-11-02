using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class ShippingLineDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public int Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateShippingLineDto
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public ShippingLineStatus Status { get; set; }
}

public class UpdateShippingLineDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public ShippingLineStatus Status { get; set; }
}

public class GetShippingLineDto
{
    public long Id { get; set; }
}

public class DeleteShippingLineDto
{
    public long Id { get; set; }
}

public class ListShippingLinesDto : BasePagedRequest
{
    public ShippingLineStatus? Status { get; set; }
}
