using FluentValidation;
using LoadlyAggregator.API.Validators;

namespace LoadlyAggregator.API.Requests.Admin.Companies;

public class ImportCompaniesRequest
{
    public const string Permission = "create_company";

    public IFormFile? ExcelFile { get; set; }
}

public class ImportCompaniesRequestValidator : AbstractValidator<ImportCompaniesRequest>
{
    public ImportCompaniesRequestValidator()
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
