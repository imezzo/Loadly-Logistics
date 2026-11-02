using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Admins;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Admins;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Admins;

public class UpdateAdminRequest
{
    [FromRoute]
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public CommunicationChannel MfaChannel { get; set; }
    public UserStatus Status { get; set; }
    public List<long> RoleIds { get; set; } = new();

    public UpdateAdminDto FromRequest(long id) => new()
    {
        Id = id,
        FullName = FullName,
        UserName = UserName,
        Email = Email,
        Phone = Phone,
        MfaChannel = (int)MfaChannel,
        Status = (int)Status,
        RoleIds = RoleIds
    };
}

public class UpdateAdminRequestValidator : AbstractValidator<UpdateAdminRequest>
{
    public UpdateAdminRequestValidator(IAdminRepository adminRepository)
    {
        RuleFor(x => x.FullName).IsRequiredFullName();
        RuleFor(x => x.UserName)
            .IsRequiredUserName()
            .IsUnique(async (userName, excludeId) => !await adminRepository.ExistsByUserNameAsync(userName, excludeId), x => x.Id, _ => "auth.username_already_taken");
        RuleFor(x => x.Email).IsRequiredEmail();
        RuleFor(x => x.Phone)
            .IsRequiredEgyptianPhone()
            .IsUnique(async (phone, excludeId) => !await adminRepository.ExistsByPhoneAsync(phone, excludeId), x => x.Id, _ => "auth.phone_already_taken");
        RuleFor(x => x.MfaChannel).MustBeAdminMfaChannel();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.RoleIds)
            .NotNull()
            .WithMessage("validation.role_ids_required");

        RuleForEach(x => x.RoleIds)
            .GreaterThan(0)
            .When(x => x.RoleIds != null && x.RoleIds.Any())
            .WithMessage("validation.must_be_greater_than_zero");
    }
}
