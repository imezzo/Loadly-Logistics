using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Invoices;

public class GetBankTransferAction
{
    private readonly IBankTransferRepository _bankTransferRepository;
    private readonly IMediaService _mediaService;
    private readonly IMapper _mapper;

    public GetBankTransferAction(
        IBankTransferRepository bankTransferRepository,
        IMediaService mediaService,
        IMapper mapper)
    {
        _bankTransferRepository = bankTransferRepository;
        _mediaService = mediaService;
        _mapper = mapper;
    }

    public async Task<BankTransferDto> ExecuteAsync(GetBankTransferDto dto)
    {
        var entity = await _bankTransferRepository.GetByIdOrThrowAsync(dto.Id);
        var bankTransferDto = _mapper.Map<BankTransferDto>(entity);
        if (!string.IsNullOrWhiteSpace(bankTransferDto.Attachment))
            bankTransferDto.FileUrl = _mediaService.Read(bankTransferDto.Attachment);

        return bankTransferDto;
    }
}
