using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Time;
using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Drivers;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Models;
using LoadlyAggregator.Infrastructure.Repositories.Implementations;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.API.Requests.Admin.Drivers;

public class CreateDriverRequest
{
    public const string Permission = "create_driver";

    public string FullNameEn { get; set; } = string.Empty;
    public string FullNameAr { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserStatus Status { get; set; } = UserStatus.ACTIVE;
    public CommunicationChannel MfaChannel { get; set; } = CommunicationChannel.None;
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public long? LicenseTypeId { get; set; }
    public DateTime? LicenseIssueDate { get; set; }
    public string? LicenseNo { get; set; }
    public DateTime? LicenseExpirationDate { get; set; }
    public string? VehicleTypeIds { get; set; }
    public int? Age { get; set; }
    public string? StreetName { get; set; }
    public string? BuildingNumber { get; set; }
    public string? BuildingType { get; set; }
    public long? GovernerateId { get; set; }
    public long? CityId { get; set; }
    public long? DistrictId { get; set; }
    public string? PostalCode { get; set; } = string.Empty;
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    public DriverWorkingStatus WorkingStatus { get; set; } = DriverWorkingStatus.Available;
    public LicenseCategory LicenseCategory { get; set; } = LicenseCategory.PrivateLicense;
    public List<SpokenLanguage>? SpokenLanguages { get; set; }
    public List<CreateDriverDocumentItem>? Documents { get; set; }
    public List<long>? RoleIds { get; set; }
    public long? ContractorId { get; set; }

    public CreateDriverDto FromRequest() => new()
    {
        FullNameEn = FullNameEn,
        FullNameAr = FullNameAr,
        NationalId = NationalId,
        UserName = UserName,
        Email = Email,
        Phone = Phone,
        Status = (int)Status,
        MfaChannel = (int)MfaChannel,
        DateOfBirth = DateOfBirth,
        Gender = Gender,
        LicenseTypeId = LicenseTypeId,
        LicenseIssueDate = LicenseIssueDate,
        LicenseNo = LicenseNo,
        LicenseExpirationDate = LicenseExpirationDate,
        VehicleTypeIds = VehicleTypeIds,
        Age = Age,
        StreetName = StreetName,
        BuildingNumber = BuildingNumber,
        BuildingType = BuildingType,
        GovernerateId = GovernerateId,
        CityId = CityId,
        DistrictId = DistrictId,
        PostalCode = PostalCode,
        EmergencyContactName = EmergencyContactName,
        EmergencyContactPhone = EmergencyContactPhone,
        WorkingStatus = WorkingStatus,
        LicenseCategory = LicenseCategory,
        SpokenLanguages = SpokenLanguages,
        Documents = Documents?.Select(d => new CreateDriverDocumentItemDto
        {
            DocumentId = d.DocumentId,
            File = d.File,
            ExpiryDate = d.ExpiryDate,
            Status = (int)d.Status
        }).ToList(),
        RoleIds = RoleIds,
        ContractorId = ContractorId
    };
}

public class CreateDriverDocumentItem
{
    public long DocumentId { get; set; }
    public FileUpload? File { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DocumentVerificationStatus Status { get; set; } = DocumentVerificationStatus.PENDING;
}

public class CreateDriverRequestValidator : AbstractValidator<CreateDriverRequest>
{
    public CreateDriverRequestValidator(IDriverRepository driverRepository, IDriverDetailRepository driverDetailRepository)
    {
        RuleFor(x => x.FullNameEn).IsRequiredEnglishFullName();
        RuleFor(x => x.FullNameAr).IsRequiredArabicFullName();

        RuleFor(x => x.NationalId)
         .IsRequiredNationalId()
         .IsUnique(async nationalId => await driverRepository.GetByNationalIdAsync(nationalId) == null, value => $"drivers.national_id_taken|{value}");

        RuleFor(x => x.UserName)
            .IsRequiredUserName()
            .IsUnique(async userName => await driverRepository.GetByUserNameAsync(userName) == null, _ => "auth.username_already_taken");
        RuleFor(x => x.Email)
            .IsRequiredEmail()
            .IsUnique(async email => await driverRepository.GetByEmailAsync(email) == null, _ => "auth.email_already_taken");
        RuleFor(x => x.Phone)
            .IsRequiredEgyptianPhone()
            .IsUnique(async phone => await driverRepository.GetByPhoneAsync(phone) == null, _ => "auth.phone_already_taken");

        RuleFor(x => x.Status).IsInEnum();

        RuleFor(x => x.Gender)
            .IsOptionalStringWithMaxLength(50, "Gender")
            .When(x => !string.IsNullOrWhiteSpace(x.Gender));

        RuleFor(x => x.LicenseNo)
            .IsOptionalStringWithMaxLength(100, "License number")
            .IsUnique(async licenseNo => await driverDetailRepository.GetByLicenseNoAsync(licenseNo) == null, value => $"drivers.license_number_taken|{value}")
            .When(x => !string.IsNullOrWhiteSpace(x.LicenseNo));

        RuleFor(x => x.VehicleTypeIds)
            .IsOptionalStringWithMaxLength(1000, "Vehicle type IDs")
            .When(x => !string.IsNullOrWhiteSpace(x.VehicleTypeIds));

        RuleFor(x => x.Age)
            .IsInclusiveRange(18, 100, "Age")
            .When(x => x.Age.HasValue);

        RuleFor(x => x.StreetName)
            .IsRequiredString(255, "Street name");

        RuleFor(x => x.GovernerateId)
            .NotNull().WithMessage("validation.governerate_required");

        RuleFor(x => x.CityId)
            .NotNull().WithMessage("validation.city_required");

        RuleFor(x => x.PostalCode)
            .IsOptionalStringWithMaxLength(50, "Postal code")
            .When(x => !string.IsNullOrWhiteSpace(x.PostalCode));

        RuleFor(x => x.WorkingStatus)
            .IsInEnum().WithMessage("validation.driver_working_status_invalid");

        RuleFor(x => x.LicenseCategory)
            .IsInEnum().WithMessage("validation.driver_license_category_invalid");

        RuleForEach(x => x.SpokenLanguages)
            .IsInEnum().WithMessage("validation.driver_spoken_language_invalid")
            .When(x => x.SpokenLanguages != null && x.SpokenLanguages.Any());

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("validation.driver_dob_required")
            .LessThanOrEqualTo(AppUtcTime.UtcNow(TimeProvider.System).AddYears(-21))
            .WithMessage("validation.driver_min_age_21");


        RuleFor(x => x.LicenseExpirationDate)
           .NotEmpty().WithMessage("validation.driver_license_expiry_required")
           .Must(expiry => expiry.HasValue && AppUtcTime.UtcDateAtMidnight(expiry.Value) >= AppUtcTime.UtcTodayAtMidnight(TimeProvider.System))
           .WithMessage("validation.driver_license_expiry_not_past");

        RuleForEach(x => x.Documents)
            .SetValidator(new CreateDriverDocumentItemValidator())
            .When(x => x.Documents != null && x.Documents.Any());
        RuleFor(x => x.ContractorId)
            .IsOptionalId("Contractor ID")
            .When(x => x.ContractorId.HasValue);
    }
}

public class CreateDriverDocumentItemValidator : AbstractValidator<CreateDriverDocumentItem>
{
    public CreateDriverDocumentItemValidator()
    {
        RuleFor(x => x.DocumentId).IsRequiredId("Document ID");

        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.ExpiryDate)
            .IsFutureDate();
    }
}
