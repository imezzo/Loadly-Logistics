using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Vehicles;

public class ListVehicleAssignmentHistoriesRequest : BasePagedRequest
{
    public const string Permission = "view_vehicle";

    public VehicleAssignmentHistorySource? Source { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }

    public ListVehicleAssignmentHistoriesDto FromRequest(long vehicleId) => new()
    {
        VehicleId = vehicleId,
        Page = Page,
        PageSize = PageSize,
        Source = Source,
        CreatedFrom = CreatedFrom,
        CreatedTo = CreatedTo
    };
}

public class ListVehicleAssignmentHistoriesRequestValidator : AbstractValidator<ListVehicleAssignmentHistoriesRequest>
{
    public ListVehicleAssignmentHistoriesRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.CreatedTo)
            .GreaterThanOrEqualTo(x => x.CreatedFrom!.Value)
            .When(x => x.CreatedFrom.HasValue && x.CreatedTo.HasValue)
            .WithMessage("validation.created_to_must_be_on_or_after_created_from");
    }
}
