using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilities;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Settings.VehicleFacilities;

public class ListVehicleFacilitiesRequest : BasePagedRequest
{
    public const string Permission = "view_vehicle_facility";

    public VehicleFacilityStatus? Status { get; set; }
    public VehicleFacilityType? FacilityType { get; set; }
    public List<long>? CompanyIds { get; set; }

    public ListVehicleFacilitiesDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        Status = Status,
        FacilityType = FacilityType,
        CompanyIds = CompanyIds
    };
}

public class ListVehicleFacilitiesRequestValidator : AbstractValidator<ListVehicleFacilitiesRequest>
{
    public ListVehicleFacilitiesRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        When(x => x.Status.HasValue, () =>
            RuleFor(x => x.Status!.Value).IsValidEnum<ListVehicleFacilitiesRequest, VehicleFacilityStatus>("Status"));
        When(x => x.FacilityType.HasValue, () =>
            RuleFor(x => x.FacilityType!.Value).IsValidEnum<ListVehicleFacilitiesRequest, VehicleFacilityType>("Location type"));
        RuleForEach(x => x.CompanyIds)
            .GreaterThan(0L).WithMessage("validation.entity_must_be_greater_than_zero|Company ID")
            .When(x => x.CompanyIds is { Count: > 0 });
    }
}
