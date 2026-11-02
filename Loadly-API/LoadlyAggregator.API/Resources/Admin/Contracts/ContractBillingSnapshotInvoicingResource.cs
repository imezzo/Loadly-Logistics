using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.API.Resources.Admin.Contracts;

public class ContractBillingSnapshotInvoicingResource
{
    public long? TripsInvoiceId { get; set; }
    public long? ClaimsInvoiceId { get; set; }
    public int? ExtraKmOverageInvoiceType { get; set; }
    public string? ExtraKmOverageInvoiceTypeName { get; set; }
    public bool RequiresTripsInvoice { get; set; }
    public bool RequiresClaimsInvoice { get; set; }
    public List<ContractBillingSnapshotSlotChargeInvoicingResource> SlotCharges { get; set; } = [];

    public static ContractBillingSnapshotInvoicingResource FromDto(ContractBillingSnapshotInvoicingDto dto) =>
        new()
        {
            TripsInvoiceId = dto.TripsInvoiceId,
            ClaimsInvoiceId = dto.ClaimsInvoiceId,
            ExtraKmOverageInvoiceType = dto.ExtraKmOverageInvoiceType is { } type ? (int)type : null,
            ExtraKmOverageInvoiceTypeName = dto.ExtraKmOverageInvoiceType?.ToLocalizedDisplay(),
            RequiresTripsInvoice = dto.RequiresTripsInvoice,
            RequiresClaimsInvoice = dto.RequiresClaimsInvoice,
            SlotCharges = dto.SlotCharges.Select(ContractBillingSnapshotSlotChargeInvoicingResource.FromDto).ToList()
        };
}

public class ContractBillingSnapshotSlotChargeInvoicingResource
{
    public int ChargeKind { get; set; }
    public string ChargeKindName { get; set; } = string.Empty;
    public decimal SnapshotAmount { get; set; }
    public int RoutedToInvoiceType { get; set; }
    public string RoutedToInvoiceTypeName { get; set; } = string.Empty;
    public bool IsInvoiced { get; set; }
    public long? InvoicedOnInvoiceId { get; set; }

    public static ContractBillingSnapshotSlotChargeInvoicingResource FromDto(
        ContractBillingSnapshotSlotChargeInvoicingDto dto) =>
        new()
        {
            ChargeKind = (int)dto.ChargeKind,
            ChargeKindName = dto.ChargeKind.ToLocalizedDisplay(),
            SnapshotAmount = dto.SnapshotAmount,
            RoutedToInvoiceType = (int)dto.RoutedToInvoiceType,
            RoutedToInvoiceTypeName = dto.RoutedToInvoiceType.ToLocalizedDisplay(),
            IsInvoiced = dto.IsInvoiced,
            InvoicedOnInvoiceId = dto.InvoicedOnInvoiceId
        };
}
