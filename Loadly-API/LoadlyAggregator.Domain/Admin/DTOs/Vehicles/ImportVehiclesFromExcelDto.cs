namespace LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

public class ImportVehiclesFromExcelDto
{
    public Func<Stream> OpenStream { get; set; } = null!;
}
