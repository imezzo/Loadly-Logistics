using System.Linq;
using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Admins;
using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Admins;

public class GetAdminAction
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;

    public GetAdminAction(IAdminRepository adminRepository, IMapper mapper)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
    }

    public async Task<AdminDto> ExecuteAsync(GetAdminDto dto)
    {
        var admin = await _adminRepository.GetByIdOrThrowAsync(dto.Id);
        var roles = admin.UserRoles.Select(mhr => mhr.Role).OrderBy(r => r.Name).ToList();
        var adminDto = _mapper.Map<AdminDto>(admin);
        adminDto.Roles = _mapper.Map<List<RoleDto>>(roles);
        return adminDto;
    }
}
