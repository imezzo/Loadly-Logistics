using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.CommunicationMethods;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.CommunicationMethods;

public class CreateCommunicationMethodRequest
{
    public const string Permission = "create_communication_method";

    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }

    public CreateCommunicationMethodDto FromRequest() => new()
    {
        NameEn = NameEn,
        NameAr = NameAr
    };
}

public class CreateCommunicationMethodRequestValidator : AbstractValidator<CreateCommunicationMethodRequest>
{
    public CreateCommunicationMethodRequestValidator(ICommunicationMethodRepository communicationMethodRepository)
    {
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async nameEn => await communicationMethodRepository.GetByNameAsync(nameEn) == null, _ => "communication_methods.name_already_exists");

        RuleFor(x => x.NameAr)
            .IsOptionalArabicName()
            .When(x => !string.IsNullOrWhiteSpace(x.NameAr));
    }
}
