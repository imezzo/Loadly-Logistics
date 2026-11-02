namespace LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;

public class BulkSetContractorPayoutsPaidDto
{
    public IReadOnlyList<long> PayoutIds { get; set; } = [];
}
