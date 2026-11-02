using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Contractor.Drivers;

public class ContractorListDriversRequest : BasePagedRequest
{
    public int? Status { get; set; }
    public bool? IsAvailable { get; set; }
    public LicenseCategory? LicenseCategory { get; set; }
    public List<DriverWorkingStatus>? WorkingStatuses { get; set; }

    public Domain.Contractor.DTOs.Drivers.ContractorListDriversDto FromRequest(long contractorId) => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        ContractorId = contractorId,
        Status = Status,
        IsAvailable = IsAvailable,
        LicenseCategory = LicenseCategory,
        WorkingStatuses = WorkingStatuses
    };
}

public class ContractorListDriversRequestValidator : AbstractValidator<ContractorListDriversRequest>
{
    public ContractorListDriversRequestValidator()
    {
        RuleFor(x => x.Page).IsGreaterThanZero("Page");
        RuleFor(x => x.PageSize)
            .IsGreaterThanZero("Page size")
            .LessThanOrEqualTo(100).WithMessage("validation.page_size_must_not_exceed|100");
    }
}
