namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class AuditTripDto
{
    public long Id { get; set; }
    public string? ProofOfDelivery { get; set; }
    public bool ConfirmFinanceApproval { get; set; }
    public DateOnly FinancialDate { get; set; }
    public IReadOnlyList<UpdateTripChargeItemDto> TripCharges { get; set; } = [];
}
