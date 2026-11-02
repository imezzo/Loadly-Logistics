using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Users;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Company.Actions.Users;

public class ListCompanyUsersAction
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;

    public ListCompanyUsersAction(
        ICompanyRepository companyRepository,
        IMapper mapper)
    {
        _companyRepository = companyRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<CompanyUserDto>> ExecuteAsync(CompanyUserManagementDto dto)
    {
        var (items, total) = await _companyRepository.ListUsersOfCompanyAsync(
            dto.CompanyId, dto.Search, dto.Status, dto.Page, dto.PageSize);

        var mapped = items.Select(user =>
        {
            var userDto = _mapper.Map<CompanyUserDto>(user);
            userDto.Roles = _mapper.Map<List<RoleDto>>(user.UserRoles.Select(mhr => mhr.Role).OrderBy(r => r.Name).ToList());
            return userDto;
        }).ToList();

        return new ListingDto<CompanyUserDto>
        {
            Data = mapped,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, total)
        };
    }
}
