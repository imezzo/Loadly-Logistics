using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Leads;

namespace LoadlyAggregator.API.Requests.Admin.Leads;

public class GetLeadByPhoneRequest
{
    public const string Permission = "view_lead";

    public string Phone { get; set; } = string.Empty;

    public GetLeadByPhoneDto FromRequest() => new()
    {
        Phone = Phone
    };
}

public class GetLeadByPhoneRequestValidator : AbstractValidator<GetLeadByPhoneRequest>
{
    public GetLeadByPhoneRequestValidator()
    {
        RuleFor(x => x.Phone).IsRequiredPhone();
    }
}
