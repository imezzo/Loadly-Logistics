using LoadlyAggregator.Domain.Admin.DTOs.Drivers;

namespace LoadlyAggregator.Domain.Contractor.DTOs.Drivers;

public class ContractorCreateDriverDto
{
    public long ContractorId { get; set; }
    public string FullNameAr { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public List<CreateDriverDocumentItemDto>? Documents { get; set; }
}
