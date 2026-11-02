using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Companies;
public class UpdateCompanyUserAction
{
    private readonly ICompanyRepository _companyRepository;

    public UpdateCompanyUserAction(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }

    public async Task ExecuteAsync(UpdateCompanyUserDto dto)
    {
        var user = await _companyRepository.GetByIdOrThrowAsync(dto.Id);

        if (user.CompanyId != dto.CompanyId || user.Type != CompanyType.EMPLOYEE)
            throw new ValidationException("company_users.not_a_valid_company_employee");

        user.FullName = dto.FullName;
        user.UserName = dto.UserName.ToLower();
        user.Email = dto.Email.ToLower();
        user.Phone = dto.Phone;
        user.MfaChannel = dto.MfaChannel;
        _companyRepository.Update(user);
        await _companyRepository.SaveChangesAsync();
    }
}
