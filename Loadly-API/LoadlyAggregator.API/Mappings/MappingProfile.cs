using AutoMapper;
using LoadlyAggregator.Domain.Admin.Helpers;
using LoadlyAggregator.Domain.Admin.DTOs.Admins;
using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Domain.Admin.DTOs.Companies;
using LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;
using LoadlyAggregator.Domain.Admin.DTOs.Drivers;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;
using LoadlyAggregator.Domain.Admin.DTOs.Leads;
using LoadlyAggregator.Domain.Admin.DTOs.Logs;
using LoadlyAggregator.Domain.Admin.DTOs.Roles;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Admin.DTOs.VehicleFacilities;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.DTOs.Addresses;
using LoadlyAggregator.Domain.Common.DTOs.Contacts;
using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Services.Validation;
using LoadlyAggregator.Domain.Company.DTOs.Roles;
using LoadlyAggregator.Domain.Contractor.DTOs.Trips;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.Constants;
using LoadlyAggregator.Shared.Utilities;
namespace LoadlyAggregator.API.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Admin Entity to DTO mappings
        CreateMap<Admin, AdminDto>()
            .ForMember(dest => dest.MfaChannel, opt => opt.MapFrom(src => (int)src.MfaChannel))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type))
            .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Addresses))
            .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => src.Contacts));

        CreateMap<Collector, LoadlyAggregator.Domain.Admin.DTOs.Collectors.CollectorDto>()
            .ForMember(dest => dest.MfaChannel, opt => opt.MapFrom(src => (int)src.MfaChannel))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Addresses))
            .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => src.Contacts));

        CreateMap<Admin, AdminRelationDto>();

        CreateMap<Contractor, ContractorDto>()
            .ForMember(dest => dest.MfaChannel, opt => opt.MapFrom(src => (int)src.MfaChannel))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.ContractorStatusName, opt => opt.MapFrom(src => src.ContractorStatus.ToLocalizedDisplay()))
            .ForMember(dest => dest.TotalDrivers, opt => opt.MapFrom(src => src.Drivers.Count))
            .ForMember(dest => dest.TotalVehicles, opt => opt.MapFrom(src => src.Vehicles.Count))
            .ForMember(dest => dest.TotalTrips, opt => opt.MapFrom(src => src.Trips.Count))
            .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.ContractorDocuments));

        CreateMap<ContractorDocument, ContractorDocumentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Document, opt => opt.MapFrom(src => src.Document));

        // Company Entity to DTO mappings
        CreateMap<Company, CompanyDto>()
            .ForMember(dest => dest.MfaChannel, opt => opt.MapFrom(src => (int)src.MfaChannel))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type))
            .ForMember(dest => dest.CompanyDocuments, opt => opt.MapFrom(src => src.CompanyDocuments));

        CreateMap<Company, CompanyUserDto>()
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.ParentCompany));

        CreateMap<Company, UserCompanyDetailsDto>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.NameAr, opt => opt.MapFrom(src => src.CompanyDetail != null ? src.CompanyDetail.NameAr : string.Empty))
            .ForMember(dest => dest.NameEn, opt => opt.MapFrom(src => src.CompanyDetail != null ? src.CompanyDetail.NameEn : string.Empty))
            .ForMember(dest => dest.DefaultUser, opt => opt.MapFrom(src => src));

        CreateMap<Company, DefaultUserDto>();

        CreateMap<CompanyDetail, CompanyDetailDto>()
            .ForMember(dest => dest.VerificationStatus, opt => opt.MapFrom(src => (int)src.VerificationStatus));

        CreateMap<Address, AddressDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<Contact, ContactDto>();

        CreateMap<Document, DocumentDto>();

        CreateMap<Discount, DiscountDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<Tax, TaxDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<Lead, LeadDto>();

        CreateMap<Target, TargetDto>();

        CreateMap<LookupDocumentStatus, DocumentStatusDto>();

        CreateMap<DriverLicenseType, DriverLicenseTypeDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<City, CityDto>()
            .ForMember(dest => dest.Governerate, opt => opt.MapFrom(src => src.Governerate));

        CreateMap<District, DistrictDto>()
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City));

        CreateMap<Governerate, GovernerateDto>();

        CreateMap<Sector, SectorDto>();

        CreateMap<Color, ColorDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<Tag, TagDto>();

        CreateMap<FuelType, FuelTypeDto>();

        CreateMap<VehicleFacility, VehicleFacilityDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.FacilityType, opt => opt.MapFrom(src => (int)src.FacilityType))
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company))
            .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => src.Contacts))
            .ForMember(dest => dest.VehicleFacilityVisits, opt => opt.MapFrom(src =>
                src.VehicleFacilityVisits.OrderByDescending(v => v.EventDate).ThenByDescending(v => v.Id)));

        CreateMap<VehicleType, LoadlyAggregator.Domain.Admin.DTOs.Vehicles.VehicleTypeDto>()
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src =>
                src.VehicleTypeTags
                    .Where(vtt => vtt.Tag != null)
                    .Select(vtt => vtt.Tag)));

        CreateMap<VehicleBrand, VehicleBrandDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<VehicleBrandModel, VehicleBrandModelDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<VehicleCapacityUnit, VehicleCapacityUnitDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<Port, PortDto>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (int)src.Type))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<Container, ContainerDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<ShippingLine, ShippingLineDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<VehicleInsurance, VehicleInsuranceDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<ServiceRequirement, ServiceRequirementDto>();

        CreateMap<IndustryType, IndustryTypeDto>();

        CreateMap<NotificationsLog, NotificationsLogDto>();

        CreateMap<OperatingHour, OperatingHoursDto>();

        CreateMap<CompanyDocument, CompanyDocumentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Document, opt => opt.MapFrom(src => src.Document))
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company));

        // Driver Entity to DTO mappings
        CreateMap<Driver, DriverDto>()
            .ForMember(dest => dest.FullNameEn, opt => opt.MapFrom(src => src.FullName ?? string.Empty))
            .ForMember(dest => dest.NationalId, opt => opt.MapFrom(src => src.NationalId ?? string.Empty))
            .ForMember(dest => dest.MfaChannel, opt => opt.MapFrom(src => (int)src.MfaChannel))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.EmailVerified, opt => opt.MapFrom(src => src.EmailVerified))
            .ForMember(dest => dest.PhoneVerified, opt => opt.MapFrom(src => src.PhoneVerified))
            .ForMember(dest => dest.WorkingStatus, opt => opt.MapFrom(src => src.WorkingStatus.HasValue ? (int?)src.WorkingStatus.Value : null))
            .ForMember(dest => dest.Addresses, opt => opt.MapFrom(src => src.Addresses))
            .ForMember(dest => dest.Contacts, opt => opt.MapFrom(src => src.Contacts));

        CreateMap<DriverDetail, DriverDetailDto>()
            .ForMember(dest => dest.LicenseCategory, opt => opt.MapFrom(src => src.LicenseCategory.HasValue ? (int?)src.LicenseCategory.Value : null))
            .ForMember(dest => dest.SpokenLanguages, opt => opt.MapFrom(src => src.SpokenLanguages != null ? src.SpokenLanguages.Select(sl => (int)sl).ToList() : null));

        CreateMap<DriverDocument, DriverDocumentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Document, opt => opt.MapFrom(src => src.Document));

        // Vehicle
        CreateMap<VehicleFacility, VehicleFacilityRelationDto>()
            .ForMember(dest => dest.FacilityType, opt => opt.MapFrom(src => (int)src.FacilityType))
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company));
        CreateMap<VehicleFacilityVisit, VehicleFacilityVisitRelationDto>()
            .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event.ToLocalizedDisplay()))
            .ForMember(dest => dest.ContextName, opt => opt.MapFrom(src => src.Context.ToLocalizedDisplay()))
            .ForMember(dest => dest.VehicleFacility, opt => opt.MapFrom(src => src.VehicleFacility))
            .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
            .ForMember(dest => dest.Driver, opt => opt.MapFrom(src => src.Driver))
            .ForMember(dest => dest.AddedBy, opt => opt.MapFrom(src => src.AddedByAdmin));
        CreateMap<VehicleFacilityVisit, VehicleFacilityVisitOnVehicleRelationDto>()
            .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.Event.ToLocalizedDisplay()))
            .ForMember(dest => dest.ContextName, opt => opt.MapFrom(src => src.Context.ToLocalizedDisplay()))
            .ForMember(dest => dest.VehicleFacility, opt => opt.MapFrom(src => src.VehicleFacility))
            .ForMember(dest => dest.Driver, opt => opt.MapFrom(src => src.Driver))
            .ForMember(dest => dest.AddedBy, opt => opt.MapFrom(src => src.AddedByAdmin));
        CreateMap<VehiclePlateAlias, VehiclePlateChangeHistoryDto>()
            .ForMember(dest => dest.SupersededAt, opt => opt.MapFrom(src => src.CreatedAt));
        CreateMap<Vehicle, VehicleDto>()
            .ForMember(dest => dest.CurrentServiceFacility, opt => opt.MapFrom(src => src.CurrentServiceFacility))
            .ForMember(dest => dest.AssignedParkingFacility, opt => opt.MapFrom(src => src.AssignedParkingFacility))
            .ForMember(dest => dest.VehicleFacilityVisits, opt => opt.MapFrom(src =>
                src.VehicleFacilityVisits.OrderByDescending(v => v.EventDate).ThenByDescending(v => v.Id)))
            .ForMember(dest => dest.PlateChangeHistory, opt => opt.MapFrom(src =>
                src.PlateAliases.OrderByDescending(a => a.CreatedAt)));
        CreateMap<Vehicle, CoupledVehicleRelationDto>()
            .ForMember(dest => dest.ChassisNumber, opt => opt.MapFrom(src => src.VinNo))
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.VehicleBrandModel != null && src.VehicleBrandModel.VehicleBrand != null ? src.VehicleBrandModel.VehicleBrand.NameEn : null))
            .ForMember(dest => dest.ModelName, opt => opt.MapFrom(src => src.VehicleBrandModel != null ? src.VehicleBrandModel.NameEn : null))
            .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.MakeYear))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType));
        CreateMap<VehicleDetail, VehicleDetailDto>();
        // Vehicle documents data
        CreateMap<VehicleDocument, VehicleDocumentDataDto>();

        // Contract Entity to DTO mappings
        CreateMap<Contract, ContractDto>()
            .ForMember(dest => dest.Approvals, opt => opt.MapFrom(src => src.Approvals))
            .ForMember(dest => dest.StatusHistory, opt => opt.MapFrom(src => src.StatusHistories))
            .ForMember(dest => dest.SalesRep, opt => opt.MapFrom(src => src.SalesRep))
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company))
            .ForMember(dest => dest.VehicleTypes, opt => opt.MapFrom(src => src.ContractVehicleTypes))
            .ForMember(dest => dest.Documents, opt => opt.MapFrom(src => src.ContractDocuments))
            .ForMember(dest => dest.ExtraFees, opt => opt.MapFrom(src => src.ContractExtraFees))
            .ForMember(dest => dest.Destinations, opt => opt.MapFrom(src =>
                src.ContractDestinations.Where(d => d.Status != ContractDestinationStatus.ARCHIVED)))
            .ForMember(dest => dest.Slots, opt => opt.MapFrom(src =>
                src.ContractSlots.Where(s => s.Status != ContractSlotStatus.ARCHIVED)));

        CreateMap<ContractSlot, ContractSlotDto>()
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType))
            .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle));

        CreateMap<Company, LoadlyAggregator.Domain.Common.DTOs.Relations.CompanyRelationDto>()
            .ForMember(dest => dest.NameEn, opt => opt.MapFrom(src => src.CompanyDetail != null ? src.CompanyDetail.NameEn : string.Empty))
            .ForMember(dest => dest.NameAr, opt => opt.MapFrom(src => src.CompanyDetail != null ? src.CompanyDetail.NameAr : null))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.RegistrationNumber, opt => opt.MapFrom(src => src.CompanyDetail != null ? src.CompanyDetail.RegistrationNumber : null))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

        CreateMap<ContractVehicleType, LoadlyAggregator.Domain.Admin.DTOs.Contracts.ContractVehicleTypeDto>()
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType))
            .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract));

        CreateMap<ContractVehicleType, LoadlyAggregator.Domain.Common.DTOs.Relations.ContractVehicleTypeRelationDto>()
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType));

        CreateMap<Vehicle, LoadlyAggregator.Domain.Common.DTOs.Relations.VehicleRelationDto>()
            .ForMember(dest => dest.ChassisNumber, opt => opt.MapFrom(src => src.VinNo))
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.VehicleBrandModel != null && src.VehicleBrandModel.VehicleBrand != null ? src.VehicleBrandModel.VehicleBrand.NameEn : null))
            .ForMember(dest => dest.ModelName, opt => opt.MapFrom(src => src.VehicleBrandModel != null ? src.VehicleBrandModel.NameEn : null))
            .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.MakeYear))
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType));

        CreateMap<Driver, LoadlyAggregator.Domain.Common.DTOs.Relations.DriverRelationDto>()
            .ForMember(dest => dest.FullNameEn, opt => opt.MapFrom(src => src.FullName))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

        CreateMap<Lead, LoadlyAggregator.Domain.Common.DTOs.Relations.LeadRelationDto>();

        CreateMap<Contractor, ContractorRelationDto>();

        CreateMap<Sector, SectorRelationDto>();

        CreateMap<ContractDocument, ContractDocumentDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Document, opt => opt.MapFrom(src => src.Document))
            .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract));

        CreateMap<ContractExtraFee, ContractExtraFeeDto>()
            .ForMember(dest => dest.ExtraFeeTypeId, opt => opt.MapFrom(src => (long)src.ExtraFeeTypeId))
            .ForMember(dest => dest.ExtraFeeType, opt => opt.MapFrom<ContractExtraFeeToExtraFeeTypeDtoResolver>())
            .ForMember(dest => dest.Contract, opt => opt.Ignore());

        CreateMap<ContractDestination, DestinationRelationDto>()
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.StartWaypointNavigation))
            .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.EndWaypointNavigation));
        CreateMap<ContractDestination, ContractDestinationDto>()
            .IncludeBase<ContractDestination, DestinationRelationDto>();
        CreateMap<Waypoint, WaypointRelationDto>();

        CreateMap<ContractApproval, ContractApprovalDto>()
            .ForMember(dest => dest.StageName, opt => opt.MapFrom(src => src.Stage.ToLocalizedDisplay()))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToLocalizedDisplay()))
            .ForMember(dest => dest.ApproverName, opt => opt.MapFrom(src => src.Approver != null ? src.Approver.FullName : null))
            .ForMember(dest => dest.ApproverRole, opt => opt.MapFrom(src => src.Approver != null ? src.Approver.Type.ToString() : null));

        CreateMap<ContractStatusHistory, ContractStatusHistoryDto>()
            .ForMember(dest => dest.PreviousStatusName, opt => opt.MapFrom(src => src.PreviousStatus.ToLocalizedDisplay()))
            .ForMember(dest => dest.NewStatusName, opt => opt.MapFrom(src => src.NewStatus.ToLocalizedDisplay()))
            .ForMember(dest => dest.ApprovalStageName, opt => opt.MapFrom(src => src.ApprovalStage.HasValue ? src.ApprovalStage.Value.ToLocalizedDisplay() : null))
            .ForMember(dest => dest.ChangedByName, opt => opt.MapFrom(src => src.ChangedByAdmin != null ? src.ChangedByAdmin.FullName : null));

        CreateMap<TripVehicleAssignmentSnapshot, TripVehicleAssignmentSnapshotDto>();

        CreateMap<TripContractDestination, TripContractDestinationDto>();

        CreateMap<Invoice, InvoiceRelationDto>()
            .ForMember(dest => dest.Lead, opt => opt.MapFrom(src => src.Lead));

        CreateMap<Trip, TripDto>()
            .ForMember(dest => dest.TripContractDestinations, opt => opt.MapFrom(src => TripContractDestinationHelper.GetActiveAssignments(src)))
            .ForMember(dest => dest.NumberOfDays, opt => opt.MapFrom(src => src.NumberOfDays.HasValue && src.NumberOfDays.Value < 0 ? 0 : src.NumberOfDays))
            .ForMember(dest => dest.VehicleTypeId, opt => opt.MapFrom(src => src.ContractVehicleType != null ? (long?)src.ContractVehicleType.VehicleTypeId : null))
            .ForMember(dest => dest.TripStatusName, opt => opt.MapFrom(src => src.Status.ToLocalizedDisplay()))
            .ForMember(dest => dest.CurrentDriverStatus, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentDriverStatusName, opt => opt.Ignore())
            .ForMember(dest => dest.CustomsStatusName, opt => opt.MapFrom(src => src.CustomsStatus.ToLocalizedDisplay()))
            .ForMember(dest => dest.TripType, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.TripTypeName, opt => opt.MapFrom(src =>
                src.Type.HasValue ? src.Type.Value.ToLocalizedDisplay() : string.Empty))
            .ForMember(dest => dest.TotalAddonsCost, opt => opt.MapFrom(src => src.TotalAddonsCost))
            .ForMember(dest => dest.Driver, opt => opt.MapFrom(src => src.Driver))
            .ForMember(dest => dest.Contractor, opt => opt.MapFrom(src => src.Contractor))
            .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract))
            .ForMember(dest => dest.ContractVehicleType, opt => opt.MapFrom(src => src.ContractVehicleType))
            .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
            .ForMember(dest => dest.VehicleSnapshot, opt => opt.MapFrom(src => src.VehicleAssignmentSnapshot))
            .ForMember(dest => dest.TailVehicle, opt => opt.MapFrom(src => src.TailVehicle))
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Contract != null && src.Contract.Company != null ? src.Contract.Company : null))
            .ForMember(dest => dest.Lead, opt => opt.MapFrom(src => src.Lead))
            .ForMember(dest => dest.Dispatcher, opt => opt.MapFrom(src => src.Dispatcher))
            .ForMember(dest => dest.AuditedBy, opt => opt.MapFrom(src => src.AuditedBy))
            .ForMember(dest => dest.Waypoints, opt => opt.MapFrom(src => src.Waypoints != null && src.Waypoints.Count > 0 ? src.Waypoints.OrderBy(w => w.Sequence) : null))
            .ForMember(dest => dest.Recordings, opt => opt.MapFrom(src => src.Recordings != null && src.Recordings.Count > 0 ? src.Recordings.OrderBy(r => r.CreatedAt) : null))
            .ForMember(dest => dest.BookingRequests, opt => opt.MapFrom(src => src.BookingRequests != null && src.BookingRequests.Count > 0 ? src.BookingRequests.OrderByDescending(r => r.CreatedAt) : null))
            .ForMember(dest => dest.CurrentLocation, opt => opt.MapFrom(src =>
                (src.Status == TripStatus.Scheduled || src.Status == TripStatus.InProgress)
                && src.Driver != null && src.Driver.DriverLocation != null
                    ? src.Driver.DriverLocation
                    : null))
            .ForMember(dest => dest.StartLocation, opt => opt.MapFrom(src => TripMappingHelper.ExtractStartLocation(src)))
            .ForMember(dest => dest.CancellationReason, opt => opt.MapFrom(src => TripMappingHelper.ExtractCancellationReason(src)))
            .ForMember(dest => dest.CompletionTime, opt => opt.MapFrom(src => TripMappingHelper.ExtractCompletionTime(src)))
            .ForMember(dest => dest.CutoffDuration, opt => opt.MapFrom(_ => TripConstants.DefaultCutoffDuration))
            .ForMember(dest => dest.TripCharges, opt => opt.MapFrom(src => src.TripCharges))
            .ForMember(dest => dest.Invoice, opt => opt.MapFrom(src => src.Invoice))
            .ForMember(dest => dest.ClaimInvoice, opt => opt.MapFrom(src => src.ClaimInvoice))
            .ForMember(dest => dest.Documents, opt => opt.Ignore())
            .ForMember(dest => dest.CollectorDocuments, opt => opt.MapFrom(src =>
                src.TripDocuments == null || src.TripDocuments.Count == 0
                    ? null
                    : src.TripDocuments.Where(td => td.Document != null).OrderBy(td => td.DocumentId).ToList()))
            .AfterMap((src, dest) =>
            {
                var invoicing = TripInvoicingStatusResolver.Resolve(src);
                dest.InvoicingStatus = invoicing.Status;
                dest.InvoicingStatusName = invoicing.Status.ToLocalizedDisplay();
                dest.RequiresMainInvoice = invoicing.RequiresMainInvoice;
                dest.RequiresClaimInvoice = invoicing.RequiresClaimInvoice;
                dest.FullyInvoiced = invoicing.FullyInvoiced;
                dest.TotalBillableTripItemsAmount = TripChargeInvoiceRouting.SumBillableForInvoiceType(src, InvoiceType.Trips);
                dest.TotalBillableClaimItemsAmount = TripChargeInvoiceRouting.SumBillableForInvoiceType(src, InvoiceType.Claims);
                dest.TotalBillableAmount = dest.TotalBillableTripItemsAmount + dest.TotalBillableClaimItemsAmount;
                dest.TotalFuelAmount = TripChargeTotals.SumForChargeTypes(src.TripCharges, TripChargeType.Fuel);
                dest.TotalBillableFuelAmount = TripChargeTotals.SumBillableForChargeTypes(src.TripCharges, TripChargeType.Fuel);
                dest.TotalBillableTollGatesAmount = TripChargeTotals.SumBillableForChargeTypes(
                    src.TripCharges, TripChargeType.TollGate, TripChargeType.TollGateWeight);
                dest.FuelPricingCalculationOption =
                    ContractFuelExtraFeeHelper.ResolveFuelPricingCalculationOption(src.Contract);
                var lifecycleActor = TripLifecycleActorResolver.Resolve(src);
                dest.LifecycleActor = lifecycleActor;
                dest.LifecycleActorName = lifecycleActor?.ToLocalizedDisplay() ?? string.Empty;
                var driverStatus = OperationalDriverStatusResolver.Resolve(src);
                dest.CurrentDriverStatus = driverStatus;
                dest.CurrentDriverStatusName = driverStatus?.ToLocalizedDisplay() ?? string.Empty;
                dest.IsEditable = TripValidator.IsEditable(src);
                dest.IsHeadVehiclePlateChanged = TripVehiclePlateChangeHelper.IsHeadVehiclePlateChanged(src);
                dest.IsTailVehiclePlateChanged = TripVehiclePlateChangeHelper.IsTailVehiclePlateChanged(src);
                dest.HeadPlateChangedAt = TripVehiclePlateChangeHelper.GetHeadPlateChangedAt(src);
                dest.TailPlateChangedAt = TripVehiclePlateChangeHelper.GetTailPlateChangedAt(src);
            });

        CreateMap<Trip, ContractorTripDto>()
            .ForMember(dest => dest.TripContractDestinations, opt => opt.MapFrom(src => TripContractDestinationHelper.GetActiveAssignments(src)))
            .ForMember(dest => dest.NumberOfDays, opt => opt.MapFrom(src => src.NumberOfDays.HasValue && src.NumberOfDays.Value < 0 ? 0 : src.NumberOfDays))
            .ForMember(dest => dest.VehicleTypeId, opt => opt.MapFrom(src => src.ContractVehicleType != null ? (long?)src.ContractVehicleType.VehicleTypeId : null))
            .ForMember(dest => dest.TripStatusName, opt => opt.MapFrom(src => src.Status.ToLocalizedDisplay()))
            .ForMember(dest => dest.Driver, opt => opt.MapFrom(src => src.Driver))
            .ForMember(dest => dest.Contractor, opt => opt.MapFrom(src => src.Contractor))
            .ForMember(dest => dest.VehicleTypeNameEn, opt => opt.MapFrom(src =>
                src.ContractVehicleType != null && src.ContractVehicleType.VehicleType != null
                    ? src.ContractVehicleType.VehicleType.NameEn
                    : string.Empty))
            .ForMember(dest => dest.VehicleTypeNameAr, opt => opt.MapFrom(src =>
                src.ContractVehicleType != null && src.ContractVehicleType.VehicleType != null
                    ? src.ContractVehicleType.VehicleType.NameAr ?? string.Empty
                    : string.Empty))
            .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle))
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Contract != null && src.Contract.Company != null ? src.Contract.Company : null))
            .ForMember(dest => dest.Lead, opt => opt.MapFrom(src => src.Lead))
            .ForMember(dest => dest.Waypoints, opt => opt.MapFrom(src => src.Waypoints != null && src.Waypoints.Count > 0 ? src.Waypoints.OrderBy(w => w.Sequence) : null))
            .ForMember(dest => dest.CutoffDuration, opt => opt.MapFrom(_ => TripConstants.DefaultCutoffDuration))
            .ForMember(dest => dest.IsPaidToContractor, opt => opt.MapFrom(src => TripContractorPayoutHelper.IsPaidToContractor(src)));

        CreateMap<TripDocument, TripDocumentDto>()
            .ForMember(dest => dest.NameEn, opt => opt.MapFrom(src => src.Document != null ? src.Document.NameEn : string.Empty))
            .ForMember(dest => dest.NameAr, opt => opt.MapFrom(src => src.Document != null ? src.Document.NameAr : null))
            .ForMember(dest => dest.Required, opt => opt.MapFrom(src => src.Document != null && src.Document.Required))
            .ForMember(dest => dest.Paths, opt => opt.MapFrom(src =>
                src.Paths == null || src.Paths.Length == 0
                    ? new List<string>()
                    : src.Paths.Where(p => !string.IsNullOrWhiteSpace(p)).ToList()));

        CreateMap<Trip, TripTrackItemDto>()
            .ForMember(dest => dest.CompanyNameEn, opt => opt.MapFrom(src => src.Contract != null && src.Contract.Company != null && src.Contract.Company.CompanyDetail != null ? src.Contract.Company.CompanyDetail.NameEn : string.Empty))
            .ForMember(dest => dest.CompanyNameAr, opt => opt.MapFrom(src => src.Contract != null && src.Contract.Company != null && src.Contract.Company.CompanyDetail != null ? src.Contract.Company.CompanyDetail.NameAr ?? string.Empty : string.Empty))
            .ForMember(dest => dest.DriverName, opt => opt.MapFrom(src => src.Driver != null ? src.Driver.FullName : string.Empty))
            .ForMember(dest => dest.DriverMobile, opt => opt.MapFrom(src => src.Driver != null ? src.Driver.Phone : string.Empty))
            .ForMember(dest => dest.VehiclePlateNumber, opt => opt.MapFrom(src => TripPlateDisplayHelper.ResolveHeadDisplayPlate(src)))
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.Vehicle != null && src.Vehicle.VehicleType != null ? src.Vehicle.VehicleType.NameEn : string.Empty))
            .ForMember(dest => dest.VehicleColor, opt => opt.MapFrom(src => src.Vehicle != null && src.Vehicle.Color != null ? src.Vehicle.Color.ColorHash : string.Empty))
            .ForMember(dest => dest.Waypoints, opt => opt.MapFrom(src => src.Waypoints != null && src.Waypoints.Count > 0 ? src.Waypoints.OrderBy(w => w.Sequence) : null))
            .ForMember(dest => dest.CurrentLocation, opt => opt.MapFrom(src => src.Driver != null && src.Driver.DriverLocation != null ? src.Driver.DriverLocation : null));

        CreateMap<TripCharge, TripChargeDto>()
            .ForMember(dest => dest.ChargeType, opt => opt.MapFrom(src => (int)src.ChargeType))
            .ForMember(dest => dest.ChargeTypeName, opt => opt.MapFrom(src => src.ChargeType.ToLocalizedDisplay()))
            .ForMember(dest => dest.ExtraFeeTypeId, opt => opt.MapFrom(src => src.ExtraFeeTypeId))
            .ForMember(dest => dest.ExtraFeeType, opt => opt.MapFrom(src =>
                src.ExtraFeeTypeId.HasValue && Enum.IsDefined(typeof(ExtraFeeTypes), src.ExtraFeeTypeId.Value)
                    ? new TripChargeExtraFeeTypeDto
                    {
                        Id = src.ExtraFeeTypeId.Value,
                        NameEn = ExtraFeeTypeDisplayNames.GetNameEn((ExtraFeeTypes)src.ExtraFeeTypeId.Value),
                        NameAr = ExtraFeeTypeDisplayNames.GetNameAr((ExtraFeeTypes)src.ExtraFeeTypeId.Value)
                    }
                    : null));

        CreateMap<TripBookingRequest, TripBookingRequestDto>()
            .ForMember(dest => dest.ContractorName, opt => opt.MapFrom(src => src.Contractor != null ? src.Contractor.FullName : null))
            .ForMember(dest => dest.ContractorPhone, opt => opt.MapFrom(src => src.Contractor != null ? src.Contractor.Phone : null))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToLocalizedDisplay()));

        CreateMap<TripBookingRequest, ContractorTripBookingRequestDto>()
            .ForMember(dest => dest.ContractorName, opt => opt.MapFrom(src => src.Contractor != null ? src.Contractor.FullName : null))
            .ForMember(dest => dest.ContractorPhone, opt => opt.MapFrom(src => src.Contractor != null ? src.Contractor.Phone : null))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToLocalizedDisplay()))
            .ForMember(dest => dest.Trip, opt => opt.MapFrom(src => src.Trip))
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src =>
                src.Trip != null && src.Trip.Contract != null ? src.Trip.Contract.Company : null))
            .ForMember(dest => dest.Lead, opt => opt.MapFrom(src =>
                src.Trip != null ? src.Trip.Lead : null))
            .ForMember(dest => dest.TripContractDestination, opt => opt.MapFrom(src =>
                src.Trip != null ? TripContractDestinationHelper.GetCurrentAssignment(src.Trip) : null))
            .ForMember(dest => dest.VehicleTypeNameEn, opt => opt.MapFrom(src =>
                src.Trip != null && src.Trip.ContractVehicleType != null && src.Trip.ContractVehicleType.VehicleType != null
                    ? src.Trip.ContractVehicleType.VehicleType.NameEn
                    : string.Empty))
            .ForMember(dest => dest.VehicleTypeNameAr, opt => opt.MapFrom(src =>
                src.Trip != null && src.Trip.ContractVehicleType != null && src.Trip.ContractVehicleType.VehicleType != null
                    ? src.Trip.ContractVehicleType.VehicleType.NameAr ?? string.Empty
                    : string.Empty))
            .ForMember(dest => dest.ContractorPrice, opt => opt.MapFrom(src =>
                src.Trip != null ? src.Trip.ContractorPrice : null))
            .ForMember(dest => dest.ContactPersonName, opt => opt.MapFrom(src =>
                src.Trip != null ? src.Trip.ContactPersonName : null))
            .ForMember(dest => dest.ContactPersonMobile, opt => opt.MapFrom(src =>
                src.Trip != null ? src.Trip.ContactPersonMobile : null))
            .ForMember(dest => dest.Waypoints, opt => opt.MapFrom(src =>
                src.Trip != null && src.Trip.Waypoints != null && src.Trip.Waypoints.Count > 0
                    ? src.Trip.Waypoints.OrderBy(w => w.Sequence)
                    : null));

        CreateMap<Contract, LoadlyAggregator.Domain.Common.DTOs.Relations.ContractRelationDto>();

        CreateMap<VehicleType, LoadlyAggregator.Domain.Common.DTOs.Relations.VehicleTypeRelationDto>();
        CreateMap<ContractVehicleType, LoadlyAggregator.Domain.Common.DTOs.Relations.ContractVehicleTypeRelationDto>()
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType));

        CreateMap<TripWaypoint, TripWaypointDto>();

        CreateMap<DriverLocation, TripLocationDto>();


        CreateMap<Recording, TripRecordingDto>()
            .ForMember(dest => dest.EventType, opt => opt.MapFrom(src => (int)src.RecordableEventType))
            .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.RecordableEventType.ToString()))
            .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data != null ? src.Data.RootElement.Clone() : (System.Text.Json.JsonElement?)null));

        // Contract DTO to Entity (create/update)
        CreateMap<CreateContractDto, Contract>()
            .ForMember(dest => dest.CargoCommission, opt => opt.MapFrom(src => src.CargoCommission ?? 0m))
            .ForMember(dest => dest.ReferenceNumber, opt => opt.Ignore())
            .ForMember(dest => dest.ReferenceSequence, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentApprovalStage, opt => opt.Ignore())
            .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Approvals, opt => opt.Ignore())
            .ForMember(dest => dest.StatusHistories, opt => opt.Ignore())
            .ForMember(dest => dest.Company, opt => opt.Ignore())
            .ForMember(dest => dest.SalesRep, opt => opt.Ignore())
            .ForMember(dest => dest.RenewedFromContract, opt => opt.Ignore())
            .ForMember(dest => dest.DeactivationDate, opt => opt.Ignore());

        CreateMap<UpdateContractDto, Contract>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CargoCommission, opt => opt.Ignore())
            .ForMember(dest => dest.MonthlyWorkingDays, opt => opt.Ignore())
            .ForMember(dest => dest.ReferenceNumber, opt => opt.Ignore())
            .ForMember(dest => dest.ReferenceSequence, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentApprovalStage, opt => opt.Ignore())
            .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Approvals, opt => opt.Ignore())
            .ForMember(dest => dest.StatusHistories, opt => opt.Ignore())
            .ForMember(dest => dest.Company, opt => opt.Ignore())
            .ForMember(dest => dest.SalesRep, opt => opt.Ignore())
            .ForMember(dest => dest.RenewedFromContract, opt => opt.Ignore())
            .ForMember(dest => dest.DeactivationDate, opt => opt.Ignore())
            .ForMember(dest => dest.RenewedFromContractId, opt => opt.Ignore());

        CreateMap<LoadlyAggregator.Domain.Admin.DTOs.Contracts.ContractVehicleTypeDto, ContractVehicleType>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ContractId, opt => opt.Ignore())
            .ForMember(dest => dest.TripsPerDay, opt => opt.Ignore())
            .ForMember(dest => dest.DailyValue, opt => opt.Ignore())
            .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Contract, opt => opt.Ignore())
            .ForMember(dest => dest.VehicleType, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

        CreateMap<LoadlyAggregator.Domain.Admin.DTOs.Contracts.CreateContractVehicleTypeDto, ContractVehicleType>()
            .ForMember(dest => dest.ContractId, opt => opt.Ignore())
            .ForMember(dest => dest.TripsPerDay, opt => opt.Ignore())
            .ForMember(dest => dest.DailyValue, opt => opt.Ignore())
            .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Contract, opt => opt.Ignore())
            .ForMember(dest => dest.VehicleType, opt => opt.Ignore());

        CreateMap<DocumentItemDto, ContractDocument>()
            .ForMember(dest => dest.ContractId, opt => opt.Ignore())
            .ForMember(dest => dest.Path, opt => opt.Ignore())
            .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Contract, opt => opt.Ignore())
            .ForMember(dest => dest.Document, opt => opt.Ignore());

        CreateMap<ExtraFeeItemDto, ContractExtraFee>()
            .ForMember(dest => dest.ExtraFeeTypeId, opt => opt.MapFrom(src => (int)src.ExtraFeeTypeId))
            .ForMember(dest => dest.ContractId, opt => opt.Ignore())
            .ForMember(dest => dest.AddedBy, opt => opt.Ignore())
            .ForMember(dest => dest.Contract, opt => opt.Ignore());

        // Role Entity to DTO mappings
        CreateMap<Role, RoleDto>()
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.RolePermissions.Select(rhp => rhp.Permission)));

        CreateMap<Permission, RolePermissionDto>();

        CreateMap<Role, CompanyRoleDto>()
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.CompanyId ?? 0))
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.RolePermissions.Select(rhp => rhp.Permission)));
        CreateMap<Permission, CompanyRolePermissionDto>();

        // Invoice Entity to DTO mappings
        CreateMap<TripCharge, TripChargeRelationDto>()
            .ForMember(dest => dest.ChargeType, opt => opt.MapFrom(src => (int)src.ChargeType))
            .ForMember(dest => dest.ChargeTypeName, opt => opt.MapFrom(src => src.ChargeType.ToLocalizedDisplay()));

        CreateMap<Trip, TripRelationDto>()
            .ForMember(dest => dest.Driver, opt => opt.MapFrom(src => src.Driver))
            .ForMember(dest => dest.TripCharges, opt => opt.MapFrom(src => src.TripCharges ?? new List<TripCharge>()));

        CreateMap<Tax, InvoiceTaxRelationDto>();
        CreateMap<Discount, InvoiceDiscountRelationDto>();

        CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.SlotChargesSubTotal, opt => opt.MapFrom(src => src.SlotChargesSubTotal))
            .ForMember(dest => dest.SlotCharges, opt => opt.Ignore())
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Contract != null ? src.Contract.CompanyId : 0))
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Contract != null ? src.Contract.Company : null))
            .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract))
            .ForMember(dest => dest.Lead, opt => opt.MapFrom(src => src.Lead))
            .ForMember(dest => dest.TripCount, opt => opt.MapFrom(src =>
                src.InvoiceType == InvoiceType.Claims
                    ? (src.ClaimInvoiceTrips != null ? src.ClaimInvoiceTrips.Count : 0)
                    : (src.Trips != null ? src.Trips.Count : 0)))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.ContractReferenceNumber, opt => opt.MapFrom(src =>
                src.Contract != null
                    ? src.Contract.ReferenceNumber
                    : (src.InvoiceType == InvoiceType.Claims
                        ? (src.ClaimInvoiceTrips != null && src.ClaimInvoiceTrips.Any() && src.ClaimInvoiceTrips.First().Contract != null ? src.ClaimInvoiceTrips.First().Contract.ReferenceNumber : null)
                        : (src.Trips != null && src.Trips.Any() && src.Trips.First().Contract != null ? src.Trips.First().Contract.ReferenceNumber : null))))
            .ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.TaxId.HasValue
                ? new InvoiceTaxRelationDto
                {
                    Id = src.TaxId.Value,
                    NameEn = src.Tax != null ? src.Tax.NameEn : string.Empty,
                    NameAr = src.Tax != null ? src.Tax.NameAr : null,
                    Value = src.TaxValue ?? 0m,
                    Type = src.TaxType ?? TaxType.Fixed
                }
                : null))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.DiscountId.HasValue
                ? new InvoiceDiscountRelationDto
                {
                    Id = src.DiscountId.Value,
                    NameEn = src.Discount != null ? src.Discount.NameEn : string.Empty,
                    NameAr = src.Discount != null ? src.Discount.NameAr : null,
                    Value = src.DiscountValue ?? 0m,
                    Type = src.DiscountType ?? DiscountType.Fixed
                }
                : null))
            .ForMember(dest => dest.Trips, opt => opt.MapFrom(src =>
                src.InvoiceType == InvoiceType.Claims
                    ? (src.ClaimInvoiceTrips ?? new List<Trip>())
                    : (src.Trips ?? new List<Trip>())))
            .ForMember(dest => dest.InvoicePayments, opt => opt.MapFrom(src => src.Payments ?? new List<InvoicePayment>()));

        CreateMap<InvoicePayment, InvoicePaymentRelationDto>()
            .ForMember(dest => dest.BankTransfer, opt => opt.Ignore());

        CreateMap<BankTransfer, BankTransferRelationDto>()
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src =>
                src.PayerType == Shared.Constants.BankTransferPayerTypes.Company ? src.Company : null))
            .ForMember(dest => dest.Lead, opt => opt.MapFrom(src =>
                src.PayerType == Shared.Constants.BankTransferPayerTypes.Lead ? src.Lead : null));

        CreateMap<BankTransfer, BankTransferDto>()
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src =>
                src.PayerType == Shared.Constants.BankTransferPayerTypes.Company ? src.Company : null))
            .ForMember(dest => dest.Lead, opt => opt.MapFrom(src =>
                src.PayerType == Shared.Constants.BankTransferPayerTypes.Lead ? src.Lead : null))
            .ForMember(dest => dest.InvoicePayments, opt => opt.MapFrom(src => src.InvoicePayments))
            .AfterMap((src, dest) =>
            {
                dest.InvoicePaymentCount = src.InvoicePayments.Count;
                dest.AllocatedAmount = src.InvoicePayments.Sum(p => p.Amount);
                dest.RemainingAmount = src.Amount - dest.AllocatedAmount;
                (dest.DisplayLabelEn, dest.DisplayLabelAr) = BankTransferDisplayLabel.Format(dest);
            });

        CreateMap<Trip, ContractorPayoutTripLineDto>()
            .ForMember(dest => dest.TripId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(src => TripReferenceNumber.Format(src.Id)));

        CreateMap<ContractorPayout, ContractorPayoutDto>()
            .ForMember(dest => dest.Trips, opt => opt.MapFrom(src => src.Trips.OrderBy(t => t.Id)))
            .AfterMap((src, dest) =>
            {
                var totalTripCost = src.Trips.Sum(t => t.TotalTripCost ?? 0m);
                var totalContractorPrice = src.Trips.Sum(t => t.ContractorPrice ?? 0m);
                dest.TripCount = src.Trips.Count;
                dest.TotalTripCostAmount = totalTripCost;
                dest.TotalContractorPriceAmount = totalContractorPrice;
                dest.LoadlyCommissionAmount = totalTripCost - totalContractorPrice;
            });

        CreateMap<TripCharge, TripChargeRelationDto>()
            .ForMember(dest => dest.ChargeType, opt => opt.MapFrom(src => (int)src.ChargeType))
            .ForMember(dest => dest.ChargeTypeName, opt => opt.MapFrom(src => src.ChargeType.ToLocalizedDisplay()));

        CreateMap<Trip, TripRelationDto>()
            .ForMember(dest => dest.Driver, opt => opt.MapFrom(src => src.Driver))
            .ForMember(dest => dest.TripCharges, opt => opt.MapFrom(src => src.TripCharges ?? new List<TripCharge>()));

        CreateMap<Tax, LoadlyAggregator.Domain.Company.DTOs.Invoices.InvoiceTaxRelationDto>();
        CreateMap<Discount, LoadlyAggregator.Domain.Company.DTOs.Invoices.InvoiceDiscountRelationDto>();

        CreateMap<Invoice, LoadlyAggregator.Domain.Company.DTOs.Invoices.InvoiceDto>()
            .ForMember(dest => dest.CompanyId, opt => opt.MapFrom(src => src.Contract != null ? src.Contract.CompanyId : 0))
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Contract != null ? src.Contract.Company : null))
            .ForMember(dest => dest.Contract, opt => opt.MapFrom(src => src.Contract))
            .ForMember(dest => dest.Lead, opt => opt.MapFrom(src => src.Lead))
            .ForMember(dest => dest.TripCount, opt => opt.MapFrom(src => src.InvoiceType == InvoiceType.Claims ? (src.ClaimInvoiceTrips != null ? src.ClaimInvoiceTrips.Count : 0) : (src.Trips != null ? src.Trips.Count : 0)))
            .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt))
            .ForMember(dest => dest.ContractReferenceNumber, opt => opt.MapFrom(src => src.Contract != null ? src.Contract.ReferenceNumber : (src.InvoiceType == InvoiceType.Claims ? (src.ClaimInvoiceTrips != null && src.ClaimInvoiceTrips.Any() && src.ClaimInvoiceTrips.First().Contract != null ? src.ClaimInvoiceTrips.First().Contract.ReferenceNumber : null) : (src.Trips != null && src.Trips.Any() && src.Trips.First().Contract != null ? src.Trips.First().Contract.ReferenceNumber : null))))
            .ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.TaxId.HasValue
                ? new LoadlyAggregator.Domain.Company.DTOs.Invoices.InvoiceTaxRelationDto
                {
                    Id = src.TaxId.Value,
                    NameEn = src.Tax != null ? src.Tax.NameEn : string.Empty,
                    NameAr = src.Tax != null ? src.Tax.NameAr : null,
                    Value = src.TaxValue ?? 0m,
                    Type = src.TaxType ?? TaxType.Fixed
                }
                : null))
            .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.DiscountId.HasValue
                ? new LoadlyAggregator.Domain.Company.DTOs.Invoices.InvoiceDiscountRelationDto
                {
                    Id = src.DiscountId.Value,
                    NameEn = src.Discount != null ? src.Discount.NameEn : string.Empty,
                    NameAr = src.Discount != null ? src.Discount.NameAr : null,
                    Value = src.DiscountValue ?? 0m,
                    Type = src.DiscountType ?? DiscountType.Fixed
                }
                : null))
            .ForMember(dest => dest.Trips, opt => opt.MapFrom(src => src.InvoiceType == InvoiceType.Claims ? (src.ClaimInvoiceTrips ?? new List<Trip>()) : (src.Trips ?? new List<Trip>())))
            .ForMember(dest => dest.InvoicePayments, opt => opt.MapFrom(src => src.Payments ?? new List<InvoicePayment>()));

    }
}
