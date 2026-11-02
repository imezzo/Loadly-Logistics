using LoadlyAggregator.Shared.Enums;
using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Collectors;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Collectors;

public class CreateCollectorRequest
{
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public UserStatus Status { get; set; } = UserStatus.ACTIVE;
    public CommunicationChannel MfaChannel { get; set; } = CommunicationChannel.None;
    public long? RoleId { get; set; }

    public CreateCollectorDto FromRequest() => new()
    {
        FullName = FullName,
        UserName = UserName,
        Email = Email,
        Phone = Phone,
        Status = (int)Status,
        MfaChannel = (int)MfaChannel,
        RoleId = RoleId
    };
}

public class CreateCollectorRequestValidator : AbstractValidator<CreateCollectorRequest>
{
    public CreateCollectorRequestValidator(ICollectorRepository collectorRepository)
    {
        RuleFor(x => x.FullName).IsRequiredFullName();
        RuleFor(x => x.UserName)
            .IsRequiredUserName()
            .IsUnique(async userName => await collectorRepository.GetByUserNameAsync(userName) == null, _ => "auth.username_already_taken");
        RuleFor(x => x.Email).IsOptionalEmail();
        RuleFor(x => x.Phone)
            .IsRequiredEgyptianPhone()
            .IsUnique(async phone => await collectorRepository.GetByPhoneAsync(phone) == null, _ => "auth.phone_already_taken");
        RuleFor(x => x.MfaChannel).IsValidCollectorMfaChannel();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.RoleId)
            .GreaterThan(0)
            .When(x => x.RoleId.HasValue)
            .WithMessage("validation.must_be_greater_than_zero");
    }
}
