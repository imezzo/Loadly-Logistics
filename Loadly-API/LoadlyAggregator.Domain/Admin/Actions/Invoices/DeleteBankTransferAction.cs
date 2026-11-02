using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Domain.Common.Services.Validation;

namespace LoadlyAggregator.Domain.Admin.Actions.Invoices;

public class DeleteBankTransferAction
{
    private readonly IBankTransferRepository _bankTransferRepository;
    private readonly IInvoicePaymentRepository _invoicePaymentRepository;

    public DeleteBankTransferAction(
        IBankTransferRepository bankTransferRepository,
        IInvoicePaymentRepository invoicePaymentRepository)
    {
        _bankTransferRepository = bankTransferRepository;
        _invoicePaymentRepository = invoicePaymentRepository;
    }

    public async Task ExecuteAsync(DeleteBankTransferDto dto)
    {
        var entity = await _bankTransferRepository.GetByIdOrThrowAsync(dto.Id);
        await BankTransferAllocationValidator.ValidateCanDeleteAsync(entity, _invoicePaymentRepository);
        _bankTransferRepository.Remove(entity);
        await _bankTransferRepository.SaveChangesAsync();
    }
}
