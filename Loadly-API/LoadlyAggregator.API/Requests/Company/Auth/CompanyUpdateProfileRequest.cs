using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Company.Auth;

public class CompanyUpdateProfileRequest
{
    public long UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public FileUpload? ProfilePicture { get; set; }
    public bool? BiometricFingerprint { get; set; }

    public CompanyUpdateProfileDto FromRequest() => new()
    {
        UserId = UserId,
        FullName = FullName,
        UserName = UserName,
        Email = Email,
        Phone = Phone,
        ProfilePicture = ProfilePicture,
        BiometricFingerprint = BiometricFingerprint
    };
}

public class CompanyUpdateProfileRequestValidator : AbstractValidator<CompanyUpdateProfileRequest>
{
    public CompanyUpdateProfileRequestValidator()
    {
        var allowedMimeTypes = new[] { "image/png", "image/jpeg", "image/jpg", "image/gif", "image/webp" };
        RuleFor(x => x.FullName).IsRequiredFullName();
        RuleFor(x => x.UserName).IsRequiredUserName();
        RuleFor(x => x.Email).IsRequiredEmail();
        RuleFor(x => x.Phone).IsRequiredEgyptianPhone();

        RuleFor(x => x.ProfilePicture)
            .Must(file => file == null || file.IsWithinSizeLimit(5 * 1024 * 1024))
            .WithMessage("validation.profile_image_max_5mb")
            .When(x => x.ProfilePicture != null && x.ProfilePicture.Length > 0);

        RuleFor(x => x.ProfilePicture)
            .IsValidFileType(allowedMimeTypes, "File type must be one of: png, jpg, jpeg, gif, webp")
            .When(x => x.ProfilePicture != null && x.ProfilePicture.Length > 0);
    }
}
