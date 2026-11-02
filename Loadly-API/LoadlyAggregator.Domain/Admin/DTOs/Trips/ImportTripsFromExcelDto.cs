namespace LoadlyAggregator.Domain.Admin.DTOs.Trips;

public class ImportTripsFromExcelDto
{
    /// <summary>Called by the action to open the Excel stream.</summary>
    public Func<Stream> OpenStream { get; set; } = null!;

    public long ContractId { get; set; }
}
