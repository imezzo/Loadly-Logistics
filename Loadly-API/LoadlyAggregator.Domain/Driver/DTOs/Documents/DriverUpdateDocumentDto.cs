using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.Domain.Driver.DTOs.Documents;

public class DriverUpdateDocumentDto
{
    public long Id { get; set; }
    public long DriverId { get; set; }
    public FileUpload? File { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
