using AutoMapper;
using LoadlyAggregator.API.Resources.Admin.Contracts;
using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;
using LoadlyAggregator.Domain.Common.DTOs.Documents;
using LoadlyAggregator.Domain.Common.DTOs.Relations;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.API.Mappings;

public class ContractResourceMappingProfile : Profile
{
    public ContractResourceMappingProfile()
    {
        CreateMap<ContractDto, ContractResource>()
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => (int)src.PaymentMethod))
            .ForMember(dest => dest.BillingCycle, opt => opt.MapFrom(src => (int)src.BillingCycle))
            .ForMember(dest => dest.ConsumptionType, opt => opt.MapFrom(src => (int)src.ConsumptionType))
            .ForMember(dest => dest.ContractType, opt => opt.MapFrom(src => (int)src.ContractType))
            .ForMember(dest => dest.KmOverageMode, opt => opt.MapFrom(src =>
                src.KmOverageMode.HasValue ? (int?)src.KmOverageMode.Value : null))
            .ForMember(dest => dest.KmOverageModeName, opt => opt.MapFrom(src =>
                src.KmOverageMode.HasValue ? src.KmOverageMode.Value.ToLocalizedDisplay() : null))
            .ForMember(dest => dest.SalesRep, opt => opt.MapFrom(src => src.SalesRep))
            .ForMember(dest => dest.Company, opt => opt.MapFrom(src => src.Company))
            .ForMember(dest => dest.ExtraFees, opt => opt.MapFrom(src =>
                src.ExtraFees.Where(extraFee => extraFee.ExtraFeeTypeId != (long)ExtraFeeTypes.Fuel)));

        CreateMap<CompanyRelationDto, CompanyRelationResource>();
        CreateMap<ContractRelationDto, ContractRelationResource>()
            .ForMember(dest => dest.ContractType, opt => opt.MapFrom(src => (int)src.ContractType))
            .ForMember(dest => dest.ContractTypeName, opt => opt.MapFrom(src => src.ContractType.ToLocalizedDisplay()))
            .ForMember(dest => dest.ConsumptionType, opt => opt.MapFrom(src => (int)src.ConsumptionType));
        CreateMap<AdminRelationDto, AdminRelationResource>();

        CreateMap<ContractApprovalDto, ContractApprovalRelationResource>()
            .ForMember(dest => dest.Stage, opt => opt.MapFrom(src => (int)src.Stage))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<ContractStatusHistoryDto, ContractStatusHistoryRelationResource>();

        CreateMap<ContractVehicleTypeDto, ContractVehicleTypeRelationResource>()
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType));

        CreateMap<ContractVehicleTypeRelationDto, ContractVehicleTypeRelationResource>()
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType));

        CreateMap<LoadlyAggregator.Domain.Common.DTOs.Relations.VehicleRelationDto, VehicleRelationResource>();
        CreateMap<LoadlyAggregator.Domain.Common.DTOs.Relations.DriverRelationDto, DriverRelationResource>();
        CreateMap<LeadRelationDto, LeadRelationResource>();
        CreateMap<LoadlyAggregator.Domain.Common.DTOs.Relations.CoupledVehicleRelationDto, CoupledVehicleRelationResource>();

        CreateMap<ContractDocumentDto, ContractDocumentRelationResource>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<ContractExtraFeeDto, ContractExtraFeeRelationResource>();

        CreateMap<LoadlyAggregator.Domain.Common.DTOs.Relations.VehicleTypeRelationDto, VehicleTypeRelationResource>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameAr))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => (int)src.Category))
            .ForMember(dest => dest.VehicleUnitType, opt => opt.MapFrom(src => src.VehicleUnitType.HasValue ? (int?)src.VehicleUnitType.Value : null));
        CreateMap<DocumentDto, DocumentRelationResource>();
        CreateMap<ExtraFeeTypeDto, ExtraFeeTypeRelationResource>();
        CreateMap<DestinationRelationDto, DestinationRelationResource>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.HasValue ? (int?)src.Status.Value : null))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.HasValue ? src.Status.Value.ToLocalizedDisplay() : string.Empty));
        CreateMap<ContractDestinationDto, ContractDestinationResource>()
            .IncludeBase<DestinationRelationDto, DestinationRelationResource>();
        CreateMap<ContractSlotDto, ContractSlotRelationResource>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToLocalizedDisplay()))
            .ForMember(dest => dest.VehicleType, opt => opt.MapFrom(src => src.VehicleType))
            .ForMember(dest => dest.Vehicle, opt => opt.MapFrom(src => src.Vehicle));
        CreateMap<WaypointRelationDto, WaypointRelationResource>();
    }
}
