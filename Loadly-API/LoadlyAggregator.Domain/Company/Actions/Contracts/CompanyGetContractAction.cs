using System.Linq;
using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Contracts;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Company.Actions.Contracts;

public class CompanyGetContractAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IMapper _mapper;
    private readonly IMediaService _mediaService;

    public CompanyGetContractAction(
        IContractRepository contractRepository,
        IMapper mapper,
        IMediaService mediaService)
    {
        _contractRepository = contractRepository;
        _mapper = mapper;
        _mediaService = mediaService;
    }

    public async Task<ContractDto> ExecuteAsync(CompanyGetContractDto dto)
    {
        var contract = await _contractRepository.GetByIdOrThrowAsync(dto.Id);

        if (contract.CompanyId != dto.CompanyId)
        {
            throw new ForbiddenException("contracts.not_authorized");
        }

        if (contract.Status != ContractStatus.ACTIVE)
        {
            throw new NotFoundException("contracts.not_found_with_id", dto.Id);
        }

        var contractDto = _mapper.Map<ContractDto>(contract);

        foreach (var doc in contractDto.Documents.Where(d => !string.IsNullOrWhiteSpace(d.Path) && d.Document != null))
        {
            doc.FileUrl = _mediaService.Read(ContractDocumentPathHelper.BuildFilePath(contract.Id, doc.Document!.NameEn, doc.Path!));
        }

        contractDto.TotalTrips = contract.Trips?.Count ?? 0;
        contractDto.TotalCompletedTrips = contract.Trips?.Count(t => t.Status == TripStatus.Completed) ?? 0;
        contractDto.TotalTripsCost = contract.Trips?.Sum(t => t.TotalTripCost ?? 0) ?? 0;
        contractDto.TotalCompletedTripsCost = contract.Trips?.Where(t => t.Status == TripStatus.Completed).Sum(t => t.TotalTripCost ?? 0) ?? 0;
        contractDto.ExtraFees.RemoveAll(e => e.ExtraFeeTypeId == (long)ExtraFeeTypes.Fuel);

        return contractDto;
    }
}
