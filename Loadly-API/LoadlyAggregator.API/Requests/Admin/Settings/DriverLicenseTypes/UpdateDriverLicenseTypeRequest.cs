using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Settings.DriverLicenseTypes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.DriverLicenseTypes;

public class UpdateDriverLicenseTypeRequest
{
    public const string Permission = "edit_driver_license_type";

    [FromRoute]
    public long Id { get; set; }
    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public UserStatus Status { get; set; }

    public UpdateDriverLicenseTypeDto FromRequest() => new()
    {
        Id = Id,
        NameEn = NameEn,
        NameAr = NameAr,
        Status = Status
    };
}

public class UpdateDriverLicenseTypeRequestValidator : AbstractValidator<UpdateDriverLicenseTypeRequest>
{
    public UpdateDriverLicenseTypeRequestValidator(IDriverLicenseTypeRepository driverLicenseTypeRepository)
    {
        RuleFor(x => x.Id).IsRequiredId("Driver license type ID");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async (nameEn, excludeId) =>
            {
                var existing = await driverLicenseTypeRepository.GetByNameAsync(nameEn);
                return existing == null || existing.Id == excludeId;
            }, x => x.Id, _ => "driver_license_types.name_already_exists");
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.Status)
            .IsValidEnum<UpdateDriverLicenseTypeRequest, Domain.Common.Enums.UserStatus>("Status");
    }
}
