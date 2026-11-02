namespace LoadlyAggregator.Domain.Admin.DTOs.Auth;



public class AdminUpdateProfileDto
{
    public long UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}
