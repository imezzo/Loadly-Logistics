using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Mfa;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Utilities;
using LoadlyAggregator.Domain.Company.DTOs.Auth;

namespace LoadlyAggregator.Domain.Company.Actions.Auth;





public class CompanySendMfaCodeAction : IAction<CompanySendMfaCodeDto, object>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly MfaMethodStrategyFactory _strategyFactory;
    private readonly IMfaCacheService _mfaCacheService;

    public CompanySendMfaCodeAction(
        ICompanyRepository companyRepository,
        MfaMethodStrategyFactory strategyFactory,
        IMfaCacheService mfaCacheService)
    {
        _companyRepository = companyRepository;
        _strategyFactory = strategyFactory;
        _mfaCacheService = mfaCacheService;
    }

    public async Task<object> ExecuteAsync(CompanySendMfaCodeDto dto)
    {
        var company = await _companyRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(company, "Company");
        UserValidator.ValidateMfaChannel(dto.Channel);
        var code = CodeGenerator.GenerateNumericCode(6);
        await _mfaCacheService.StoreMfaCodeAsync(
            userId: company!.Id,
            userType: "company",
            code: code,
            channel: dto.Channel);
        var channelString = ChannelHelper.ToChannelString(dto.Channel);
        await _strategyFactory.GetStrategy(channelString)
            .SendCodeAsync(
                MfaRecipientResolver.GetRecipient(channelString, company.Email, company.Phone),
                company.FullName,
                code,
                "company",
                _mfaCacheService.MfaCodeExpirationMinutes,
                company.Id,
                "Company");
        return new { };
    }
}

