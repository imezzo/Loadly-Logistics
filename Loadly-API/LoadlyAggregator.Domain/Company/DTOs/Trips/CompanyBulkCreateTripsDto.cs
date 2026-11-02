namespace LoadlyAggregator.Domain.Company.DTOs.Trips;

public class CompanyBulkCreateTripsDto
{
    public long CompanyId { get; set; }
    public long ContractId { get; set; }
    public long VehicleTypeId { get; set; }
    public List<CompanyBulkCreateTripItemDto> Trips { get; set; } = new();
}

public class CompanyBulkCreateTripItemDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public decimal? DistanceKms { get; set; }
    public string? ContactName { get; set; }
    public string? ContactMobile { get; set; }
    public int? HelpersCount { get; set; }
}
