using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

namespace LoadlyAggregator.Domain.Admin.Actions.Invoices;

public class ListBankTransfersAction
{
    private readonly IBankTransferRepository _bankTransferRepository;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;

    public ListBankTransfersAction(
        IBankTransferRepository bankTransferRepository,
        IMediaService mediaService,
        IMapper mapper)
    {
        _bankTransferRepository = bankTransferRepository;
        _mediaService = mediaService;
        _mapper = mapper;
    }

    public async Task<ListingDto<BankTransferDto>> ExecuteAsync(ListBankTransfersDto dto)
    {
        var (items, totalCount) = await _bankTransferRepository.ListAsync(
            dto.PaymentMethod,
            dto.PayerType,
            dto.PayerIds,
            dto.Search,
            dto.Page,
            dto.PageSize);

        var data = _mapper.Map<List<BankTransferDto>>(items);
        foreach (var bankTransferDto in data)
        {
            if (!string.IsNullOrWhiteSpace(bankTransferDto.Attachment))
                bankTransferDto.FileUrl = _mediaService.Read(bankTransferDto.Attachment);
        }

        return new ListingDto<BankTransferDto>
        {
            Data = data,
            Pagination = new PaginationDto(dto.Page, dto.PageSize, totalCount)
        };
    }
}
