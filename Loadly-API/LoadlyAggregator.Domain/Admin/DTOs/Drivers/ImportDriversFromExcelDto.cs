namespace LoadlyAggregator.Domain.Admin.DTOs.Drivers;

public class ImportDriversFromExcelDto
{
    /// <summary>Called by the action to open the Excel stream.</summary>
    public Func<Stream> OpenStream { get; set; } = null!;
}
