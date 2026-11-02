using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.DriverLicenseTypes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.DriverLicenseTypes;

public class GetDriverLicenseTypeRequest
{
    public const string Permission = "view_driver_license_type";
    public long Id { get; set; }
    public GetDriverLicenseTypeDto FromRequest() => new() { Id = Id };
}

public class GetDriverLicenseTypeRequestValidator : AbstractValidator<GetDriverLicenseTypeRequest>
{
    public GetDriverLicenseTypeRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("ID");
    }
}
