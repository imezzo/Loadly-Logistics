using LoadlyAggregator.Domain.Common.DTOs.Contracts;

namespace LoadlyAggregator.API.Resources.Admin.Contracts;

public class ContractVehicleTypeIdResource
{
    public long Id { get; set; }

    public static ContractVehicleTypeIdResource FromDto(ContractVehicleTypeIdDto dto) => new()
    {
        Id = dto.Id
    };
}
