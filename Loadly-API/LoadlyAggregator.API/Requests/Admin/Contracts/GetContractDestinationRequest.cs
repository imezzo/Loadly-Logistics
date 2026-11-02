using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class GetContractDestinationRequest
{
    public const string Permission = GetContractRequest.Permission;

    public long DestinationId { get; set; }

    public GetContractDestinationDto FromRequest() => new() { Id = DestinationId };
}

public class GetContractDestinationRequestValidator : AbstractValidator<GetContractDestinationRequest>
{
    public GetContractDestinationRequestValidator()
    {
        RuleFor(x => x.DestinationId).IsRequiredId("Destination ID");
    }
}
