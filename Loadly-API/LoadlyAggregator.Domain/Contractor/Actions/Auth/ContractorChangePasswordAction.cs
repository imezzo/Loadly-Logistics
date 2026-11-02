using LoadlyAggregator.Domain.Common.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Contractor.DTOs.Auth;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Contractor.Actions.Auth;



public class ContractorChangePasswordAction : IAction<ContractorChangePasswordDto, bool>
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IPasswordHashingService _passwordHashingService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IJwtTokenService _jwtTokenService;
    public ContractorChangePasswordAction(
        IContractorRepository contractorRepository,
        IPasswordHashingService passwordHashingService,
        IRefreshTokenService refreshTokenService,
        IJwtTokenService jwtTokenService)
    {
        _contractorRepository = contractorRepository;
        _passwordHashingService = passwordHashingService;
        _refreshTokenService = refreshTokenService;
        _jwtTokenService = jwtTokenService;
    }
    public async Task<bool> ExecuteAsync(ContractorChangePasswordDto dto)
    {
        var contractor = await _contractorRepository.GetByIdAsync(dto.UserId);
        UserValidator.ValidateUserExists(contractor, "Contractor");
        UserValidator.ValidatePassword(_passwordHashingService, dto.CurrentPassword, contractor!.Password);
        UpdatePassword(contractor, dto.NewPassword);
        await _contractorRepository.UpdateAsync(contractor);
        await Task.WhenAll(
            _refreshTokenService.RevokeModelTokensAsync(contractor.Id, "Contractor"),
            _jwtTokenService.RevokeAllUserTokensAsync(contractor.Id, "contractor", expirationMinutes: 120));
        return true;
    }
    private void UpdatePassword(Domain.Common.Entities.Contractor contractor, string newPassword)
    {
        contractor.Password = _passwordHashingService.HashPassword(newPassword);
    }
}
