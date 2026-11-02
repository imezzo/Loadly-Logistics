using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class DeleteContractRequest
{
    public const string Permission = "delete_contract";

    [FromRoute]
    public long Id { get; set; }

    public DeleteContractDto FromRequest() => new() { Id = Id };
}

public class DeleteContractRequestValidator : AbstractValidator<DeleteContractRequest>
{
    public DeleteContractRequestValidator(IContractRepository contractRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Contract ID");

        RuleFor(x => x)
            .MustAsync(async (request, cancellationToken) =>
            {
                if (request.Id <= 0) return false;
                var contract = await contractRepository.GetByIdAsync(request.Id);
                return contract != null && contract.Status == ContractStatus.DRAFT;
            })
            .WithMessage("validation.delete_contract_draft_only")
            .When(x => x.Id > 0);
    }
}
