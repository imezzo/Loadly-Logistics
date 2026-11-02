using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Contractors;

public class ListContractorsDto : BasePagedRequest
{
    public ContractorStatus? ContractorStatus { get; set; }
    public UserStatus? Status { get; set; }
    public DateTime? TripStartDate { get; set; }
    public DateTime? TripEndDate { get; set; }
}
