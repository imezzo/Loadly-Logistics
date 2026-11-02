using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.Domain.Company.Actions.Auth;




public class CompanyEnableMfaAction : IAction<CompanyEnableMfaDto, bool>
{
    private readonly ICompanyRepository _companyRepository;

    public CompanyEnableMfaAction(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task<bool> ExecuteAsync(CompanyEnableMfaDto dto)
    {
        UserValidator.ValidateMfaChannel(dto.Channel);

        var company = await _companyRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(company, "Company");


        UserValidator.ValidateChannelVerified(dto.Channel, company!.EmailVerified, company.PhoneVerified);

        EnableMfa(company!, dto.Channel);
        await _companyRepository.UpdateAsync(company!);
        await _companyRepository.SaveChangesAsync();

        return true;
    }

    private static void EnableMfa(Domain.Common.Entities.Company company, CommunicationChannel channel)
    {

        company.MfaChannel = channel;
    }
}

