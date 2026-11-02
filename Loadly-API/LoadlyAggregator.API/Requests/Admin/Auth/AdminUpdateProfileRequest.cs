using LoadlyAggregator.Domain.Admin.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Admin.Auth;

public class AdminUpdateProfileRequest
{
    public long UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public AdminUpdateProfileDto FromRequest() => new()
    {
        UserId = UserId,
        FullName = FullName,
        UserName = UserName,
        Email = Email,
        Phone = Phone
    };
}
