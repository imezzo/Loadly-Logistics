using LoadlyAggregator.Domain.Common.BaseClasses;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Common.Entities;

public class Trip : BaseEntity
{
    public long? ContractId { get; set; }
    public long? ContractVehicleTypeId { get; set; }
    public long? ContractSlotId { get; set; }
    public long? VehicleId { get; set; }
    public long? TailVehicleId { get; set; }
    public long? DriverId { get; set; }
    public long? ContractorId { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    /// <summary>Calendar day used for monthly Included and Excluded contract billing.</summary>
    public DateOnly? FinancialDate { get; set; }
    public TripStatus Status { get; set; } = TripStatus.Pending;
    public TripDocumentStatus TripDocumentStatus { get; set; } = TripDocumentStatus.Pending;
    public TripCustomsStatus CustomsStatus { get; set; } = TripCustomsStatus.None;
    public decimal? CustomsTotalAmount { get; set; }
    public long? DispatcherId { get; set; }
    public string? DispatcherNotes { get; set; }
    public string? ProofOfDelivery { get; set; }
    public long? AuditedById { get; set; }
    public DateTime? AuditedAt { get; set; }
    public long? CompanyUserId { get; set; }
    public decimal? ContractDistanceKm { get; set; }
    public decimal? ActualDistanceKm { get; set; }
    public decimal? ActualCargoWeightTons { get; set; }
    public decimal? CargoPrice { get; set; }
    public decimal? SpotPrice { get; set; }
    public string? ContactPersonName { get; set; }
    public string? ContactPersonMobile { get; set; }
    public int? HelpersCount { get; set; }
    public int? NumberOfDays { get; set; }
    public OverNightCostBearer? OverNightCostBearer { get; set; }
    public TripDispatchingMode? DispatchingMode { get; set; }
    public TripType? Type { get; set; }
    public decimal? TotalAddonsCost { get; set; }
    public decimal? TotalTripCost { get; set; }
    public decimal? ContractorPrice { get; set; }
    public bool IsExternalTrip { get; set; } = false;
    public bool? IsBackfill { get; set; }
    public long? CollectorId { get; set; }
    public long? FinanceCollectorId { get; set; }
    public long? InvoiceId { get; set; }
    public virtual Invoice? Invoice { get; set; }
    public long? ClaimInvoiceId { get; set; }
    public virtual Invoice? ClaimInvoice { get; set; }
    public long? ContractorPayoutId { get; set; }
    public virtual ContractorPayout? ContractorPayout { get; set; }
    public long? LeadId { get; set; }
    public virtual Lead? Lead { get; set; }
    public virtual Contract? Contract { get; set; }
    public virtual ContractVehicleType? ContractVehicleType { get; set; }
    public virtual ContractSlot? ContractSlot { get; set; }
    public virtual Vehicle? Vehicle { get; set; }
    public virtual Vehicle? TailVehicle { get; set; }
    public virtual Driver? Driver { get; set; }
    public virtual Contractor? Contractor { get; set; }
    public virtual Admin? Dispatcher { get; set; }
    public virtual Admin? AuditedBy { get; set; }
    public virtual Collector? Collector { get; set; }
    public virtual Collector? FinanceCollector { get; set; }
    public virtual Company? CompanyUser { get; set; }

    public virtual ICollection<TripWaypoint>? Waypoints { get; set; }
    public virtual ICollection<Recording>? Recordings { get; set; }
    public virtual ICollection<TripCharge>? TripCharges { get; set; }
    public virtual ICollection<TripDocument> TripDocuments { get; set; } = new List<TripDocument>();
    public virtual ICollection<TripBookingInvitation> BookingInvitations { get; set; } = new List<TripBookingInvitation>();
    public virtual ICollection<TripBookingRequest> BookingRequests { get; set; } = new List<TripBookingRequest>();
    public virtual ICollection<TripBookingInvitationRound> BookingInvitationRounds { get; set; } = new List<TripBookingInvitationRound>();
    public virtual TripContractDestination? ContractDestinationAssignment { get; set; }

    public virtual TripVehicleAssignmentSnapshot? VehicleAssignmentSnapshot { get; set; }
}

