using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Collectors;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Collectors;

public class UpdateCollectorRequest
{
    [FromRoute]
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public CommunicationChannel MfaChannel { get; set; }
    public UserStatus Status { get; set; }
    public long? RoleId { get; set; }

    public UpdateCollectorDto FromRequest(long id) => new()
    {
        Id = id,
        FullName = FullName,
        UserName = UserName,
        Email = Email,
        Phone = Phone,
        MfaChannel = (int)MfaChannel,
        Status = (int)Status,
        RoleId = RoleId
    };
}

public class UpdateCollectorRequestValidator : AbstractValidator<UpdateCollectorRequest>
{
    public UpdateCollectorRequestValidator(ICollectorRepository collectorRepository)
    {
        RuleFor(x => x.FullName).IsRequiredFullName();
        RuleFor(x => x.UserName)
            .IsRequiredUserName()
            .IsUnique(async (userName, excludeId) => !await collectorRepository.ExistsByUserNameAsync(userName, excludeId), x => x.Id, _ => "auth.username_already_taken");
        RuleFor(x => x.Email).IsOptionalEmail();
        RuleFor(x => x.Phone)
            .IsRequiredEgyptianPhone()
            .IsUnique(async (phone, excludeId) => !await collectorRepository.ExistsByPhoneAsync(phone, excludeId), x => x.Id, _ => "auth.phone_already_taken");
        RuleFor(x => x.MfaChannel).IsValidCollectorMfaChannel();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.RoleId)
            .NotNull()
            .WithMessage("validation.role_id_required")
            .GreaterThan(0)
            .WithMessage("validation.must_be_greater_than_zero");
    }
}
