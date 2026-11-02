using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Users;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Users;

public class UpdateCompanyUserAction
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPermissionService _permissionService;

    public UpdateCompanyUserAction(
        ICompanyRepository companyRepository,
        IUnitOfWork unitOfWork,
        IPermissionService permissionService)
    {
        _companyRepository = companyRepository;
        _unitOfWork = unitOfWork;
        _permissionService = permissionService;
    }

    public async Task ExecuteAsync(CompanyUserManagementDto dto)
    {
        if (!dto.Id.HasValue)
        {
            throw new ValidationException("company_users.id_required_for_update");
        }

        var user = await _companyRepository.GetByIdOrThrowAsync(dto.Id.Value);
        if (user.CompanyId != dto.CompanyId || user.Type != CompanyType.EMPLOYEE)
        {
            throw new ValidationException("company_users.not_a_valid_company_user");
        }

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            user.FullName = dto.FullName;
            user.UserName = dto.Email.ToLower();
            user.Email = dto.Email.ToLower();
            user.Phone = dto.Phone;
            user.MfaChannel = dto.MfaChannel;
            user.Status = (UserStatus)dto.Status!.Value;
            _companyRepository.Update(user);
            await _companyRepository.SaveChangesAsync();
            await _permissionService.UpdateRolesAsync(user, dto.RoleIds);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
