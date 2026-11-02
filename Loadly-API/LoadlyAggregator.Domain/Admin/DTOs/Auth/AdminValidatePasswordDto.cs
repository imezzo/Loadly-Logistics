namespace LoadlyAggregator.Domain.Admin.DTOs.Auth;

public class AdminValidatePasswordDto
{
    public long UserId { get; set; }

    public string Password { get; set; } = string.Empty;
}


