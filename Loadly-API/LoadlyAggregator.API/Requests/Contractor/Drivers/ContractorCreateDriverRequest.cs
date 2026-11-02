using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Drivers;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Contractor.DTOs.Drivers;

namespace LoadlyAggregator.API.Requests.Contractor.Drivers;

public class ContractorCreateDriverRequest
{
    public string FullNameAr { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public List<CreateDriverDocumentItem>? Documents { get; set; }

    public ContractorCreateDriverDto FromRequest(long contractorId) => new()
    {
        ContractorId = contractorId,
        FullNameAr = FullNameAr,
        Phone = Phone,
        Documents = Documents?.Select(d => new CreateDriverDocumentItemDto
        {
            DocumentId = d.DocumentId,
            File = d.File,
            ExpiryDate = d.ExpiryDate,
            Status = (int)d.Status
        }).ToList()
    };
}

public class ContractorCreateDriverRequestValidator : AbstractValidator<ContractorCreateDriverRequest>
{
    public ContractorCreateDriverRequestValidator(IDriverRepository driverRepository)
    {
        RuleFor(x => x.FullNameAr).IsRequiredArabicFullName();
        RuleFor(x => x.Phone)
            .IsRequiredEgyptianPhone()
            .IsUnique(async phone => await driverRepository.GetByPhoneAsync(phone) == null, _ => "auth.phone_already_taken");

        RuleForEach(x => x.Documents)
            .SetValidator(new CreateDriverDocumentItemValidator())
            .When(x => x.Documents != null && x.Documents.Any());
    }
}
