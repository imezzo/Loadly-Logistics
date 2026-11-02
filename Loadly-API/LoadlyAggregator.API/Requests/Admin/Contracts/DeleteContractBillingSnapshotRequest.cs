using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class DeleteContractBillingSnapshotRequest
{
    public const string Permission = "delete_contract_billing_snapshot";

    [FromRoute(Name = "snapshotId")]
    public long SnapshotId { get; set; }

    public DeleteContractBillingSnapshotDto FromRequest() => new() { SnapshotId = SnapshotId };
}

public class DeleteContractBillingSnapshotRequestValidator : AbstractValidator<DeleteContractBillingSnapshotRequest>
{
    public DeleteContractBillingSnapshotRequestValidator(IContractBillingSnapshotRepository snapshotRepository)
    {
        RuleFor(x => x.SnapshotId).IsRequiredId("Billing snapshot ID");

        RuleFor(x => x)
            .MustAsync(async (request, cancellationToken) =>
            {
                if (request.SnapshotId <= 0)
                    return false;

                var snapshot = await snapshotRepository.GetByIdAsync(request.SnapshotId);
                return snapshot is not null
                    && !ContractBillingSnapshotInvoicingProgress.IsBillingLocked(snapshot.Status);
            })
            .WithMessage("validation.delete_contract_billing_snapshot_draft_only")
            .When(x => x.SnapshotId > 0);
    }
}
