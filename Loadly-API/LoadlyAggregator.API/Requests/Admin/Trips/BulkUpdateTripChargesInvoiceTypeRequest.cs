using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.API.Requests.Admin.Trips;

public class BulkUpdateTripChargesInvoiceTypeRequest
{
    public const string Permission = UpdateTripChargesRequest.Permission;

    public List<long> TripIds { get; set; } = [];
    public List<ChargeInvoiceTypeItemRequest> Charges { get; set; } = [];

    public BulkUpdateTripChargesInvoiceTypeDto FromRequest() => new()
    {
        TripIds = TripIds,
        Charges = Charges
            .Select(c => new TripChargeInvoiceTypeDto
            {
                ChargeType = (TripChargeType)c.ChargeType,
                InvoiceType = c.InvoiceType.HasValue ? (InvoiceType)c.InvoiceType.Value : null
            })
            .ToList()
    };
}

public class ChargeInvoiceTypeItemRequest
{
    public int ChargeType { get; set; }
    public int? InvoiceType { get; set; }
}

public class BulkUpdateTripChargesInvoiceTypeRequestValidator
    : AbstractValidator<BulkUpdateTripChargesInvoiceTypeRequest>
{
    public BulkUpdateTripChargesInvoiceTypeRequestValidator()
    {
        RuleFor(x => x.TripIds)
            .IsRequiredNonEmptyCollection("TripIds");

        RuleForEach(x => x.TripIds)
            .IsRequiredId("Trip ID");

        RuleFor(x => x.Charges)
            .IsRequiredNonEmptyCollection("Charges");

        RuleFor(x => x.Charges)
            .Must(list => list.Select(c => c.ChargeType).Distinct().Count() == list.Count)
            .WithMessage(TripMessageKeys.DuplicateTripChargeTypeInUpdateRequest);

        RuleForEach(x => x.Charges).ChildRules(charge =>
        {
            charge.RuleFor(c => c.ChargeType)
                .Must(t => Enum.IsDefined(typeof(TripChargeType), t))
                .WithMessage("validation.trip_charge_type_invalid");

            charge.RuleFor(c => c.InvoiceType)
                .Must(t => !t.HasValue || Enum.IsDefined(typeof(InvoiceType), t.Value))
                .WithMessage("validation.invoice_type_invalid");
        });
    }
}
