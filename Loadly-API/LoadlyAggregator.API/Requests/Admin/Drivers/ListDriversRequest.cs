using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Drivers;

public class ListDriversRequest : BasePagedRequest
{
    public const string Permission = "view_driver";

    public int? Status { get; set; }
    public List<long>? ContractorIds { get; set; }
    public bool? HasContractor { get; set; }
    public bool? IsAvailable { get; set; }
    public LicenseCategory? LicenseCategory { get; set; }
    public List<DriverWorkingStatus>? WorkingStatuses { get; set; }
    public DateTime? TripsStartDateFrom { get; set; }
    public DateTime? TripsEndDateTo { get; set; }
    public bool? OrderByCompletedTripsDescending { get; set; }

    public ListDriversDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        Status = Status,
        ContractorIds = ContractorIds,
        HasContractor = HasContractor,
        IsAvailable = IsAvailable,
        LicenseCategory = LicenseCategory,
        WorkingStatuses = WorkingStatuses,
        TripsStartDateFrom = TripsStartDateFrom,
        TripsEndDateTo = TripsEndDateTo,
        OrderByCompletedTripsDescending = OrderByCompletedTripsDescending
    };
}

public class ListDriversRequestValidator : AbstractValidator<ListDriversRequest>
{
    public ListDriversRequestValidator()
    {
        RuleForEach(x => x.ContractorIds)
            .GreaterThan(0).WithMessage("validation.entity_must_be_greater_than_zero|Contractor ID")
            .When(x => x.ContractorIds is { Count: > 0 });
    }
}
