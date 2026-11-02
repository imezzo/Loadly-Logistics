using System.Linq;
using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Contracts;

public class GetContractAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IMapper _mapper;
    private readonly IMediaService _mediaService;

    public GetContractAction(
        IContractRepository contractRepository,
        IMapper mapper,
        IMediaService mediaService)
    {
        _contractRepository = contractRepository;
        _mapper = mapper;
        _mediaService = mediaService;
    }

    public async Task<ContractDto> ExecuteAsync(GetContractDto dto)
    {
        var contract = await _contractRepository.GetByIdOrThrowAsync(dto.Id);
        var contractDto = _mapper.Map<ContractDto>(contract);

        foreach (var doc in contractDto.Documents.Where(d => !string.IsNullOrWhiteSpace(d.Path) && d.Document != null))
            doc.FileUrl = _mediaService.Read(ContractDocumentPathHelper.BuildFilePath(contract.Id, doc.Document!.NameEn, doc.Path!));

        contractDto.TotalTrips = contract.Trips?.Count ?? 0;
        contractDto.TotalUnInvoicedTrips = TripInvoicingStatusResolver.CountNotFullyInvoiced(contract.Trips);
        contractDto.TotalCompletedTrips = contract.Trips?.Count(t => t.Status == TripStatus.Completed) ?? 0;
        contractDto.TotalTripsCost = contract.Trips?.Sum(t => t.TotalTripCost ?? 0) ?? 0;
        contractDto.TotalCompletedTripsCost = contract.Trips?.Where(t => t.Status == TripStatus.Completed).Sum(t => t.TotalTripCost ?? 0) ?? 0;
        contractDto.ExtraFees.RemoveAll(e => e.ExtraFeeTypeId == (long)ExtraFeeTypes.Fuel);

        return contractDto;
    }
}
