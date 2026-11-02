using LoadlyAggregator.Shared.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Auth;

public class AdminForgetPasswordDto
{
    public string Identifier { get; set; } = string.Empty;
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Sms;
}
