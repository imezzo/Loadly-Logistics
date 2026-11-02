using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Domain.Common.Helpers;

public static class BankTransferPayerValidator
{
    public static async Task ValidateAsync(
        long payerId,
        string payerType,
        ICompanyRepository companyRepository,
        ILeadRepository leadRepository)
    {
        if (!BankTransferPayerTypes.All.Contains(payerType))
            throw new ValidationException("validation.invalid_payer_type");

        switch (payerType)
        {
            case BankTransferPayerTypes.Company:
                await companyRepository.GetByIdOrThrowAsync(payerId);
                break;
            case BankTransferPayerTypes.Lead:
                await leadRepository.GetByIdOrThrowAsync(payerId);
                break;
        }
    }
}
