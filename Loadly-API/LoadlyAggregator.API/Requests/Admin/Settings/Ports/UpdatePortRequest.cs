using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Ports;

public class UpdatePortRequest
{
    public const string Permission = "edit_port";

    [FromRoute]
    public long Id { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string? NameEn { get; set; }
    public string Country { get; set; } = string.Empty;
    public string? PortCode { get; set; }
    public PortType Type { get; set; }
    public PortStatus Status { get; set; }

    public UpdatePortDto FromRequest() => new()
    {
        Id = Id,
        NameAr = NameAr,
        NameEn = NameEn,
        Country = Country,
        PortCode = PortCode,
        Type = Type,
        Status = Status
    };
}

public class UpdatePortRequestValidator : AbstractValidator<UpdatePortRequest>
{
    public UpdatePortRequestValidator(IPortRepository portRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Port ID");
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.NameEn).IsOptionalString(255, "NameEn");
        RuleFor(x => x.Country).IsRequiredString(255, "Country");
        RuleFor(x => x.PortCode).IsOptionalString(50, "PortCode");
        RuleFor(x => x.Type)
            .IsValidEnum<UpdatePortRequest, PortType>("Type");
        RuleFor(x => x.PortCode)
            .IsUnique(async (portCode, excludeId) =>
            {
                Port? existing = await portRepository.GetByPortCodeAsync(portCode!);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "ports.port_code_already_exists");
        RuleFor(x => x)
            .MustAsync(async (request, _) =>
            {
                Port? existing =
                    await portRepository.GetByNameArCountryAndTypeAsync(request.NameAr, request.Country, request.Type);
                return existing == null || existing.Id == request.Id;
            })
            .WithMessage("ports.name_ar_country_type_already_exists");
        RuleFor(x => x.Status)
            .IsValidEnum<UpdatePortRequest, PortStatus>("Status");
    }
}
