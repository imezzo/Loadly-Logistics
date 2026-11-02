using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Mfa;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Shared.Exceptions;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.Domain.Admin.Actions.Auth;

public class AdminSendMfaCodeAction : IAction<AdminSendMfaCodeDto, object>
{
    private readonly IAdminRepository _adminRepository;
    private readonly MfaMethodStrategyFactory _strategyFactory;
    private readonly IMfaCacheService _mfaCacheService;

    public AdminSendMfaCodeAction(
        IAdminRepository adminRepository,
        MfaMethodStrategyFactory strategyFactory,
        IMfaCacheService mfaCacheService)
    {
        _adminRepository = adminRepository;
        _strategyFactory = strategyFactory;
        _mfaCacheService = mfaCacheService;
    }

    public async Task<object> ExecuteAsync(AdminSendMfaCodeDto dto)
    {
        _ = dto.Channel == CommunicationChannel.Sms
            ? true
            : throw new ValidationException("validation.admin_channel_must_be_sms");

        var admin = await _adminRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(admin, "Admin");
        UserValidator.ValidateMfaEnabled(admin!.MfaChannel);
        UserValidator.ValidateChannelVerified(dto.Channel, admin.EmailVerified, admin.PhoneVerified);

        var channelString = ChannelHelper.ToChannelString(dto.Channel);
        var code = CodeGenerator.GenerateNumericCode(6);
        await _mfaCacheService.StoreMfaCodeAsync(
            userId: admin.Id,
            userType: "admin",
            code: code,
            channel: dto.Channel);
        await _strategyFactory.GetStrategy(channelString)
            .SendCodeAsync(
                MfaRecipientResolver.GetRecipient(channelString, admin.Email, admin.Phone),
                admin.FullName,
                code,
                "admin",
                _mfaCacheService.MfaCodeExpirationMinutes,
                admin.Id,
                "Admin");
        return new { };
    }
}
