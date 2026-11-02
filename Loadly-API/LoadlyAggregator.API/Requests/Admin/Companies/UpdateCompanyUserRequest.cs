using LoadlyAggregator.Shared.Enums;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Companies;

public class UpdateCompanyUserRequest
{
    public const string Permission = "edit_company";
    [FromRoute]
    public long CompanyId { get; set; }
    [FromRoute]
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public CommunicationChannel MfaChannel { get; set; } = CommunicationChannel.None;


    public UpdateCompanyUserDto FromRequest() => new UpdateCompanyUserDto
    {
        Id = Id,
        CompanyId = CompanyId,
        FullName = FullName,
        UserName = UserName,
        Email = Email,
        Phone = Phone,
        MfaChannel = MfaChannel
    };
}
public class UpdateCompanyUserRequestValidator : AbstractValidator<UpdateCompanyUserRequest>
{
    public UpdateCompanyUserRequestValidator(ICompanyRepository companyRepository)
    {
        RuleFor(x => x.FullName)
            .IsRequiredFullName();

        RuleFor(x => x.UserName)
            .IsRequiredString(50, 3, "Username")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("validation.username_pattern")
            .MustAsync(async (request, userName, ct) =>
            {
                var existing = await companyRepository.GetByUserNameAsync(userName);
                return existing == null || existing.Id == request.Id;
            })
            .WithMessage("companies.username_already_exists");

        RuleFor(x => x.Email)
            .IsRequiredEmail()
            .MustAsync(async (request, email, ct) =>
            {
                if (string.IsNullOrWhiteSpace(email)) return true;
                var existing = await companyRepository.GetByEmailAsync(email);
                return existing == null || existing.Id == request.Id;
            })
            .WithMessage("companies.email_already_exists");

        RuleFor(x => x.Phone)
            .IsRequiredEgyptianPhone()
            .MustAsync(async (request, phone, ct) =>
            {
                var existing = await companyRepository.GetByPhoneAsync(phone);
                return existing == null || existing.Id == request.Id;
            })
            .WithMessage("companies.phone_already_exists");
    }
}
