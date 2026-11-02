using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.ContractApprovals;

public class SubmitContractApprovalDecisionRequest
{
    [FromRoute]
    public long Id { get; set; }

    public ContractApprovalDecision Action { get; set; }
    public string? Comments { get; set; }

    public ContractApprovalDecisionDto FromRequest() => new()
    {
        ContractId = Id,
        Action = Action,
        Comments = Comments
    };
}

public class SubmitContractApprovalDecisionRequestValidator : AbstractValidator<SubmitContractApprovalDecisionRequest>
{
    public SubmitContractApprovalDecisionRequestValidator(IContractRepository contractRepository)
    {
        RuleFor(x => x.Id)
            .IsRequiredId("Contract ID");

        RuleFor(x => x.Action)
            .IsInEnum().WithMessage("validation.contract_approval_action_invalid");

        RuleFor(x => x.Comments)
            .IsOptionalStringWithMaxLength(1000, "Comments")
            .When(x => !string.IsNullOrWhiteSpace(x.Comments));
    }
}
