using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Companies;

public class ListCompaniesAction
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMapper _mapper;
    private readonly IMediaService _mediaService;

    public ListCompaniesAction(
        ICompanyRepository companyRepository,
        IMapper mapper,
        IMediaService mediaService)
    {
        _companyRepository = companyRepository;
        _mapper = mapper;
        _mediaService = mediaService;
    }

    public async Task<ListingDto<CompanyDto>> ExecuteAsync(ListCompaniesDto dto)
    {
        var (companies, totalCount) = await _companyRepository.ListWithChildrenAsync(
            dto.Search, dto.Status, dto.Page, dto.PageSize);

        if (companies.Count == 0)
        {
            return new ListingDto<CompanyDto>
            {
                Data = new List<CompanyDto>(),
                Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
            };
        }

        var data = companies.Select(company =>
        {
            var companyDto = _mapper.Map<CompanyDto>(company);
            foreach (var doc in companyDto.CompanyDocuments.Where(d => !string.IsNullOrWhiteSpace(d.Path) && d.Document != null))
                doc.FileUrl = _mediaService.Read(CompanyDocumentPathHelper.BuildFilePath(company.Id, doc.Document!.NameEn, doc.Path!));
            return companyDto;
        }).ToList();

        return new ListingDto<CompanyDto>
        {
            Data = data,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
