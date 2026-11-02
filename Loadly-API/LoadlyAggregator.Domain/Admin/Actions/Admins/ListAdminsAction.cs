using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Admins;
using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Admins;

public class ListAdminsAction
{
    private readonly IAdminRepository _adminRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public ListAdminsAction(
        IAdminRepository adminRepository,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _adminRepository = adminRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    public async Task<ListingDto<AdminDto>> ExecuteAsync(ListAdminsDto dto)
    {
        var (admins, totalCount) = await _adminRepository.ListAsync(
            dto.Search,
            dto.Type,
            dto.Status,
            _currentUserService.GetCurrentUserId(),
            dto.Page,
            dto.PageSize,
            dto.RoleIds);

        var adminDtos = admins.Select(admin =>
        {
            var roles = admin.UserRoles.Select(mhr => mhr.Role).OrderBy(r => r.Name).ToList();
            var adminDto = _mapper.Map<AdminDto>(admin);
            adminDto.Roles = _mapper.Map<List<RoleDto>>(roles);
            return adminDto;
        }).ToList();

        return new ListingDto<AdminDto>
        {
            Data = adminDtos,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
