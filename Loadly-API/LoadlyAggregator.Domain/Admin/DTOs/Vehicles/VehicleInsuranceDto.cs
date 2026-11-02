namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class VehicleInsuranceDto
{
    public long Id { get; set; }
    public string PolicyNo { get; set; } = string.Empty;
    public string InsuranceProvider { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public int Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
