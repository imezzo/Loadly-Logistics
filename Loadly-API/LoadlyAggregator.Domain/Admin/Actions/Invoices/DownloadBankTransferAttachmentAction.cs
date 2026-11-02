using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Admin.Actions.Invoices;

public class DownloadBankTransferAttachmentAction
{
    private readonly IBankTransferRepository _bankTransferRepository;
    private readonly IMediaService _mediaService;

    public DownloadBankTransferAttachmentAction(
        IBankTransferRepository bankTransferRepository,
        IMediaService mediaService)
    {
        _bankTransferRepository = bankTransferRepository;
        _mediaService = mediaService;
    }

    public async Task<string> ExecuteAsync(DownloadBankTransferAttachmentDto dto)
    {
        var entity = await _bankTransferRepository.GetByIdOrThrowAsync(dto.Id);
        if (string.IsNullOrWhiteSpace(entity.Attachment))
            throw new NotFoundException("bank_transfers.attachment_not_found");

        return _mediaService.Read(entity.Attachment);
    }
}
