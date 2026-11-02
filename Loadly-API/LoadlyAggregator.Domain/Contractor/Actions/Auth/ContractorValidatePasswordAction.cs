using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Auth;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;

namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;

public class ContractorValidatePasswordAction : IVoidAction<ContractorValidatePasswordDto>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly AuthService _authService;

    public ContractorValidatePasswordAction(
        IContractorRepository contractorRepository,
        AuthService authService)
    {
        _contractorRepository = contractorRepository;
        _authService = authService;
    }

    public async Task ExecuteAsync(ContractorValidatePasswordDto dto)
    {
        _authService.ValidateCredentials(
            await _contractorRepository.GetByIdAsync(dto.UserId),
            dto.Password);
    }
}



