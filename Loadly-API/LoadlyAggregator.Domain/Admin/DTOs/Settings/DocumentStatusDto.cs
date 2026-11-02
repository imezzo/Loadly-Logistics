namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class DocumentStatusDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public long? AddedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CreateDocumentStatusDto
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
}

public class UpdateDocumentStatusDto
{
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
}

public class GetDocumentStatusDto
{
    public long Id { get; set; }
}

public class DeleteDocumentStatusDto
{
    public long Id { get; set; }
}
