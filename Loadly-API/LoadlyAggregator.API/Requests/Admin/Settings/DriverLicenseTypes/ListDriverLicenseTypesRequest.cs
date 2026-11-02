using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.DriverLicenseTypes;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Settings.DriverLicenseTypes;

public class ListDriverLicenseTypesRequest : BasePagedRequest
{
    public const string Permission = "view_driver_license_type";

    public UserStatus? Status { get; set; }

    public ListDriverLicenseTypesDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        Status = Status
    };
}

public class ListDriverLicenseTypesRequestValidator : AbstractValidator<ListDriverLicenseTypesRequest>
{
    public ListDriverLicenseTypesRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
        RuleFor(x => x.Status)
            .IsValidEnum<ListDriverLicenseTypesRequest, Domain.Common.Enums.UserStatus>("Status")
            .When(x => x.Status.HasValue);
    }
}
