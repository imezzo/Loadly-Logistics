using FluentValidation;
using LoadlyAggregator.API.Validators;

namespace LoadlyAggregator.API.Requests.Admin.Drivers;

public class ImportDriversRequest
{
    public const string Permission = "create_driver";

    public IFormFile? ExcelFile { get; set; }
}

public class ImportDriversRequestValidator : AbstractValidator<ImportDriversRequest>
{
    public ImportDriversRequestValidator()
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
