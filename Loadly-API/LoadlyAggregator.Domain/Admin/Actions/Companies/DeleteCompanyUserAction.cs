using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Companies;
public class DeleteCompanyUserAction
{
    private readonly ICompanyRepository _companyRepository;

    public DeleteCompanyUserAction(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
    }
    public async Task ExecuteAsync(DeleteCompanyUserDto dto)
    {
        var user = await _companyRepository.GetByIdOrThrowAsync(dto.Id);

        if (user.CompanyId != dto.CompanyId || user.Type != CompanyType.EMPLOYEE)
            throw new NotFoundException("company_users.user_not_found");

        _companyRepository.Remove(user);
        await _companyRepository.SaveChangesAsync();
    }
}
