namespace LoadlyAggregator.Domain.Contractor.DTOs.Auth;



public class ContractorLoginDto
{
    public string Login { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? DeviceInfoJson { get; set; }
    public string? FcmToken { get; set; }
    public string? DeviceId { get; set; }
    public string? DeviceModel { get; set; }
    public string? DeviceOs { get; set; }
    public string? AppVersion { get; set; }
}
