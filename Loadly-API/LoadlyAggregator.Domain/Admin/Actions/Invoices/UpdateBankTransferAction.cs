using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;

namespace LoadlyAggregator.Domain.Admin.Actions.Invoices;

public class UpdateBankTransferAction
{
    private readonly IBankTransferRepository _bankTransferRepository;
    private readonly IInvoicePaymentRepository _invoicePaymentRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILeadRepository _leadRepository;
    private readonly IMediaService _mediaService;

    public UpdateBankTransferAction(
        IBankTransferRepository bankTransferRepository,
        IInvoicePaymentRepository invoicePaymentRepository,
        ICompanyRepository companyRepository,
        ILeadRepository leadRepository,
        IMediaService mediaService)
    {
        _bankTransferRepository = bankTransferRepository;
        _invoicePaymentRepository = invoicePaymentRepository;
        _companyRepository = companyRepository;
        _leadRepository = leadRepository;
        _mediaService = mediaService;
    }

    public async Task ExecuteAsync(UpdateBankTransferDto dto)
    {
        await BankTransferPayerValidator.ValidateAsync(
            dto.PayerId, dto.PayerType, _companyRepository, _leadRepository);

        var entity = await _bankTransferRepository.GetByIdOrThrowAsync(dto.Id);
        await BankTransferAllocationValidator.ValidateCanUpdateAsync(
            entity, dto.Amount, dto.PaymentMethod, dto.PayerId, dto.PayerType, _invoicePaymentRepository);

        entity.Amount = dto.Amount;
        entity.IssueDate = dto.IssueDate;
        entity.PaymentMethod = dto.PaymentMethod;
        entity.ExternalPaymentReferenceNumber = dto.ExternalPaymentReferenceNumber;
        entity.PayerId = dto.PayerId;
        entity.PayerType = dto.PayerType;

        if (dto.Attachment != null && dto.Attachment.Length > 0)
        {
            var directoryPath = $"bank-transfer/{entity.Id}";
            if (!string.IsNullOrWhiteSpace(entity.Attachment))
            {
                try
                {
                    await _mediaService.RemoveAsync(entity.Attachment);
                }
                catch
                {
                    // ponytail: best-effort cleanup; stale file possible if remove fails
                }
            }

            var fileName = await _mediaService.UploadAsync(dto.Attachment, directoryPath);
            entity.Attachment = $"{directoryPath}/{fileName}";
        }

        _bankTransferRepository.Update(entity);
        await _bankTransferRepository.SaveChangesAsync();
    }
}
