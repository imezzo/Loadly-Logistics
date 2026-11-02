using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;
namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;



public class ContractorDisableMfaAction : IAction<ContractorDisableMfaDto, bool>
{
    private readonly IContractorRepository _contractorRepository;
    public ContractorDisableMfaAction(IContractorRepository contractorRepository)
    {
        _contractorRepository = contractorRepository;
    }
    public async Task<bool> ExecuteAsync(ContractorDisableMfaDto dto)
    {
        var contractor = await _contractorRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(contractor, "Contractor");

        contractor!.MfaChannel = CommunicationChannel.None;
        await _contractorRepository.UpdateAsync(contractor);
        return true;
    }
}
