namespace LoadlyAggregator.Domain.Admin.DTOs.Admins;

public class BulkUpdateAdminStatusDto
{
    public List<long> AdminIds { get; set; } = new();
    public int Status { get; set; }
}
