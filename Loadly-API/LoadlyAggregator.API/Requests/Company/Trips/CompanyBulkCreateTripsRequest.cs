using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Company.DTOs.Trips;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Company.Trips;

public class CompanyBulkCreateTripsRequest
{
    public const string Permission = "create_trip";

    public long ContractId { get; set; }
    public long VehicleTypeId { get; set; }
    public List<CompanyBulkCreateTripItemRequest> Trips { get; set; } = new();

    public CompanyBulkCreateTripsDto FromRequest(long companyId) => new()
    {
        CompanyId = companyId,
        ContractId = ContractId,
        VehicleTypeId = VehicleTypeId,
        Trips = (Trips ?? new List<CompanyBulkCreateTripItemRequest>())
            .Select(t => new CompanyBulkCreateTripItemDto
            {
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                From = t.From,
                To = t.To,
                DistanceKms = t.DistanceKms,
                ContactName = t.ContactName,
                ContactMobile = t.ContactMobile,
                HelpersCount = t.HelpersCount
            }).ToList()
    };
}

public class CompanyBulkCreateTripItemRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string From { get; set; } = string.Empty;
    public string To { get; set; } = string.Empty;
    public decimal? DistanceKms { get; set; }
    public string? ContactName { get; set; }
    public string? ContactMobile { get; set; }
    public int? HelpersCount { get; set; }
}

public class CompanyBulkCreateTripsRequestValidator : AbstractValidator<CompanyBulkCreateTripsRequest>
{
    public CompanyBulkCreateTripsRequestValidator()
    {
        RuleFor(x => x.ContractId)
            .IsRequiredId("Contract ID");

        RuleFor(x => x.VehicleTypeId)
            .IsRequiredId("Vehicle type ID");

        RuleFor(x => x.Trips)
            .IsRequiredNonEmptyCollection("Trips list");

        RuleForEach(x => x.Trips).ChildRules(trip =>
        {
            trip.RuleFor(t => t.StartDate)
                .IsRequiredDate("Start date")
                .IsFutureDate("Start date");

            trip.RuleFor(t => t.EndDate)
                .IsRequiredDate("End date")
                .IsAfterDate(t => t.StartDate, "End date", "Start date");

            trip.RuleFor(t => t.From)
                .NotEmpty().WithMessage("validation.from_required");

            trip.RuleFor(t => t.To)
                .NotEmpty().WithMessage("validation.to_required");

            trip.When(t => !string.IsNullOrWhiteSpace(t.ContactName), () =>
            {
                trip.RuleFor(t => t.ContactName)
                    .MaximumLength(255).WithMessage("validation.must_not_exceed_characters|255");
            });

            trip.When(t => !string.IsNullOrWhiteSpace(t.ContactMobile), () =>
            {
                trip.RuleFor(t => t.ContactMobile)
                    .MaximumLength(20).WithMessage("validation.must_not_exceed_characters|20");
            });

            trip.When(t => t.HelpersCount.HasValue, () =>
            {
                trip.RuleFor(t => t.HelpersCount)
                    .GreaterThanOrEqualTo(1).WithMessage(TripMessageKeys.HelpersCountMustBeAtLeastOne)
                    .LessThanOrEqualTo(10).WithMessage(TripMessageKeys.HelpersCountMustNotExceed10);
            });
        });
    }
}
