namespace LoadlyAggregator.Domain.Driver.DTOs.Auth;

public class DriverListSessionsDto
{
    public long UserId { get; set; }
    public string? CurrentTokenFamilyId { get; set; }
}
