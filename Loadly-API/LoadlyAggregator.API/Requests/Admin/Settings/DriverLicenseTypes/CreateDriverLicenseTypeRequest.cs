using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.DriverLicenseTypes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Settings.DriverLicenseTypes;

public class CreateDriverLicenseTypeRequest
{
    public const string Permission = "create_driver_license_type";

    public string NameEn { get; set; } = string.Empty;
    public string NameAr { get; set; } = string.Empty;
    public UserStatus Status { get; set; } = UserStatus.ACTIVE;

    public CreateDriverLicenseTypeDto FromRequest() => new()
    {
        NameEn = NameEn,
        NameAr = NameAr,
        Status = Status
    };
}

public class CreateDriverLicenseTypeRequestValidator : AbstractValidator<CreateDriverLicenseTypeRequest>
{
    public CreateDriverLicenseTypeRequestValidator(IDriverLicenseTypeRepository driverLicenseTypeRepository)
    {
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .IsUnique(async nameEn => await driverLicenseTypeRepository.GetByNameAsync(nameEn) == null, _ => "driver_license_types.name_already_exists");
        RuleFor(x => x.NameAr).IsRequiredArabicName();
        RuleFor(x => x.Status)
            .IsValidEnum<CreateDriverLicenseTypeRequest, Domain.Common.Enums.UserStatus>("Status");
    }
}
