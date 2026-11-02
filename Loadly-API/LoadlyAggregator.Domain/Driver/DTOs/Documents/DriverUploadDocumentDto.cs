using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.Domain.Driver.DTOs.Documents;

public class DriverUploadDocumentDto
{
    public long DriverId { get; set; }
    public long DocumentId { get; set; }
    public required FileUpload File { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
