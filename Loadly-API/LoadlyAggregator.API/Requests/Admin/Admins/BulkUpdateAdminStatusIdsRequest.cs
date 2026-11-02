using FluentValidation;
using LoadlyAggregator.Domain.Admin.DTOs.Admins;

namespace LoadlyAggregator.API.Requests.Admin.Admins;

public class BulkUpdateAdminStatusIdsRequest
{
    public List<long> AdminIds { get; set; } = new();

    public BulkUpdateAdminStatusDto FromRequest(int status) => new()
    {
        AdminIds = AdminIds,
        Status = status
    };
}

public class BulkUpdateAdminStatusIdsRequestValidator : AbstractValidator<BulkUpdateAdminStatusIdsRequest>
{
    public BulkUpdateAdminStatusIdsRequestValidator()
    {
        RuleFor(x => x.AdminIds)
            .NotEmpty()
            .WithMessage("validation.at_least_one_admin_id");
    }
}
