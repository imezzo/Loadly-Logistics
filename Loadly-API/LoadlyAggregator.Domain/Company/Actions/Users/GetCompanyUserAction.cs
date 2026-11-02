using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Users;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Users;

public class GetCompanyUserAction
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    public GetCompanyUserAction(ICompanyRepository companyRepository, IMapper mapper)
    {
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    public async Task<CompanyUserDto> ExecuteAsync(CompanyUserManagementDto dto)
    {
        if (!dto.Id.HasValue)
        {
            throw new ValidationException("company_users.id_required");
        }

        var user = await _companyRepository.GetUserByIdOrThrowAsync(dto.Id.Value);
        if (user.CompanyId != dto.CompanyId || user.Type != CompanyType.EMPLOYEE)
        {
            throw new NotFoundException("company_users.user_not_found");
        }

        var userDto = _mapper.Map<CompanyUserDto>(user);
        userDto.Roles = _mapper.Map<List<RoleDto>>(user.UserRoles.Select(mhr => mhr.Role).OrderBy(r => r.Name).ToList());
        return userDto;
    }
}
