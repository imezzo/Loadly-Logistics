using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
namespace LoadlyAggregator.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    private readonly IEnumerable<IInterceptor>? _interceptors;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IEnumerable<IInterceptor>? interceptors = null) : base(options)
    {
        _interceptors = interceptors;
    }

    public DbSet<Admin> Admins { get; set; }
    public DbSet<Collector> Collectors { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<Lead> Leads { get; set; }
    public DbSet<CompanyDetail> CompanyDetails { get; set; }
    public DbSet<CompanyDocument> CompanyDocuments { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Contractor> Contractors { get; set; }
    public DbSet<ContractorDocument> ContractorDocuments { get; set; }
    public DbSet<DriverDetail> DriverDetails { get; set; }
    public DbSet<DriverLocation> DriverLocations { get; set; }
    public DbSet<DriverLicenseType> DriverLicenseTypes { get; set; }
    public DbSet<DriverDocument> DriverDocuments { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<Governerate> Governerates { get; set; }
    public DbSet<Sector> Sectors { get; set; }
    public DbSet<District> Districts { get; set; }
    public DbSet<IndustryType> IndustryTypes { get; set; }
    public DbSet<ServiceRequirement> ServiceRequirements { get; set; }
    public DbSet<CommunicationMethod> CommunicationMethods { get; set; }
    public DbSet<Document> Documents { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<VehicleType> VehicleTypes { get; set; }
    public DbSet<Tax> Taxes { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<Color> Colors { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<VehicleTypeTag> VehicleTypeTags { get; set; }
    public DbSet<FuelType> FuelTypes { get; set; }
    public DbSet<LookupDocumentStatus> LookupDocumentStatuses { get; set; }
    public DbSet<VehicleCapacityUnit> VehicleCapacityUnits { get; set; }
    public DbSet<Port> Ports { get; set; }
    public DbSet<Container> Containers { get; set; }
    public DbSet<ShippingLine> ShippingLines { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<VehicleInsurance> VehicleInsurances { get; set; }
    public DbSet<VehicleFacility> VehicleFacilities { get; set; }
    public DbSet<VehicleFacilityVisit> VehicleFacilityVisits { get; set; }
    public DbSet<VehicleAssignmentHistory> VehicleAssignmentHistories { get; set; }
    public DbSet<VehicleBrand> VehicleBrands { get; set; }
    public DbSet<VehicleBrandModel> VehicleBrandModels { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<ContractSlot> ContractSlots { get; set; }
    public DbSet<ContractBillingSnapshot> ContractBillingSnapshots { get; set; }
    public DbSet<ContractBillingSnapshotCalendarDay> ContractBillingSnapshotCalendarDays { get; set; }
    public DbSet<ContractBillingSnapshotChargeLine> ContractBillingSnapshotChargeLines { get; set; }
    public DbSet<ContractBillingSnapshotKmEvidence> ContractBillingSnapshotKmEvidence { get; set; }
    public DbSet<ContractBillingSnapshotCommercialOverride> ContractBillingSnapshotCommercialOverrides { get; set; }
    public DbSet<ContractVehicleType> ContractVehicleTypes { get; set; }
    public DbSet<ContractExtraFee> ContractExtraFees { get; set; }
    public DbSet<ContractApproval> ContractApprovals { get; set; }
    public DbSet<ContractStatusHistory> ContractStatusHistories { get; set; }
    public DbSet<ContractDocument> ContractDocuments { get; set; }
    public DbSet<OperatingHour> OperatingHours { get; set; }
    public DbSet<Trip> Trips { get; set; }
    public DbSet<TripBookingInvitation> TripBookingInvitations { get; set; }
    public DbSet<TripBookingRequest> TripBookingRequests { get; set; }
    public DbSet<TripBookingInvitationRound> TripBookingInvitationRounds { get; set; }
    public DbSet<TripDocument> TripDocuments { get; set; }
    public DbSet<TripCharge> TripCharges { get; set; }
    public DbSet<TripWaypoint> TripWaypoints { get; set; }
    public DbSet<TripContractDestination> TripContractDestinations { get; set; }
    public DbSet<TripRouteSync> TripRouteSyncs { get; set; }
    public DbSet<TripVehicleAssignmentSnapshot> TripVehicleAssignmentSnapshots { get; set; }
    public DbSet<VehiclePlateAlias> VehiclePlateAliases { get; set; }
    public DbSet<Waypoint> Waypoints { get; set; }
    public DbSet<ContractDestination> ContractDestinations { get; set; }
    public DbSet<Recording> Recordings { get; set; }

    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<PermissionGroup> PermissionGroups { get; set; }
    public DbSet<ModelHasRole> ModelHasRoles { get; set; }
    public DbSet<RoleHasPermission> RoleHasPermissions { get; set; }
    public DbSet<ModelHasPermission> ModelHasPermissions { get; set; }
    public DbSet<MigrationTest> MigrationTests { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<DeviceToken> DeviceTokens { get; set; }
    public DbSet<GeneralSetting> GeneralSettings { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
    public DbSet<ContractorPayout> ContractorPayouts { get; set; }
    public DbSet<InvoicePayment> InvoicePayments { get; set; }
    public DbSet<BankTransfer> BankTransfers { get; set; }
    public DbSet<Target> Targets { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationPreference> NotificationPreferences { get; set; }
    public DbSet<BroadcastNotification> BroadcastNotifications { get; set; }
    public DbSet<BroadcastNotificationAttachment> BroadcastNotificationAttachments { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        if (_interceptors != null)
        {
            foreach (var interceptor in _interceptors)
            {
                optionsBuilder.AddInterceptors(interceptor);
            }
        }
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly,
            type => type.Namespace == typeof(ApplicationDbContext).Namespace + ".Configurations"
                    && type.Name != "AuditLogConfiguration"
                    && type.Name != "NotificationsLogConfiguration"
                    && type.Name != "OdometerScanLogConfiguration");

        ApplySoftDeleteQueryFilter(modelBuilder);
    }





    private void ApplySoftDeleteQueryFilter(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {

            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {

                var parameter = Expression.Parameter(entityType.ClrType, "entity");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.DeletedAt));
                var filter = Expression.Lambda(
                    Expression.Equal(property, Expression.Constant(null, typeof(DateTime?))),
                    parameter
                );

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(filter);
            }
        }
    }
}
