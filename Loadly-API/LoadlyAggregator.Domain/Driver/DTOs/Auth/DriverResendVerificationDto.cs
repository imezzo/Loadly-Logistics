using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
namespace LoadlyAggregator.Domain.Driver.DTOs.Auth;



public class DriverResendVerificationDto
{
    public long UserId { get; set; }
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;
}
