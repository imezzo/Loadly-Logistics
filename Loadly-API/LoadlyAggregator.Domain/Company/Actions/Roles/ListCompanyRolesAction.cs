using AutoMapper;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Roles;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Company.Actions.Roles;

public class ListCompanyRolesAction
{
    private readonly IRoleRepository _roleRepository;
    private readonly IMapper _mapper;

    public ListCompanyRolesAction(IRoleRepository roleRepository, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _mapper = mapper;
    }

    public async Task<ListingDto<CompanyRoleDto>> ExecuteAsync(ListCompanyRolesDto dto)
    {
        var (roles, totalCount) = await _roleRepository.ListAsync(dto.Search, "company", dto.Page, dto.PageSize, dto.CompanyId);

        return new ListingDto<CompanyRoleDto>
        {
            Data = _mapper.Map<List<CompanyRoleDto>>(roles),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
