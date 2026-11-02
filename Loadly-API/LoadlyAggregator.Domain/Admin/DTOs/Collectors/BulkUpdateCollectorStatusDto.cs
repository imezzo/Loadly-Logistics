namespace LoadlyAggregator.Domain.Admin.DTOs.Collectors;

public class BulkUpdateCollectorStatusDto
{
    public List<long> CollectorIds { get; set; } = new();
    public int Status { get; set; }
}
