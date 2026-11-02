using LoadlyAggregator.Domain.Common.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Contracts;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Contracts;

public class CompanyGetContractConsumptionTypeAction
{
    private readonly IContractRepository _contractRepository;

    public CompanyGetContractConsumptionTypeAction(IContractRepository contractRepository)
    {
        _contractRepository = contractRepository;
    }

    public async Task<ContractConsumptionTypeDto> ExecuteAsync(CompanyGetContractDto dto)
    {
        var row = await _contractRepository.GetContractConsumptionTypeAsync(dto.Id) ?? throw new NotFoundException("contracts.not_found_with_id", dto.Id);
        if (row.CompanyId != dto.CompanyId) throw new NotFoundException("contracts.not_found_with_id", dto.Id);
        return new ContractConsumptionTypeDto { ConsumptionType = row.ConsumptionType };
    }
}
