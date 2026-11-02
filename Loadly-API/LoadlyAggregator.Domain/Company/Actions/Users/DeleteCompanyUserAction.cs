using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Users;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Users;

public class DeleteCompanyUserAction
{
    private readonly ICompanyRepository _companyRepository;

    public DeleteCompanyUserAction(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task ExecuteAsync(CompanyUserManagementDto dto)
    {
        if (!dto.Id.HasValue)
        {
            throw new ValidationException("company_users.id_required");
        }

        var user = await _companyRepository.GetByIdOrThrowAsync(dto.Id.Value);
        if (user.CompanyId != dto.CompanyId || user.Type != CompanyType.EMPLOYEE)
        {
            throw new NotFoundException("company_users.user_not_found");
        }

        _companyRepository.Remove(user);
        await _companyRepository.SaveChangesAsync();
    }
}
