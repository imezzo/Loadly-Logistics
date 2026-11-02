using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Trips;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class BulkCreateTripsRequest
{
    public const string Permission = "create_trip";

    public long ContractId { get; set; }
    public List<BulkTripItemRequest> Trips { get; set; } = new();

    public BulkCreateTripsDto FromRequest() => new()
    {
        ContractId = ContractId,
        Trips = Trips.Select(t => new BulkTripItemDto
        {
            VehicleTypeId = t.VehicleTypeId,
            DestinationId = t.DestinationId,
            VehicleId = t.VehicleId,
            DriverId = t.DriverId,
            StartDate = t.StartDate,
            EndDate = t.EndDate,
            FinancialDate = t.FinancialDate,
            ContractDistanceKm = t.ContractDistanceKm
        }).ToList()
    };
}

public class BulkTripItemRequest
{
    public long VehicleTypeId { get; set; }
    public long? DestinationId { get; set; }
    public long VehicleId { get; set; }  // Head vehicle (required)
    public long? DriverId { get; set; }  // Driver (optional)
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateOnly? FinancialDate { get; set; }
    public decimal? ContractDistanceKm { get; set; }
}

public class BulkCreateTripsRequestValidator : AbstractValidator<BulkCreateTripsRequest>
{
    public BulkCreateTripsRequestValidator()
    {
        RuleFor(x => x.ContractId)
            .IsRequiredId("Contract ID");

        RuleFor(x => x.Trips)
            .IsRequiredNonEmptyCollection("Trips list");

        RuleForEach(x => x.Trips).ChildRules(trip =>
        {
            trip.RuleFor(t => t.VehicleTypeId)
                .IsRequiredId("Vehicle type ID");

            trip.RuleFor(t => t.DestinationId)
                .IsOptionalId("Destination ID");

            trip.RuleFor(t => t.VehicleId)
                .IsRequiredId("Vehicle ID");

            // Driver ID validation (optional)
            trip.When(t => t.DriverId.HasValue, () =>
            {
                trip.RuleFor(t => t.DriverId)
                    .GreaterThan(0).WithMessage("validation.driver_id_gt_zero");
            });

            trip.RuleFor(t => t.StartDate)
                .IsRequiredDate("Start date");

            trip.RuleFor(t => t.EndDate)
                .IsRequiredDate("End date")
                .IsAfterDate(t => t.StartDate, "End date", "Start date");
        });
    }
}
