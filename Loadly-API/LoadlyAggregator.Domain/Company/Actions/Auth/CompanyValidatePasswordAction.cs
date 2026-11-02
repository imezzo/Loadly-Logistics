using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Auth;
using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.Domain.Company.Actions.Auth;

public class CompanyValidatePasswordAction : IVoidAction<CompanyValidatePasswordDto>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly AuthService _authService;

    public CompanyValidatePasswordAction(
        ICompanyRepository companyRepository,
        AuthService authService)
    {
        _companyRepository = companyRepository;
        _authService = authService;
    }

    public async Task ExecuteAsync(CompanyValidatePasswordDto dto)
    {
        _authService.ValidateCredentials(
            await _companyRepository.GetByIdAsync(dto.UserId),
            dto.Password);
    }
}



