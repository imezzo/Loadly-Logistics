namespace LoadlyAggregator.Domain.Driver.DTOs.Auth;

public class DriverValidatePasswordDto
{
    public long UserId { get; set; }

    public string Password { get; set; } = string.Empty;
}


