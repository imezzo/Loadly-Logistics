using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class DiscountDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public DiscountStatus Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateDiscountDto
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public DiscountStatus Status { get; set; } = DiscountStatus.ACTIVE;
}

public class UpdateDiscountDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public DiscountType Type { get; set; }
    public decimal Value { get; set; }
    public DiscountStatus Status { get; set; }
}

public class GetDiscountDto
{
    public long Id { get; set; }
}

public class ListDiscountsDto : BasePagedRequest
{
    public DiscountType? Type { get; set; }
    public DiscountStatus? Status { get; set; }
}

public class DeleteDiscountDto
{
    public long Id { get; set; }
}
