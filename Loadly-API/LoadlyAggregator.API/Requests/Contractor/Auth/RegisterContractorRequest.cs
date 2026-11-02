using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Contractors;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.API.Requests.Contractor.Auth;

public class RegisterContractorRequest
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? Address { get; set; }
    public List<ContractorDocumentItem>? Documents { get; set; }

    public RegisterContractorDto FromRequest() => new()
    {
        FullName = FullName,
        Phone = Phone,
        NationalId = NationalId,
        Address = string.IsNullOrWhiteSpace(Address) ? null : Address.Trim(),
        Documents = Documents?.Select(d => new ContractorDocumentItemDto
        {
            DocumentId = d.DocumentId,
            File = d.File,
            ExpiryDate = d.ExpiryDate,
            Status = DocumentVerificationStatus.PENDING
        }).ToList()
    };
}

public class RegisterContractorRequestValidator : AbstractValidator<RegisterContractorRequest>
{
    public RegisterContractorRequestValidator(IContractorRepository contractorRepository)
    {
        RuleFor(x => x.FullName).IsRequiredFullName();
        RuleFor(x => x.Phone)
            .IsRequiredEgyptianPhone()
            .IsUnique(async phone => await contractorRepository.GetByPhoneAsync(phone) == null, _ => "auth.phone_already_taken");
        RuleFor(x => x.NationalId)
            .Matches(@"^\d{14}$").WithMessage("validation.national_id_must_be_14_digits|National ID")
            .When(x => !string.IsNullOrWhiteSpace(x.NationalId));
        RuleFor(x => x.NationalId)
            .IsUnique(async nationalId => await contractorRepository.GetByNationalIdAsync(nationalId) == null, value => $"contractors.national_id_taken|{value}")
            .When(x => !string.IsNullOrWhiteSpace(x.NationalId));
        RuleFor(x => x.Address)
            .IsRequiredString(1000, "Address");
        RuleForEach(x => x.Documents)
            .SetValidator(new ContractorDocumentItemValidator())
            .When(x => x.Documents != null && x.Documents.Any());
    }
}
