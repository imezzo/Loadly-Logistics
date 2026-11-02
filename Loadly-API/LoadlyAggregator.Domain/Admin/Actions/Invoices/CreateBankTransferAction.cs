using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;

namespace LoadlyAggregator.Domain.Admin.Actions.Invoices;

public class CreateBankTransferAction
{
    private readonly IBankTransferRepository _bankTransferRepository;
    private readonly ICompanyRepository _companyRepository;
    private readonly ILeadRepository _leadRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediaService _mediaService;

    public CreateBankTransferAction(
        IBankTransferRepository bankTransferRepository,
        ICompanyRepository companyRepository,
        ILeadRepository leadRepository,
        ICurrentUserService currentUserService,
        IMediaService mediaService)
    {
        _bankTransferRepository = bankTransferRepository;
        _companyRepository = companyRepository;
        _leadRepository = leadRepository;
        _currentUserService = currentUserService;
        _mediaService = mediaService;
    }

    public async Task ExecuteAsync(CreateBankTransferDto dto)
    {
        await BankTransferPayerValidator.ValidateAsync(
            dto.PayerId, dto.PayerType, _companyRepository, _leadRepository);

        var entity = new BankTransfer
        {
            Amount = dto.Amount,
            IssueDate = dto.IssueDate,
            PaymentMethod = dto.PaymentMethod,
            ExternalPaymentReferenceNumber = dto.ExternalPaymentReferenceNumber,
            PayerId = dto.PayerId,
            PayerType = dto.PayerType,
            AddedBy = _currentUserService.GetCurrentUserId()
        };

        await _bankTransferRepository.AddAsync(entity);
        await _bankTransferRepository.SaveChangesAsync();

        if (dto.Attachment != null && dto.Attachment.Length > 0)
        {
            var directoryPath = $"bank-transfer/{entity.Id}";
            var fileName = await _mediaService.UploadAsync(dto.Attachment, directoryPath);
            entity.Attachment = $"{directoryPath}/{fileName}";
            _bankTransferRepository.Update(entity);
            await _bankTransferRepository.SaveChangesAsync();
        }
    }
}
