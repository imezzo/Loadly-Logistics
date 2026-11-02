using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Utilities;
namespace LoadlyAggregator.Domain.Contractor.DTOs.Auth;



public class ContractorForgetPasswordDto
{



    public string Identifier { get; set; } = string.Empty;
    public CommunicationChannel Channel { get; set; } = CommunicationChannel.Email;
}
