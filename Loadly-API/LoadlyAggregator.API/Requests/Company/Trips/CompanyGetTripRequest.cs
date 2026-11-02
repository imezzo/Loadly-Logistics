using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;

namespace LoadlyAggregator.API.Requests.Company.Trips;

public class CompanyGetTripRequest
{
    public long Id { get; set; }

    public Domain.Company.DTOs.Trips.CompanyGetTripDto FromRequest(long companyId) => new()
    {
        Id = Id,
        CompanyId = companyId
    };
}

public class CompanyGetTripRequestValidator : AbstractValidator<CompanyGetTripRequest>
{
    public CompanyGetTripRequestValidator()
    {
        RuleFor(x => x.Id)
            .IsRequiredId("Trip ID");
    }
}
