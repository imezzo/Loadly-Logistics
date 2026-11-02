using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Leads;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Leads;

public class CreateLeadRequest
{
    public const string Permission = "create_lead";

    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }

    public CreateLeadDto FromRequest() => new()
    {
        FullName = FullName,
        Phone = Phone,
        Email = Email
    };
}

public class CreateLeadRequestValidator : AbstractValidator<CreateLeadRequest>
{
    public CreateLeadRequestValidator(ILeadRepository leadRepository)
    {
        RuleFor(x => x.FullName).IsRequiredFullName();
        RuleFor(x => x.Phone)
            .IsRequiredPhone()
            .IsUnique(async phone => await leadRepository.GetByPhoneAsync(phone) == null, _ => "auth.phone_already_taken");
        RuleFor(x => x.Email).IsOptionalEmail();
    }
}
