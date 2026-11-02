using LoadlyAggregator.Shared.Enums;
using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Admins;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Admins;

public class CreateAdminRequest
{
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserStatus Status { get; set; } = UserStatus.ACTIVE;
    public CommunicationChannel MfaChannel { get; set; } = CommunicationChannel.None;
    public List<long>? RoleIds { get; set; }

    public CreateAdminDto FromRequest() => new()
    {
        FullName = FullName,
        UserName = UserName,
        Email = Email,
        Phone = Phone,
        Status = (int)Status,
        MfaChannel = (int)MfaChannel,
        RoleIds = RoleIds
    };
}

public class CreateAdminRequestValidator : AbstractValidator<CreateAdminRequest>
{
    public CreateAdminRequestValidator(IAdminRepository adminRepository)
    {
        RuleFor(x => x.FullName).IsRequiredFullName();
        RuleFor(x => x.UserName)
            .IsRequiredUserName()
            .IsUnique(async userName => await adminRepository.GetByUserNameAsync(userName) == null, _ => "auth.username_already_taken");
        RuleFor(x => x.Email).IsRequiredEmail();
        RuleFor(x => x.Phone)
            .IsRequiredEgyptianPhone()
            .IsUnique(async phone => await adminRepository.GetByPhoneAsync(phone) == null, _ => "auth.phone_already_taken");
        RuleFor(x => x.MfaChannel).MustBeAdminMfaChannel();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.RoleIds)
            .Must(roleIds => roleIds == null || roleIds.Count == 0 || roleIds.All(id => id > 0))
            .WithMessage("validation.role_ids_must_be_positive")
            .When(x => x.RoleIds != null && x.RoleIds.Any());
    }
}
