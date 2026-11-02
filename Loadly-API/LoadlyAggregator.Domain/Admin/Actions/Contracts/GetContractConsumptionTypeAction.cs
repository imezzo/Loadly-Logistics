using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class GetContractConsumptionTypeAction
{
    private readonly IContractRepository _contractRepository;

    public GetContractConsumptionTypeAction(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task<ContractConsumptionTypeDto> ExecuteAsync(GetContractDto dto) =>
        new ContractConsumptionTypeDto
        {
            ConsumptionType = (await _contractRepository.GetContractConsumptionTypeAsync(dto.Id) ?? throw new NotFoundException("contracts.not_found_with_id", dto.Id)).ConsumptionType
        };
}
