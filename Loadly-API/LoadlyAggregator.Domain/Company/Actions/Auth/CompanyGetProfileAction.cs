using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;
namespace LoadlyAggregator.Domain.Company.Actions.Auth;



public class CompanyGetProfileAction : IAction<long, object>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IPermissionService _permissionService;
    private readonly IMediaService _mediaService;
    public CompanyGetProfileAction(
        ICompanyRepository companyRepository,
        IPermissionService permissionService,
        IMediaService mediaService)
    {
        _companyRepository = companyRepository;
        _permissionService = permissionService;
        _mediaService = mediaService;
    }
    public async Task<object> ExecuteAsync(long userId)
    {
        var company = await _companyRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("companies.not_found");
        return new
        {
            Id = company.Id,
            FullName = company.FullName,
            UserName = company.UserName,
            Email = company.Email,
            Phone = company.Phone,
            UserType = "company",
            MfaChannel = (int)company.MfaChannel,
            EmailVerified = company.EmailVerified,
            PhoneVerified = company.PhoneVerified,
            Status = (int)company.Status,
            ProfilePicture = !string.IsNullOrWhiteSpace(company.ProfilePicture)
                ? _mediaService.Read($"company/{company.Id}/profile-picture/{company.ProfilePicture}")
                : null,
            BiometricFingerprint = company.BiometricFingerprint,
            Permissions = await _permissionService.GetPermissionsGroupedByGroupAsync(company),
            CreatedAt = company.CreatedAt,
            UpdatedAt = company.UpdatedAt
        };
    }
}
