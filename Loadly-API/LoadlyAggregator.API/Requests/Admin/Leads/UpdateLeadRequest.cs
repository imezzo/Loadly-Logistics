using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Leads;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Leads;

public class UpdateLeadRequest
{
    public const string Permission = "edit_lead";

    [FromRoute]
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }

    public UpdateLeadDto FromRequest() => new()
    {
        Id = Id,
        FullName = FullName,
        Phone = Phone,
        Email = Email
    };
}

public class UpdateLeadRequestValidator : AbstractValidator<UpdateLeadRequest>
{
    public UpdateLeadRequestValidator(ILeadRepository leadRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Lead ID");
        RuleFor(x => x.FullName).IsRequiredFullName();
        RuleFor(x => x.Phone)
            .IsRequiredPhone()
            .IsUnique(async (phone, excludeId) => !await leadRepository.ExistsByPhoneAsync(phone, excludeId), x => x.Id, _ => "auth.phone_already_taken");
        RuleFor(x => x.Email).IsOptionalEmail();
    }
}
