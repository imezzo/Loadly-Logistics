using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.FuelTypes;

public class DeleteFuelTypeRequest
{
    public const string Permission = "delete_fuel_type";
    public long Id { get; set; }
    public DeleteFuelTypeDto FromRequest() => new() { Id = Id };
}
