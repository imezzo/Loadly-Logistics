using LoadlyAggregator.Domain.Admin.DTOs.Auth;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Auth;

namespace LoadlyAggregator.Domain.Admin.Actions.Auth;

public class AdminValidatePasswordAction : IVoidAction<AdminValidatePasswordDto>
{
    private readonly IAdminRepository _adminRepository;
    private readonly AuthService _authService;

    public AdminValidatePasswordAction(
        IAdminRepository adminRepository,
        AuthService authService)
    {
        _adminRepository = adminRepository;
        _authService = authService;
    }

    public async Task ExecuteAsync(AdminValidatePasswordDto dto)
    {
        _authService.ValidateCredentials(
            await _adminRepository.GetByIdAsync(dto.UserId),
            dto.Password);
    }
}



