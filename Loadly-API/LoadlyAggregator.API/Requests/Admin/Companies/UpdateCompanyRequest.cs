using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.API.Requests.Admin.Companies;

public class UpdateCompanyRequest
{
    [FromRoute]
    public long Id { get; set; }
    public const string Permission = "edit_company";

    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string? NameAr { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? TaxId { get; set; }
    public string? WebsiteUrl { get; set; }
    public long? IndustryTypeId { get; set; }
    public string? CompanySize { get; set; }
    public string? ServiceRequirementsIds { get; set; }
    public long? BusinessHourId { get; set; }
    public long? CommunicationsMethodId { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.PENDING;
    public string? QrCode { get; set; }
    public List<AddressData>? Addresses { get; set; }
    public List<ContactData>? Contacts { get; set; }
    public List<CompanyDocumentData>? CompanyDocuments { get; set; }

    public UpdateCompanyDto FromRequest() => new()
    {
        Id = Id,
        FullName = FullName,
        UserName = UserName,
        Email = Email,
        Phone = Phone,
        NameEn = NameEn,
        NameAr = NameAr,
        RegistrationNumber = RegistrationNumber,
        TaxId = TaxId,
        WebsiteUrl = WebsiteUrl,
        IndustryTypeId = IndustryTypeId,
        CompanySize = CompanySize,
        ServiceRequirementsIds = ServiceRequirementsIds,
        BusinessHourId = BusinessHourId,
        CommunicationsMethodId = CommunicationsMethodId,
        VerificationStatus = VerificationStatus,
        QrCode = QrCode,
        Addresses = Addresses?.Select(a => new AddressDataDto
        {
            Id = a.Id,
            StreetName = a.StreetName,
            BuildingNumber = a.BuildingNumber,
            BuildingType = a.BuildingType,
            GovernerateId = a.GovernerateId,
            CityId = a.CityId,
            DistrictId = a.DistrictId,
            PostalCode = a.PostalCode,
            AddedBy = a.AddedBy
        }).ToList(),
        Contacts = Contacts?.Select(c => new ContactDataDto
        {
            Id = c.Id,
            Primary = c.Primary,
            Name = c.Name,
            Email = c.Email,
            Phone = c.Phone,
            Position = c.Position,
            CommunicationMethodId = c.CommunicationMethodId,
            AddedBy = c.AddedBy
        }).ToList(),
        CompanyDocuments = CompanyDocuments?.Select(d => new CompanyDocumentDataDto
        {
            Id = d.Id,
            DocumentId = d.DocumentId,
            File = d.File,
            ExpiryDate = d.ExpiryDate,
            Status = d.Status,
            AddedBy = d.AddedBy
        }).ToList()
    };
}

public class UpdateCompanyRequestValidator : AbstractValidator<UpdateCompanyRequest>
{
    public UpdateCompanyRequestValidator(ICompanyRepository companyRepository, ICompanyDetailRepository companyDetailRepository)
    {
        RuleFor(x => x.UserName)
            .IsRequiredString(50, 3, "Username")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("validation.username_pattern")
            .MustAsync(async (request, userName, cancellationToken) =>
            {
                var existing = await companyRepository.GetByUserNameAsync(userName);
                return existing == null || existing.Id == request.Id;
            })
            .WithMessage("companies.username_already_exists");
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .Matches(@"^[a-zA-Z0-9\s]+$").WithMessage("validation.company_name_en_format");

        RuleFor(x => x.NameAr)
            .IsRequiredArabicName()
            .Matches(@"^[\u0600-\u06FF0-9\u0660-\u0669\s]+$").WithMessage("validation.company_name_ar_format_alt");

        RuleFor(x => x.Email)
            .IsOptionalEmail()
            .When(x => !string.IsNullOrWhiteSpace(x.Email))
            .MustAsync(async (request, email, cancellationToken) =>
            {
                if (string.IsNullOrWhiteSpace(email))
                    return true;
                var existing = await companyRepository.GetByEmailAsync(email);
                return existing == null || existing.Id == request.Id;
            })
            .WithMessage("companies.email_already_exists")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
           .IsRequiredEgyptianPhone()
           .MustAsync(async (request, phone, cancellationToken) =>
           {
               var existing = await companyRepository.GetByPhoneAsync(phone!);
               return existing == null || existing.Id == request.Id;
           })
           .WithMessage("companies.phone_already_exists");






        When(ShouldValidateDetailsKeys, () =>
        {

            RuleFor(x => x.NameEn)
             .IsRequiredEnglishName(minLength: 10)
             .Matches(@"^[a-zA-Z0-9\s]+$").WithMessage("validation.company_name_en_format")
             .IsUnique(async (nameEn, excludeId) => !await companyDetailRepository.ExistsByNameEnAsync(nameEn, excludeId), x => x.Id, value => $"validation.company_detail_name_en_in_use|{value}");

            RuleFor(x => x.NameAr)
                .IsRequiredArabicName(minLength: 10)
                .Matches(@"^[\u0600-\u06FF\s]+$").WithMessage("validation.company_name_ar_chars_only")
                .IsUnique(async (nameAr, excludeId) => !await companyDetailRepository.ExistsByNameArAsync(nameAr, excludeId), x => x.Id, value => $"validation.company_detail_name_ar_in_use|{value}");

            RuleFor(x => x.RegistrationNumber)
                .MinimumLength(8).WithMessage("validation.registration_number_min_when_provided")
                .MaximumLength(100).WithMessage("validation.must_not_exceed_characters|100")
                .When(x => !string.IsNullOrWhiteSpace(x.RegistrationNumber));
            RuleFor(x => x.RegistrationNumber)
                .MustAsync(async (request, regNo, cancellationToken) => string.IsNullOrWhiteSpace(regNo) || !await companyDetailRepository.ExistsByRegistrationNumberAsync(regNo!, request.Id))
                .WithMessage((_, regNo) => $"validation.company_detail_registration_exists|{regNo}")
                .When(x => !string.IsNullOrWhiteSpace(x.RegistrationNumber));

            RuleFor(x => x.WebsiteUrl)
                .Matches(@"^(https?:\/\/)?(www\.)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(\/.*)?$")
                .WithMessage("validation.website_url_invalid")
                .IsUnique(async (url, excludeId) => string.IsNullOrWhiteSpace(url) || !await companyDetailRepository.ExistsByWebsiteUrlAsync(url, excludeId), x => x.Id, value => $"validation.company_detail_website_in_use|{value}")
                .When(x => !string.IsNullOrWhiteSpace(x.WebsiteUrl));

            RuleFor(x => x.VerificationStatus)
                .NotNull().WithMessage("validation.verification_status_required")
                .IsValidEnum<UpdateCompanyRequest, Domain.Common.Enums.VerificationStatus>("VerificationStatus");

            RuleFor(x => x.CompanySize)
                .IsOptionalStringWithMaxLength(50, "Company size")
                .When(x => !string.IsNullOrWhiteSpace(x.CompanySize));

            RuleFor(x => x.ServiceRequirementsIds)
                .IsOptionalStringWithMaxLength(500, "Service requirements IDs")
                .When(x => !string.IsNullOrWhiteSpace(x.ServiceRequirementsIds));

            RuleFor(x => x.QrCode)
                .IsOptionalStringWithMaxLength(500, "QR code")
                .When(x => !string.IsNullOrWhiteSpace(x.QrCode));
        });


        RuleForEach(x => x.Addresses)
            .SetValidator(new AddressDataValidator())
            .When(x => x.Addresses != null && x.Addresses.Any());




        RuleForEach(x => x.Contacts)
            .SetValidator(new ContactDataValidator())
            .When(x => x.Contacts != null && x.Contacts.Any());



    }





    private bool ShouldValidateDetailsKeys(UpdateCompanyRequest request)
    {

        return !string.IsNullOrWhiteSpace(request.NameEn) ||
                    !string.IsNullOrWhiteSpace(request.NameAr) ||
                    !string.IsNullOrWhiteSpace(request.RegistrationNumber) ||
                    !string.IsNullOrWhiteSpace(request.TaxId) ||
                    !string.IsNullOrWhiteSpace(request.WebsiteUrl) ||
                    !string.IsNullOrWhiteSpace(request.CompanySize) ||
                    request.IndustryTypeId.HasValue ||
                    request.BusinessHourId.HasValue ||
                    request.CommunicationsMethodId.HasValue ||
                    !string.IsNullOrWhiteSpace(request.ServiceRequirementsIds) ||
                    !string.IsNullOrWhiteSpace(request.QrCode);

    }
}
