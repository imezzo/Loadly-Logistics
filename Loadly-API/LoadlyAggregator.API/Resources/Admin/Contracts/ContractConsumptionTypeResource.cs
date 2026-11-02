using LoadlyAggregator.Domain.Common.DTOs.Contracts;

namespace LoadlyAggregator.API.Resources.Admin.Contracts;

public class ContractConsumptionTypeResource
{
    public int ConsumptionType { get; set; }

    public static ContractConsumptionTypeResource FromDto(ContractConsumptionTypeDto dto) => new()
    {
        ConsumptionType = (int)dto.ConsumptionType
    };
}
