using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Contractors;

public class GetContractorAction
{
    private readonly IContractorRepository _contractorRepository;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;

    public GetContractorAction(
        IContractorRepository contractorRepository,
        IMediaService mediaService,
        IMapper mapper)
    {
        _contractorRepository = contractorRepository;
        _mediaService = mediaService;
        _mapper = mapper;
    }

    public async Task<ContractorDto> ExecuteAsync(GetContractorDto dto)
    {
        var contractor = await _contractorRepository.GetByIdWithChildrenOrThrowAsync(dto.Id);
        var counts = await _contractorRepository.GetAggregateCountsAsync(dto.Id);

        var contractorDto = _mapper.Map<ContractorDto>(contractor);
        ContractorHelper.ApplyAggregateCounts(contractorDto, counts);
        ContractorHelper.ApplyDocumentFileUrls(contractor.Id, contractorDto.Documents, _mediaService);

        return contractorDto;
    }
}
