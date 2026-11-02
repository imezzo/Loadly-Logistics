using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.Domain.Driver.DTOs.Auth;



public class DriverUpdateProfileDto
{
    public long UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string FullNameAr { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public FileUpload? ProfilePicture { get; set; }
    public bool? BiometricFingerprint { get; set; }
}
