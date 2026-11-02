namespace LoadlyAggregator.Domain.Admin.DTOs.Auth;

public class AdminListSessionsDto
{
    public long UserId { get; set; }
    public string? CurrentTokenFamilyId { get; set; }
}
