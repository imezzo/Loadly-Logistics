using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Collector.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Collector.Auth;

public class CollectorUpdateProfileRequest
{
    public long UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;

    public CollectorUpdateProfileDto FromRequest() => new()
    {
        UserId = UserId,
        FullName = FullName,
        UserName = UserName,
        Email = Email,
        Phone = Phone
    };
}

public class CollectorUpdateProfileRequestValidator : AbstractValidator<CollectorUpdateProfileRequest>
{
    public CollectorUpdateProfileRequestValidator()
    {
        RuleFor(x => x.Email).IsOptionalEmail();
    }
}
