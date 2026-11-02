using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Contractors;

public class UpdateContractorRequest
{
    public const string Permission = "edit_contractor";

    [FromRoute]
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? SecondPhoneNumber { get; set; }
    public CommunicationChannel MfaChannel { get; set; }
    public UserStatus Status { get; set; }
    public bool IsActive { get; set; } = false;
    public List<ContractorDocumentItem>? Documents { get; set; }

    public UpdateContractorDto FromRequest() => new()
    {
        Id = Id,
        FullName = FullName,
        Email = Email,
        Phone = Phone,
        NationalId = NationalId,
        SecondPhoneNumber = SecondPhoneNumber,
        MfaChannel = MfaChannel,
        Status = Status,
        IsActive = IsActive,
        Documents = Documents?.Select(d => new ContractorDocumentItemDto
        {
            Id = d.Id,
            DocumentId = d.DocumentId,
            File = d.File,
            ExpiryDate = d.ExpiryDate,
            Status = d.Status
        }).ToList()
    };
}

public class UpdateContractorRequestValidator : AbstractValidator<UpdateContractorRequest>
{
    public UpdateContractorRequestValidator(IContractorRepository contractorRepository)
    {
        RuleFor(x => x.FullName).IsRequiredFullName();
        RuleFor(x => x.Email)
            .IsOptionalEmail()
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Email)
            .IsUnique(async (email, excludeId) => !await contractorRepository.ExistsByEmailAsync(email, excludeId), x => x.Id, _ => "auth.email_already_taken")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));
        RuleFor(x => x.Phone)
            .IsRequiredEgyptianPhone()
            .IsUnique(async (phone, excludeId) => !await contractorRepository.ExistsByPhoneAsync(phone, excludeId), x => x.Id, _ => "auth.phone_already_taken");
        RuleFor(x => x.NationalId)
            .Matches(@"^\d{14}$").WithMessage("validation.national_id_must_be_14_digits|National ID")
            .When(x => !string.IsNullOrWhiteSpace(x.NationalId));
        RuleFor(x => x.NationalId)
            .IsUnique(async (nationalId, excludeId) => !await contractorRepository.ExistsByNationalIdAsync(nationalId, excludeId), x => x.Id, value => $"contractors.national_id_taken|{value}")
            .When(x => !string.IsNullOrWhiteSpace(x.NationalId));
        RuleFor(x => x.SecondPhoneNumber)
            .IsOptionalEgyptianPhone()
            .IsUnique(async (secondPhone, excludeId) => !await contractorRepository.ExistsBySecondPhoneNumberAsync(secondPhone, excludeId), x => x.Id, _ => "auth.phone_already_taken")
            .When(x => !string.IsNullOrWhiteSpace(x.SecondPhoneNumber));
        RuleFor(x => x.MfaChannel).IsInEnum();
        RuleFor(x => x.Status).IsInEnum();
        RuleForEach(x => x.Documents)
            .SetValidator(new ContractorDocumentItemValidator())
            .When(x => x.Documents != null && x.Documents.Any());
    }
}
