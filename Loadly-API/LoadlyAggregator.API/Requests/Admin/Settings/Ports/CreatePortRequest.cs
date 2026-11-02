using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Ports;

public class CreatePortRequest
{
    public const string Permission = "create_port";

    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? PortCode { get; set; }
    public PortType Type { get; set; }
    public PortStatus Status { get; set; } = PortStatus.ACTIVE;

    public CreatePortDto FromRequest() => new()
    {
        NameAr = NameAr,
        NameEn = NameEn,
        Country = Country,
        PortCode = PortCode,
        Type = Type,
        Status = Status
    };
}

public class CreatePortRequestValidator : AbstractValidator<CreatePortRequest>
{
    public CreatePortRequestValidator(IPortRepository portRepository)
    {
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.NameEn).IsOptionalString(255, "NameEn");
        RuleFor(x => x.Country).IsRequiredString(255, "Country");
        RuleFor(x => x.PortCode).IsOptionalString(50, "PortCode");
        RuleFor(x => x.Type)
            .IsValidEnum<CreatePortRequest, PortType>("Type");
        RuleFor(x => x.Status)
            .IsValidEnum<CreatePortRequest, PortStatus>("Status");
        RuleFor(x => x.PortCode)
            .IsUnique(async portCode => await portRepository.GetByPortCodeAsync(portCode!) == null,
                _ => "ports.port_code_already_exists");
        RuleFor(x => x)
            .MustAsync(async (request, _) =>
            {
                Port? existing =
                    await portRepository.GetByNameArCountryAndTypeAsync(request.NameAr, request.Country, request.Type);
                return existing == null;
            })
            .WithMessage("ports.name_ar_country_type_already_exists");
    }
}
