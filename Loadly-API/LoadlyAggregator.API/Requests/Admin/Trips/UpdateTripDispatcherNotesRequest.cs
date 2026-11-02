using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class UpdateTripDispatcherNotesRequest
{
    public const string Permission = "edit_trip";

    public string? DispatcherNotes { get; set; }

    public UpdateTripDispatcherNotesDto FromRequest(long id) => new()
    {
        Id = id,
        DispatcherNotes = DispatcherNotes
    };
}
