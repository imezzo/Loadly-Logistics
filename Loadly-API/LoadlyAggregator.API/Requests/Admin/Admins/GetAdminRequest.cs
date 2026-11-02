using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Admins;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Admins;

namespace LoadlyAggregator.API.Requests.Admin.Admins;

public class GetAdminRequest
{
    public long Id { get; set; }

    public GetAdminDto FromRequest() => new() { Id = Id };
}

public class GetAdminRequestValidator : AbstractValidator<GetAdminRequest>
{
    public GetAdminRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Admin ID");
    }
}
