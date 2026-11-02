using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Contracts;

public class ListContractBillingSnapshotsDto : BasePagedRequest
{
    public long? ContractId { get; set; }
    public ContractBillingSnapshotStatus? Status { get; set; }
    public DateOnly? PeriodStart { get; set; }
    public DateOnly? PeriodEnd { get; set; }
}
