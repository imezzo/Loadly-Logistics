using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Contracts;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class GetContractRequest
{
    public const string Permission = "view_contract";

    public long Id { get; set; }

    public GetContractDto FromRequest() => new() { Id = Id };
}

public class GetContractRequestValidator : AbstractValidator<GetContractRequest>
{
    public GetContractRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Contract ID");
    }
}
