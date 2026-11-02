using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;

public class ListContractorPayoutsDto : BasePagedRequest
{
    public long? ContractorId { get; set; }
    public ContractorPayoutStatus? Status { get; set; }
}
