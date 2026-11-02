using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.Export;
using LoadlyAggregator.Domain.Common.Export;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class ExportContractDestinationsExcelRequest
{
    public const string Permission = GetContractRequest.Permission;

    public long Id { get; set; }
    public List<string>? Columns { get; set; }

    public ExportContractDestinationsExcelDto FromRequest() => new()
    {
        Id = Id,
        Columns = Columns
    };
}

public class ExportContractDestinationsExcelRequestValidator : AbstractValidator<ExportContractDestinationsExcelRequest>
{
    public ExportContractDestinationsExcelRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Contract ID");
        RuleFor(x => x.Columns)
            .AreValidExportColumnKeys(ExcelExportBuilder.Keys(ContractDestinationExportColumns.All));
    }
}
