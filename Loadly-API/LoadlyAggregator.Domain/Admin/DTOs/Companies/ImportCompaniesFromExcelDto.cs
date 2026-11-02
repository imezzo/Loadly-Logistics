namespace LoadlyAggregator.Domain.Admin.DTOs.Companies;

public class ImportCompaniesFromExcelDto
{
    /// <summary>Called by the action to open the Excel stream.</summary>
    public Func<Stream> OpenStream { get; set; } = null!;
}
