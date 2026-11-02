using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Companies;
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
    public async Task<ListingDto<CompanyUserDto>> ExecuteAsync(ListCompanyUsersDto dto)
    {
        var (items, total) = await _companyRepository.ListUsersOfCompanyAsync(
            dto.CompanyId, dto.Search, dto.Status, dto.Page, dto.PageSize);

        return new ListingDto<CompanyUserDto>
        {
            Data = _mapper.Map<List<CompanyUserDto>>(items),
            Pagination = new PaginationDto(dto.Page, dto.PageSize, total)
        };
    }
}
