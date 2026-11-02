using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Contractors;

public class UpdateContractorDto
{
    public long Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? SecondPhoneNumber { get; set; }
    public CommunicationChannel MfaChannel { get; set; } = CommunicationChannel.None;
    public UserStatus Status { get; set; }
    public bool IsActive { get; set; } = false;
    public List<ContractorDocumentItemDto>? Documents { get; set; }
}
