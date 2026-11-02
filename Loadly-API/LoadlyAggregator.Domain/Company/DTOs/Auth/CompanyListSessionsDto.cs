namespace LoadlyAggregator.Domain.Company.DTOs.Auth;

public class CompanyListSessionsDto
{
    public long UserId { get; set; }
    public string? CurrentTokenFamilyId { get; set; }
}
