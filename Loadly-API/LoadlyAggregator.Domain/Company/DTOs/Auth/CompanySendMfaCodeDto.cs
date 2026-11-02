using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
namespace LoadlyAggregator.Domain.Company.DTOs.Auth;



public class CompanySendMfaCodeDto
{
    public long UserId { get; set; }
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;
}
