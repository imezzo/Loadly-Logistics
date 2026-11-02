using FluentValidation;
using LoadlyAggregator.API.Validators;

namespace LoadlyAggregator.API.Requests.Admin.Vehicles;

public class ImportVehiclesRequest
{
    public const string Permission = "create_vehicle";

    public IFormFile? ExcelFile { get; set; }
}

public class ImportVehiclesRequestValidator : AbstractValidator<ImportVehiclesRequest>
{
    public ImportVehiclesRequestValidator()
    {
        RuleFor(x => x.ExcelFile)
            .NotNull().WithMessage("validation.excel_file_required")
            .File(
                isRequired: true,
                maxSizeInBytes: 10 * 1024 * 1024,
                allowedMimeTypes: new[] { "xlsx", "xls" }
            );
    }
}
