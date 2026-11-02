using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
namespace LoadlyAggregator.Domain.Contractor.DTOs.Auth;



public class ContractorResendVerificationDto
{
    public long UserId { get; set; }
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;
}
