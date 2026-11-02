using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.Domain.Admin.DTOs.Drivers;

public class DriverDocumentDto
{
    public long Id { get; set; }
    public long DocumentId { get; set; }
    public long DriverId { get; set; }
    public string? Path { get; set; }
    public string? FileUrl { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DocumentVerificationStatus Status { get; set; }
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }


    public DocumentDto? Document { get; set; }
}

public class CreateDriverDocumentDto
{
    public long DocumentId { get; set; }
    public long DriverId { get; set; }
    public FileUpload? File { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DocumentVerificationStatus Status { get; set; } = DocumentVerificationStatus.PENDING;
    public long? AddedBy { get; set; }
}

public class GetDriverDocumentDto
{
    public long Id { get; set; }
}

public class ListDriverDocumentsDto : BasePagedRequest
{
    public long? DriverId { get; set; }
    public long? DocumentId { get; set; }
    public DocumentVerificationStatus? Status { get; set; }
}

public class UpdateDriverDocumentDto
{
    public long Id { get; set; }
    public long DocumentId { get; set; }
    public long DriverId { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DocumentVerificationStatus Status { get; set; }
}

public class DeleteDriverDocumentDto
{
    public long Id { get; set; }
}

public class DownloadDriverDocumentDto
{
    public long Id { get; set; }
}

