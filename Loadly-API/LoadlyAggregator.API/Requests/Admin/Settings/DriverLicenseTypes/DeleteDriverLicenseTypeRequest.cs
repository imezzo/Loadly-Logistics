using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.DriverLicenseTypes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;

namespace LoadlyAggregator.API.Requests.Admin.Settings.DriverLicenseTypes;

public class DeleteDriverLicenseTypeRequest
{
    public const string Permission = "delete_driver_license_type";
    public long Id { get; set; }
    public DeleteDriverLicenseTypeDto FromRequest() => new() { Id = Id };
}

public class DeleteDriverLicenseTypeRequestValidator : AbstractValidator<DeleteDriverLicenseTypeRequest>
{
    public DeleteDriverLicenseTypeRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Entity ID");
    }
}
