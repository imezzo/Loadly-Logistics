namespace LoadlyAggregator.Domain.Collector.DTOs.Auth;

public class CollectorValidatePasswordDto
{
    public long UserId { get; set; }

    public string Password { get; set; } = string.Empty;
}


