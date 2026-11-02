using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Companies;
public class GetCompanyUserAction
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;
    public GetCompanyUserAction(ICompanyRepository companyRepository, IMapper mapper)
    {
        _companyRepository = companyRepository;
        _mapper = mapper;
    }
    public async Task<CompanyUserDto> ExecuteAsync(GetCompanyUserDto dto)
    {
        var user = await _companyRepository.GetUserByIdOrThrowAsync(dto.Id);

        if (user.CompanyId != dto.CompanyId || user.Type != CompanyType.EMPLOYEE)
            throw new NotFoundException("company_users.user_not_found");

        return _mapper.Map<CompanyUserDto>(user);
    }
}
