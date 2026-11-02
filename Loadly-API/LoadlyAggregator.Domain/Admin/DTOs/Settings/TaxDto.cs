using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class TaxDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public TaxType Type { get; set; }
    public decimal Value { get; set; }
    public TaxStatus Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateTaxDto
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public TaxType Type { get; set; }
    public decimal Value { get; set; }
    public TaxStatus Status { get; set; } = TaxStatus.ACTIVE;
}

public class UpdateTaxDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public TaxType Type { get; set; }
    public decimal Value { get; set; }
    public TaxStatus Status { get; set; }
}

public class GetTaxDto
{
    public long Id { get; set; }
}

public class ListTaxesDto : BasePagedRequest
{
    public TaxType? Type { get; set; }
    public TaxStatus? Status { get; set; }
}

public class DeleteTaxDto
{
    public long Id { get; set; }
}
