using LoadlyAggregator.Domain.Common.Entities;

namespace LoadlyAggregator.Domain.Common.TripExtraFeePricing;

internal interface IExtraFeeCalculator
{
    decimal Calculate(ContractExtraFee extraFee, TripExtraFeeInputs inputs);
}
