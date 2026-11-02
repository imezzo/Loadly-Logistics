using FluentValidation;
using LoadlyAggregator.API.Validators;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class ImportTripsRequest
{
    public const string Permission = "create_trip";

    public IFormFile? ExcelFile { get; set; }
    public long ContractId { get; set; }

    public ImportTripsFromExcelDto FromRequest() => new()
    {
        OpenStream = () => ExcelFile!.OpenReadStream(),
        ContractId = ContractId
    };
}

public class ImportTripsRequestValidator : AbstractValidator<ImportTripsRequest>
{
    public ImportTripsRequestValidator()
    {
        RuleFor(x => x.ExcelFile)
            .NotNull().WithMessage("validation.excel_file_required")
            .File(
                isRequired: true,
                maxSizeInBytes: 10 * 1024 * 1024,
                allowedMimeTypes: new[] { "xlsx", "xls" });

        RuleFor(x => x.ContractId)
            .IsRequiredId("Contract ID");
    }
}
