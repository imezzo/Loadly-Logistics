using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Admins;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Admins;

namespace LoadlyAggregator.API.Requests.Admin.Admins;

public class DeleteAdminRequest
{
    public long Id { get; set; }

    public DeleteAdminDto FromRequest() => new() { Id = Id };
}

public class DeleteAdminRequestValidator : AbstractValidator<DeleteAdminRequest>
{
    public DeleteAdminRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Admin ID");
    }
}
