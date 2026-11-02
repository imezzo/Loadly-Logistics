using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Admin.Export;
using LoadlyAggregator.Domain.Common.Export;

namespace LoadlyAggregator.API.Requests.Admin.Dashboard;

public class ExportDriverLifecycleTripsRequest
{
    public const string Permission = ListDriverCompletedTripsByLifecycleActorRequest.Permission;

    public long? DriverId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public List<string>? Columns { get; set; }

    public ExportDriverLifecycleTripsDto FromRequest() => new()
    {
        DriverId = DriverId,
        FromDate = FromDate,
        ToDate = ToDate,
        Columns = Columns
    };
}

public class ExportDriverLifecycleTripsRequestValidator
    : AbstractValidator<ExportDriverLifecycleTripsRequest>
{
    public ExportDriverLifecycleTripsRequestValidator()
    {
        RuleFor(x => x.DriverId)
            .IsOptionalId("Driver ID")
            .When(x => x.DriverId.HasValue);

        RuleFor(x => x.ToDate)
            .IsGreaterThanOrEqualToDate(x => x.FromDate!.Value, "End date", "Start date")
            .When(x => x.FromDate.HasValue && x.ToDate.HasValue);

        RuleFor(x => x.Columns)
            .AreValidExportColumnKeys(ExcelExportBuilder.Keys(DriverLifecycleTripsExportColumns.All));
    }
}
