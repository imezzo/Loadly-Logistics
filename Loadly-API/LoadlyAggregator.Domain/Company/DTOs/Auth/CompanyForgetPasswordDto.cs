using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Utilities;
namespace LoadlyAggregator.Domain.Company.DTOs.Auth;



public class CompanyForgetPasswordDto
{



    public string Identifier { get; set; } = string.Empty;
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;
}
