using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Resources.Admin.Settings;

/// <summary>
/// Resource for VehicleInsurance API responses (both listing and single get)
/// </summary>
public class VehicleInsuranceResource
{
    public long Id { get; set; }
    public string PolicyNo { get; set; } = string.Empty;
    public string InsuranceProvider { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public int Status { get; set; }

    public static VehicleInsuranceResource FromDto(VehicleInsuranceDto dto) =>
        new VehicleInsuranceResource
        {
            Id = dto.Id,
            PolicyNo = dto.PolicyNo,
            InsuranceProvider = dto.InsuranceProvider,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Amount = dto.Amount,
            Status = dto.Status
        };
}
