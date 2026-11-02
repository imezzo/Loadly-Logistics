using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Contractors;

public class ListContractorsAction
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IMapper _mapper;
    private readonly IMediaService _mediaService;

    public ListContractorsAction(
        IContractorRepository contractorRepository,
        IMapper mapper,
        IMediaService mediaService)
    {
        _contractorRepository = contractorRepository;
        _mapper = mapper;
        _mediaService = mediaService;
    }

    public async Task<ListingDto<ContractorDto>> ExecuteAsync(ListContractorsDto dto)
    {
        var (contractors, totalCount) = await _contractorRepository.ListWithChildrenAsync(
            dto.Search,
            dto.ContractorStatus,
            dto.Status,
            dto.Page,
            dto.PageSize,
            dto.TripStartDate,
            dto.TripEndDate);

        if (contractors.Count == 0)
        {
            return new ListingDto<ContractorDto>
            {
                Data = new List<ContractorDto>(),
                Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
            };
        }

        var countsById = await _contractorRepository.GetAggregateCountsByIdsAsync(
            contractors.Select(c => c.Id).ToList(),
            dto.TripStartDate,
            dto.TripEndDate);

        var data = contractors.Select(contractor =>
        {
            var contractorDto = _mapper.Map<ContractorDto>(contractor);
            if (countsById.TryGetValue(contractor.Id, out var counts))
            {
                ContractorHelper.ApplyAggregateCounts(contractorDto, counts);
            }

            ContractorHelper.ApplyDocumentFileUrls(contractor.Id, contractorDto.Documents, _mediaService);
            return contractorDto;
        }).ToList();

        return new ListingDto<ContractorDto>
        {
            Data = data,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
