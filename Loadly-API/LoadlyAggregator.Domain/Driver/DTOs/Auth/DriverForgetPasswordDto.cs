using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Utilities;
namespace LoadlyAggregator.Domain.Driver.DTOs.Auth;



public class DriverForgetPasswordDto
{



    public string Identifier { get; set; } = string.Empty;
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;
}
