using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.FuelTypes;

public class GetFuelTypeRequest
{
    public const string Permission = "view_fuel_type";
    public long Id { get; set; }
    public GetFuelTypeDto FromRequest() => new() { Id = Id };
}
