namespace LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;

public class SetContractorPayoutCancelledDto
{
    public long Id { get; set; }
    public string? CancellationReason { get; set; }
}
