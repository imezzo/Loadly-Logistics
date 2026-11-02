using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Contractor.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Contractor.Trips;

public class ContractorListBookingRequestsRequest : BasePagedRequest
{
    public TripBookingRequestStatus? Status { get; set; }
    public long? TripId { get; set; }
    public DateTime? StartDateFrom { get; set; }
    public DateTime? StartDateTo { get; set; }
    public long? VehicleTypeId { get; set; }

    public ContractorListBookingRequestsDto FromRequest(long contractorId) => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        ContractorId = contractorId,
        Status = Status,
        TripId = TripId,
        StartDateFrom = StartDateFrom,
        StartDateTo = StartDateTo,
        VehicleTypeId = VehicleTypeId
    };
}

public class ContractorListBookingRequestsRequestValidator : AbstractValidator<ContractorListBookingRequestsRequest>
{
    public ContractorListBookingRequestsRequestValidator()
    {
        RuleFor(x => x.Page).IsGreaterThanZero("Page");
        RuleFor(x => x.PageSize)
            .IsGreaterThanZero("Page size")
            .LessThanOrEqualTo(100).WithMessage("validation.page_size_must_not_exceed|100");

        RuleFor(x => x.Search).IsOptionalString(255, "Search term");

        RuleFor(x => x.TripId)
            .IsOptionalId("Trip ID")
            .When(x => x.TripId.HasValue);

        RuleFor(x => x.VehicleTypeId)
            .IsOptionalId("Vehicle type ID")
            .When(x => x.VehicleTypeId.HasValue);

        RuleFor(x => x.StartDateTo)
            .IsGreaterThanOrEqualToDate(x => x.StartDateFrom!.Value, "End date", "Start date")
            .When(x => x.StartDateFrom.HasValue && x.StartDateTo.HasValue);

        When(x => x.Status.HasValue, () =>
            RuleFor(x => x.Status).IsInEnum().WithMessage("validation.trip_booking_request_status_enum_invalid"));
    }
}

public class ContractorGetBookingRequestRequest
{
    public long Id { get; set; }

    public ContractorGetBookingRequestDto FromRequest(long contractorId) => new()
    {
        Id = Id,
        ContractorId = contractorId
    };
}

public class ContractorGetBookingRequestRequestValidator : AbstractValidator<ContractorGetBookingRequestRequest>
{
    public ContractorGetBookingRequestRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Booking request ID");
    }
}
