using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class ExportContractBillingSnapshotExcelRequest
{
    public const string Permission = GetContractBillingSnapshotRequest.Permission;

    [FromRoute(Name = "snapshotId")]
    public long SnapshotId { get; set; }

    public ExportContractBillingSnapshotExcelDto FromRequest() => new()
    {
        SnapshotId = SnapshotId
    };
}

public class ExportContractBillingSnapshotExcelRequestValidator
    : AbstractValidator<ExportContractBillingSnapshotExcelRequest>
{
    public ExportContractBillingSnapshotExcelRequestValidator()
    {
        RuleFor(x => x.SnapshotId).IsRequiredId("Billing snapshot ID");
    }
}
