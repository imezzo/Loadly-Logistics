using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Company.DTOs.Auth;
namespace LoadlyAggregator.Domain.Company.Actions.Auth;



public class CompanyDisableMfaAction : IAction<CompanyDisableMfaDto, bool>
{
    private readonly ICompanyRepository _companyRepository;
    public CompanyDisableMfaAction(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }
    public async Task<bool> ExecuteAsync(CompanyDisableMfaDto dto)
    {
        var company = await _companyRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(company, "Company");

        company!.MfaChannel = CommunicationChannel.None;
        await _companyRepository.UpdateAsync(company);
        await _companyRepository.SaveChangesAsync();
        return true;
    }
}
