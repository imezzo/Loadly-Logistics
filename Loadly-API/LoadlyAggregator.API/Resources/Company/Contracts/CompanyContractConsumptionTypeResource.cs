using LoadlyAggregator.Domain.Common.DTOs.Contracts;

namespace LoadlyAggregator.API.Resources.Company.Contracts;

public class CompanyContractConsumptionTypeResource
{
    public int ConsumptionType { get; set; }

    public static CompanyContractConsumptionTypeResource FromDto(ContractConsumptionTypeDto dto) => new()
    {
        ConsumptionType = (int)dto.ConsumptionType
    };
}
