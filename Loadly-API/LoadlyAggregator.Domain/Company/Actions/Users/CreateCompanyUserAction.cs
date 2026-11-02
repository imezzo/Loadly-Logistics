using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Users;
using LoadlyAggregator.Domain.Company.Services;
using LoadlyAggregator.Shared.Enums;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Users;

public class CreateCompanyUserAction
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;
    private readonly IPasswordResetTokenService _passwordResetTokenService;
    private readonly IResetPasswordUrlService _resetPasswordUrlService;
    private readonly ICompanyUserNotificationService _companyUserNotificationService;

    public CreateCompanyUserAction(
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork,
        IPermissionService permissionService,
        IPasswordResetTokenService passwordResetTokenService,
        IResetPasswordUrlService resetPasswordUrlService,
        ICompanyUserNotificationService companyUserNotificationService)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
        _passwordResetTokenService = passwordResetTokenService;
        _resetPasswordUrlService = resetPasswordUrlService;
        _companyUserNotificationService = companyUserNotificationService;
    }

    public async Task ExecuteAsync(CompanyUserManagementDto dto)
    {
        var parent = await _companyRepository.GetByIdOrThrowAsync(dto.CompanyId);
        if (parent.Type != CompanyType.COMPANY)
        {
            throw new ValidationException("company_users.parent_must_be_company");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var user = new Common.Entities.Company
            {
                FullName = dto.FullName,
                UserName = dto.Email.ToLower(),
                Email = dto.Email.ToLower(),
                Phone = dto.Phone,
                Type = CompanyType.EMPLOYEE,
                CompanyId = dto.CompanyId,
                Status = UserStatus.ACTIVE,
                MfaChannel = dto.MfaChannel,
                Password = string.Empty,
                EmailVerified = false,
                PhoneVerified = false
            };

            await _companyRepository.AddAsync(user);
            await _companyRepository.SaveChangesAsync();
            await _permissionService.AssignRolesIfProvidedAsync(user, dto.RoleIds);
            await _unitOfWork.CommitTransactionAsync();

            string resetToken = _passwordResetTokenService.GenerateResetToken(
                user.Id,
                "company",
                channel: CommunicationChannel.Email,
                expirationHours: 24);
            string resetUrl = _resetPasswordUrlService.GenerateResetPasswordUrl("company", resetToken);

            try
            {
                await _companyUserNotificationService.SendWelcomeSetPasswordEmailAsync(user, resetUrl);
            }
            catch (Exception)
            {
            }
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
