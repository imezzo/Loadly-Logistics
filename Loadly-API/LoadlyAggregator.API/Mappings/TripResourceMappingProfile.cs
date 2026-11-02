using AutoMapper;
using LoadlyAggregator.API.Resources.Collector.Trips;
using LoadlyAggregator.API.Resources.Admin.Trips;
using LoadlyAggregator.API.Resources.Company.Trips;
using LoadlyAggregator.API.Resources.Contractor.Trips;
using LoadlyAggregator.API.Resources.Driver.Trips;
using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Contractor.DTOs.Trips;
using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Shared.Api;
using LoadlyAggregator.Shared.Utilities;

namespace LoadlyAggregator.API.Mappings;

public class TripResourceMappingProfile : Profile
{
    public TripResourceMappingProfile()
    {
        CreateMap<ContractorRelationDto, ContractorRelationResource>();
        CreateMap<LeadRelationDto, LeadRelationResource>();
        CreateMap<AdminRelationDto, AdminRelationResource>();

        CreateMap<TripVehicleAssignmentSnapshotDto, TripVehicleAssignmentSnapshotResource>();
        CreateMap<TripContractDestinationDto, TripContractDestinationResource>();

        CreateMap<TripDto, TripResource>()
            .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(src => TripReferenceNumber.Format(src.Id)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.CurrentDriverStatus, opt => opt.MapFrom(src =>
                src.CurrentDriverStatus.HasValue ? (int?)src.CurrentDriverStatus.Value : null))
            .ForMember(dest => dest.CurrentDriverStatusName, opt => opt.MapFrom(src => src.CurrentDriverStatusName))
            .ForMember(dest => dest.TripDocumentStatus, opt => opt.MapFrom(src => (int)src.TripDocumentStatus))
            .ForMember(dest => dest.TripDocumentStatusName, opt => opt.MapFrom(src => src.TripDocumentStatus.ToLocalizedDisplay()))
            .ForMember(dest => dest.CustomsStatus, opt => opt.MapFrom(src => (int)src.CustomsStatus))
            .ForMember(dest => dest.CustomsStatusName, opt => opt.MapFrom(src => src.CustomsStatus.ToLocalizedDisplay()))
            .ForMember(dest => dest.VehicleTypeId, opt => opt.MapFrom(src => src.VehicleTypeId))
            .ForMember(dest => dest.NumberOfDays, opt => opt.MapFrom(src =>
                src.NumberOfDays.HasValue && src.NumberOfDays.Value < 0 ? 0 : src.NumberOfDays))
            .ForMember(dest => dest.OverNightCostBearer, opt => opt.MapFrom(src =>
                src.OverNightCostBearer.HasValue ? (int?)src.OverNightCostBearer.Value : null))
            .ForMember(dest => dest.OverNightCostBearerName, opt => opt.MapFrom(src =>
                src.OverNightCostBearer.HasValue ? src.OverNightCostBearer.Value.ToLocalizedDisplay() : string.Empty))
            .ForMember(dest => dest.DispatchingMode, opt => opt.MapFrom(src =>
                src.DispatchingMode.HasValue ? (int?)src.DispatchingMode.Value : null))
            .ForMember(dest => dest.DispatchingModeName, opt => opt.MapFrom(src =>
                src.DispatchingMode.HasValue ? src.DispatchingMode.Value.ToLocalizedDisplay() : string.Empty))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src =>
                src.TripType.HasValue ? (int?)src.TripType.Value : null))
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.TripTypeName))
            .ForMember(dest => dest.TripCostWithoutAddon, opt => opt.MapFrom(src =>
                src.TotalTripCost.HasValue
                    ? src.TotalTripCost.Value - (src.TotalAddonsCost ?? 0m)
                    : (decimal?)null))
            .ForMember(dest => dest.InvoicingStatus, opt => opt.MapFrom(src => (int)src.InvoicingStatus))
            .ForMember(dest => dest.InvoicingStatusName, opt => opt.MapFrom(src => src.InvoicingStatusName))
            .ForMember(dest => dest.LifecycleActor, opt => opt.MapFrom(src =>
                src.LifecycleActor.HasValue ? (int?)src.LifecycleActor.Value : null))
            .ForMember(dest => dest.LifecycleActorName, opt => opt.MapFrom(src => src.LifecycleActorName))
            .ForMember(dest => dest.FuelPricingCalculationOption, opt => opt.MapFrom(src =>
                src.FuelPricingCalculationOption.HasValue ? (int?)src.FuelPricingCalculationOption.Value : null))
            .ForMember(dest => dest.FuelPricingCalculationOptionName, opt => opt.MapFrom(src =>
                src.FuelPricingCalculationOption.HasValue
                    ? src.FuelPricingCalculationOption.Value.ToLocalizedDisplay()
                    : string.Empty));

        CreateMap<TripDto, CompanyTripResource>()
            .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(src => TripReferenceNumber.Format(src.Id)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.CurrentDriverStatus, opt => opt.MapFrom(src =>
                src.CurrentDriverStatus.HasValue ? (int?)src.CurrentDriverStatus.Value : null))
            .ForMember(dest => dest.CurrentDriverStatusName, opt => opt.MapFrom(src => src.CurrentDriverStatusName))
            .ForMember(dest => dest.TripDocumentStatus, opt => opt.MapFrom(src => (int)src.TripDocumentStatus))
            .ForMember(dest => dest.TripDocumentStatusName, opt => opt.MapFrom(src => src.TripDocumentStatus.ToLocalizedDisplay()))
            .ForMember(dest => dest.VehicleTypeId, opt => opt.MapFrom(src => src.VehicleTypeId))
            .ForMember(dest => dest.NumberOfDays, opt => opt.MapFrom(src =>
                src.NumberOfDays.HasValue && src.NumberOfDays.Value < 0 ? 0 : src.NumberOfDays))
            .ForMember(dest => dest.TripCostWithoutAddon, opt => opt.MapFrom(src =>
                src.TotalTripCost.HasValue
                    ? src.TotalTripCost.Value - (src.TotalAddonsCost ?? 0m)
                    : (decimal?)null))
            .ForMember(dest => dest.LifecycleActor, opt => opt.MapFrom(src =>
                src.LifecycleActor.HasValue ? (int?)src.LifecycleActor.Value : null))
            .ForMember(dest => dest.LifecycleActorName, opt => opt.MapFrom(src => src.LifecycleActorName));

        CreateMap<ContractorTripDto, ContractorTripResource>()
            .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(src => TripReferenceNumber.Format(src.Id)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.VehicleTypeId, opt => opt.MapFrom(src => src.VehicleTypeId))
            .ForMember(dest => dest.NumberOfDays, opt => opt.MapFrom(src =>
                src.NumberOfDays.HasValue && src.NumberOfDays.Value < 0 ? 0 : src.NumberOfDays));

        CreateMap<TripDto, DriverTripResource>()
            .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(src => TripReferenceNumber.Format(src.Id)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.CurrentDriverStatus, opt => opt.MapFrom(src =>
                src.CurrentDriverStatus.HasValue ? (int?)src.CurrentDriverStatus.Value : null))
            .ForMember(dest => dest.CurrentDriverStatusName, opt => opt.MapFrom(src => src.CurrentDriverStatusName))
            .ForMember(dest => dest.CustomsStatus, opt => opt.MapFrom(src => (int)src.CustomsStatus))
            .ForMember(dest => dest.CustomsStatusName, opt => opt.MapFrom(src => src.CustomsStatus.ToLocalizedDisplay()))
            .ForMember(dest => dest.CompletionTime, opt => opt.MapFrom(src => src.CompletionTime))
            .ForMember(dest => dest.LifecycleActor, opt => opt.MapFrom(src =>
                src.LifecycleActor.HasValue ? (int?)src.LifecycleActor.Value : null))
            .ForMember(dest => dest.LifecycleActorName, opt => opt.MapFrom(src => src.LifecycleActorName));

        CreateMap<TripDto, CollectorTripResource>()
            .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(src => TripReferenceNumber.Format(src.Id)))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.TripDocumentStatus, opt => opt.MapFrom(src => (int)src.TripDocumentStatus))
            .ForMember(dest => dest.TripDocumentStatusName, opt => opt.MapFrom(src => src.TripDocumentStatus.ToLocalizedDisplay()))
            .ForMember(dest => dest.CustomsStatus, opt => opt.MapFrom(src => (int)src.CustomsStatus))
            .ForMember(dest => dest.LifecycleActor, opt => opt.MapFrom(src =>
                src.LifecycleActor.HasValue ? (int?)src.LifecycleActor.Value : null))
            .ForMember(dest => dest.LifecycleActorName, opt => opt.MapFrom(src => src.LifecycleActorName));

        CreateMap<TripDriverDocumentDto, TripDriverDocumentResource>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
            .ForMember(dest => dest.TypeName, opt => opt.MapFrom(src => src.Type.ToLocalizedDisplay()));

        CreateMap<TripBookingRequestDto, TripBookingRequestResource>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));
        CreateMap<ContractorTripBookingRequestDto, ContractorTripBookingRequestResource>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<TripDocumentDto, TripDocumentResource>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.HasValue ? (int?)src.Status.Value : null))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.HasValue ? src.Status.Value.ToString() : null));

        CreateMap<TripWaypointDto, TripWaypointResource>()
            .ForMember(dest => dest.StopType, opt => opt.MapFrom(src => (int)src.StopType))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<TripRecordingDto, TripRecordingResource>();
        CreateMap<TripChargeExtraFeeTypeDto, TripChargeExtraFeeTypeResource>();
        CreateMap<TripChargeDto, TripChargeResource>();
        CreateMap<TripLocationDto, TripLocationResource>();
    }
}
