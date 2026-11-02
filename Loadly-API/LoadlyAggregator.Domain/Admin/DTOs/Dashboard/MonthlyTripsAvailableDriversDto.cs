namespace LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

public class GetMonthlyTripsAvailableDriversDto
{
    public int Year { get; set; }
    public int? Month { get; set; }
}

public class MonthlyTripsAvailableDriversDto
{
    public int Month { get; set; }
    public int TripsCount { get; set; }
    public int AvailableDriversCount { get; set; }
}
