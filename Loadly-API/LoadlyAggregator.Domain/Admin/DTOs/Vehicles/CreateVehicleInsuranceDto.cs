namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class CreateVehicleInsuranceDto
{
    public string PolicyNo { get; set; } = string.Empty;
    public string InsuranceProvider { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
}
