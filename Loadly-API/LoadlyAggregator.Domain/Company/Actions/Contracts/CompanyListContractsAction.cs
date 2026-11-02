using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Company.DTOs.Contracts;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Company.Actions.Contracts;

public class CompanyListContractsAction
{
    private readonly IContractRepository _contractRepository;
    private readonly IMapper _mapper;
    private readonly IMediaService _mediaService;

    public CompanyListContractsAction(
        IContractRepository contractRepository,
        IMapper mapper,
        IMediaService mediaService)
    {
        _contractRepository = contractRepository;
        _mapper = mapper;
        _mediaService = mediaService;
    }

    public async Task<ListingDto<ContractDto>> ExecuteAsync(CompanyListContractsDto dto)
    {
        var (contracts, totalCount) = await _contractRepository.ListWithChildrenAsync(
            dto.Search,
            dto.ContractId,
            dto.CompanyId,
            dto.ContractType,
            salesRepId: null,
            ContractStatus.ACTIVE,
            dto.ReferenceNumber,
            dto.Page,
            dto.PageSize);

        if (contracts.Count == 0)
        {
            return new ListingDto<ContractDto>
            {
                Data = new List<ContractDto>(),
                Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
            };
        }

        var data = contracts
            .Select(contract =>
            {
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
            })
            .ToList();

        return new ListingDto<ContractDto>
        {
            Data = data,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
