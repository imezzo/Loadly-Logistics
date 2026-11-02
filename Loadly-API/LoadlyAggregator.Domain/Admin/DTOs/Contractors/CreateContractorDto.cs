using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Contractors;

public class CreateContractorDto
{
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? SecondPhoneNumber { get; set; }
    public UserStatus Status { get; set; } = UserStatus.ACTIVE;
    public CommunicationChannel MfaChannel { get; set; } = CommunicationChannel.None;
    public bool IsActive { get; set; } = false;
    public List<ContractorDocumentItemDto>? Documents { get; set; }
}
