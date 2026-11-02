using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Admin.Export;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Export;

namespace LoadlyAggregator.API.Requests.Admin.Drivers;

public class ExportDriversExcelRequest
{
    public const string Permission = ListDriversRequest.Permission;

    public string? Search { get; set; }
    public int? Status { get; set; }
    public List<long>? ContractorIds { get; set; }
    public bool? HasContractor { get; set; }
    public bool? IsAvailable { get; set; }
    public LicenseCategory? LicenseCategory { get; set; }
    public List<DriverWorkingStatus>? WorkingStatuses { get; set; }
    public DateTime? TripsStartDateFrom { get; set; }
    public DateTime? TripsEndDateTo { get; set; }
    public bool? OrderByCompletedTripsDescending { get; set; }
    public List<string>? Columns { get; set; }

    public ExportDriversExcelDto FromRequest() => new()
    {
        Search = Search,
        Status = Status,
        ContractorIds = ContractorIds,
        HasContractor = HasContractor,
        IsAvailable = IsAvailable,
        LicenseCategory = LicenseCategory,
        WorkingStatuses = WorkingStatuses,
        TripsStartDateFrom = TripsStartDateFrom,
        TripsEndDateTo = TripsEndDateTo,
        OrderByCompletedTripsDescending = OrderByCompletedTripsDescending,
        Columns = Columns
    };
}

public class ExportDriversExcelRequestValidator : AbstractValidator<ExportDriversExcelRequest>
{
    public ExportDriversExcelRequestValidator()
    {
        RuleFor(x => x.Search).IsOptionalString(255, "Search term");
        RuleForEach(x => x.ContractorIds)
            .GreaterThan(0).WithMessage("validation.entity_must_be_greater_than_zero|Contractor ID")
            .When(x => x.ContractorIds is { Count: > 0 });
        RuleFor(x => x.Columns)
            .AreValidExportColumnKeys(ExcelExportBuilder.Keys(DriverExportColumns.All));
    }
}
