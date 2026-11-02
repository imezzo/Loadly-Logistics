using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Contractor.DTOs.Drivers;

public class ContractorListDriversDto : BasePagedRequest
{
    public long ContractorId { get; set; }
    public int? Status { get; set; }
    public bool? IsAvailable { get; set; }
    public LicenseCategory? LicenseCategory { get; set; }
    public List<DriverWorkingStatus>? WorkingStatuses { get; set; }
}
