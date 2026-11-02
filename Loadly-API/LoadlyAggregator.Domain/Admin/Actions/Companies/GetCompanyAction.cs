using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Companies;

public class GetCompanyAction
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;

    public GetCompanyAction(
        ICompanyRepository companyRepository,
        IMediaService mediaService,
        IMapper mapper)
    {
        _companyRepository = companyRepository;
        _mediaService = mediaService;
        _mapper = mapper;
    }

    public async Task<CompanyDto> ExecuteAsync(GetCompanyDto dto)
    {
        var company = await _companyRepository.GetByIdOrThrowAsync(dto.Id);

        var companyDto = _mapper.Map<CompanyDto>(company);
        foreach (var doc in companyDto.CompanyDocuments.Where(d => !string.IsNullOrWhiteSpace(d.Path) && d.Document != null))
            doc.FileUrl = _mediaService.Read(CompanyDocumentPathHelper.BuildFilePath(company.Id, doc.Document!.NameEn, doc.Path!));
        return companyDto;
    }
}
