using FluentValidation;
using LoadlyAggregator.API.Requests.Contractor.Drivers;
using LoadlyAggregator.Domain.Contractor.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Contractor.Trips;

public class ContractorChangeTripDriverRequest
{
    public long? DriverId { get; set; }
    public ContractorCreateDriverRequest? Driver { get; set; }
    public string? Note { get; set; }

    public ContractorChangeTripDriverDto FromRequest(long tripId, long contractorId) => new()
    {
        TripId = tripId,
        ContractorId = contractorId,
        DriverId = DriverId,
        Note = Note,
        Driver = Driver == null || DriverId is > 0
            ? null
            : Driver.FromRequest(contractorId)
    };
}

public class ContractorChangeTripDriverRequestValidator : AbstractValidator<ContractorChangeTripDriverRequest>
{
    public ContractorChangeTripDriverRequestValidator(IValidator<ContractorCreateDriverRequest> createDriverValidator)
    {
        RuleFor(x => x)
            .Must(x => x.DriverId is > 0 || x.Driver != null)
            .WithMessage("trips.booking_driver_required");

        RuleFor(x => x.Driver!)
            .SetValidator(createDriverValidator)
            .When(x => x.Driver != null && x.DriverId is null or <= 0);
    }
}
