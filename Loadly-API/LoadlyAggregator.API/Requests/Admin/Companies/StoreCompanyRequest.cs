using LoadlyAggregator.Shared.Enums;
using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.API.Requests.Admin.Companies;

public class StoreCompanyRequest
{
    public const string Permission = "create_company";

    public string FullName { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public CommunicationChannel MfaChannel { get; set; } = CommunicationChannel.None;

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
    public long? AddedBy { get; set; }

    public List<AddressData> Addresses { get; set; } = new List<AddressData>();
    public List<ContactData> Contacts { get; set; } = new List<ContactData>();
    public List<CompanyDocumentData> CompanyDocuments { get; set; } = new List<CompanyDocumentData>();

    public StoreCompanyDto FromRequest() => new()
    {
        FullName = FullName,
        UserName = UserName,
        Email = Email,
        Phone = Phone,
        MfaChannel = (int)MfaChannel,
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
        AddedBy = AddedBy,
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
        }).ToList() ?? new List<AddressDataDto>(),
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
        }).ToList() ?? new List<ContactDataDto>(),
        CompanyDocuments = CompanyDocuments?.Select(d => new CompanyDocumentDataDto
        {
            Id = d.Id,
            DocumentId = d.DocumentId,
            File = d.File,
            ExpiryDate = d.ExpiryDate,
            Status = d.Status,
            AddedBy = d.AddedBy
        }).ToList() ?? new List<CompanyDocumentDataDto>()
    };
}

public class AddressData
{
    public long? Id { get; set; }
    public string StreetName { get; set; } = string.Empty;
    public string? BuildingNumber { get; set; }
    public string? BuildingType { get; set; }
    public long GovernerateId { get; set; }
    public long CityId { get; set; }
    public long? DistrictId { get; set; }
    public string PostalCode { get; set; } = string.Empty;
    public long? AddedBy { get; set; }
}

public class ContactData
{
    public long? Id { get; set; }
    public bool Primary { get; set; } = false;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public long? CommunicationMethodId { get; set; }
    public long? AddedBy { get; set; }
}

public class CompanyDocumentData
{
    public long? Id { get; set; }
    public long DocumentId { get; set; }
    public required FileUpload File { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DocumentVerificationStatus Status { get; set; } = DocumentVerificationStatus.PENDING;
    public long? AddedBy { get; set; }
}

public class StoreCompanyRequestValidator : AbstractValidator<StoreCompanyRequest>
{
    public StoreCompanyRequestValidator(ICompanyRepository companyRepository, ICompanyDetailRepository companyDetailRepository)
    {
        RuleFor(x => x.FullName).IsRequiredFullName();

        RuleFor(x => x.UserName)
            .IsRequiredString(50, 3, "Username")
            .Matches("^[a-zA-Z0-9_]+$").WithMessage("validation.username_pattern")
            .IsUnique(async userName => await companyRepository.GetByUserNameAsync(userName) == null, _ => "companies.username_already_exists");

        RuleFor(x => x.Email)
            .IsRequiredEmail()
            .IsUnique(async email => await companyRepository.GetByEmailAsync(email) == null, _ => "companies.email_already_exists");

        RuleFor(x => x.Phone)
           .IsRequiredEgyptianPhone()
           .IsUnique(async phone => await companyRepository.GetByPhoneAsync(phone) == null, _ => "companies.phone_already_exists");


        When(ShouldValidateDetailsKeys, () =>
        {

            RuleFor(x => x.NameEn)
                 .IsRequiredEnglishName()
                 .Matches(@"^[a-zA-Z0-9\s]+$").WithMessage("validation.company_name_en_format")
                 .IsUnique(async nameEn => await companyDetailRepository.GetByNameEnAsync(nameEn) == null, value => $"companies.company_english_name_exists|{value}");

            RuleFor(x => x.NameAr)
                .IsRequiredArabicName()
                .Matches(@"^[\u0600-\u06FF0-9\u0660-\u0669\s]+$").WithMessage("validation.company_name_ar_format_alt")
                .IsUnique(async nameAr => await companyDetailRepository.GetByNameArAsync(nameAr) == null, value => $"companies.company_arabic_name_exists|{value}");

            RuleFor(x => x.RegistrationNumber)
                .MinimumLength(8).WithMessage("validation.registration_number_min_when_provided")
                .MaximumLength(100).WithMessage("validation.must_not_exceed_characters|100")
                .When(x => !string.IsNullOrWhiteSpace(x.RegistrationNumber));
            RuleFor(x => x.RegistrationNumber)
                .MustAsync(async (_, regNo, cancellationToken) => string.IsNullOrWhiteSpace(regNo) || await companyDetailRepository.GetByRegistrationNumberAsync(regNo!) == null)
                .WithMessage((_, regNo) => $"companies.registration_number_exists|{regNo}")
                .When(x => !string.IsNullOrWhiteSpace(x.RegistrationNumber));

            RuleFor(x => x.WebsiteUrl)
                .Matches(@"^(https?:\/\/)?(www\.)?([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}(\/.*)?$")
                .WithMessage("validation.website_url_invalid")
                .IsUnique(async url => string.IsNullOrWhiteSpace(url) || await companyDetailRepository.GetByWebsiteUrlAsync(url) == null, value => $"companies.website_url_in_use|{value}")
                .When(x => !string.IsNullOrWhiteSpace(x.WebsiteUrl));



            RuleFor(x => x.VerificationStatus)
                .NotNull().WithMessage("validation.verification_status_required")
                .IsValidEnum<StoreCompanyRequest, Domain.Common.Enums.VerificationStatus>("VerificationStatus");





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


        RuleFor(x => x.Addresses)
           .IsRequiredNonEmptyCollection("Addresses");


        RuleForEach(x => x.Addresses)
            .SetValidator(new AddressDataValidator())
            .When(x => x.Addresses != null && x.Addresses.Any());



        RuleForEach(x => x.Contacts)
            .SetValidator(new ContactDataValidator())
            .When(x => x.Contacts != null && x.Contacts.Any());




        RuleForEach(x => x.CompanyDocuments)
            .SetValidator(new CompanyDocumentDataValidator())
            .When(x => x.VerificationStatus == VerificationStatus.VERIFIED &&
                       x.CompanyDocuments != null &&
                       x.CompanyDocuments.Any());
    }

    private bool ShouldValidateDetailsKeys(StoreCompanyRequest request)
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

public class AddressDataValidator : AbstractValidator<AddressData>
{
    public AddressDataValidator()
    {
        RuleFor(x => x.StreetName)
            .IsRequiredString(255, "Street name");

        RuleFor(x => x.BuildingNumber)
            .IsOptionalStringWithMaxLength(50, "Building number")
            .When(x => !string.IsNullOrWhiteSpace(x.BuildingNumber));

        RuleFor(x => x.BuildingType)
            .IsOptionalStringWithMaxLength(50, "Building type")
            .When(x => !string.IsNullOrWhiteSpace(x.BuildingType));

        RuleFor(x => x.GovernerateId)
            .NotNull().WithMessage("validation.governerate_required")
            .IsGreaterThanZero("Governerate ID");

        RuleFor(x => x.CityId)
            .NotNull().WithMessage("validation.city_required")
            .IsGreaterThanZero("City ID");

        RuleFor(x => x.PostalCode)
          .IsOptionalString(50, "Postal code")
          .Matches(@"^\d{5}$")
          .When(x => !string.IsNullOrEmpty(x.PostalCode))
          .WithMessage("validation.egyptian_postal_code");
    }
}

public class ContactDataValidator : AbstractValidator<ContactData>
{
    public ContactDataValidator()
    {
        RuleFor(x => x.Name)
            .IsRequiredString(200, 2, "Contact name");

        RuleFor(x => x.Email)
            .IsOptionalEmail()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .IsOptionalPhone(maxLength: 20)
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Position)
            .IsOptionalStringWithMaxLength(100, "Position")
            .When(x => !string.IsNullOrWhiteSpace(x.Position));

        RuleFor(x => x.CommunicationMethodId)
            .IsOptionalId("Communication method ID")
            .When(x => x.CommunicationMethodId.HasValue);
    }
}

public class CompanyDocumentDataValidator : AbstractValidator<CompanyDocumentData>
{
    public CompanyDocumentDataValidator()
    {
        RuleFor(x => x.DocumentId).IsRequiredId("Document ID");

        RuleFor(x => x.File)
            .NotNull().WithMessage("validation.company_document_file_required")
            .DependentRules(() =>
            {
                RuleFor(x => x.File.Length)
                    .IsGreaterThanZero("File");
            });

        RuleFor(x => x.Status)
            .IsValidEnum<CompanyDocumentData, Domain.Common.Enums.DocumentVerificationStatus>("Status");

        RuleFor(x => x.ExpiryDate)
          .IsRequiredFutureDate("Expiry date");
    }
}
