using AutoMapper;
using LoadlyAggregator.API.Resources.Admin.Settings;
using LoadlyAggregator.API.Resources.Admin.Vehicles;
using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Admin.DTOs.Vehicles;

namespace LoadlyAggregator.API.Mappings;

public class VehicleResourceMappingProfile : Profile
{
    public VehicleResourceMappingProfile()
    {
        CreateMap<VehicleDto, VehicleResource>();

        CreateMap<VehiclePlateChangeHistoryDto, VehiclePlateChangeHistoryResource>();

        CreateMap<Domain.Common.DTOs.Relations.VehicleFacilityRelationDto, VehicleFacilityRelationResource>();
        CreateMap<Domain.Common.DTOs.Relations.CompanyRelationDto, CompanyRelationResource>();
        CreateMap<Domain.Common.DTOs.Relations.VehicleFacilityVisitOnVehicleRelationDto, VehicleFacilityVisitOnVehicleRelationResource>()
            .ForMember(dest => dest.Event, opt => opt.MapFrom(src => (int)src.Event))
            .ForMember(dest => dest.Context, opt => opt.MapFrom(src => (int)src.Context))
            .ForMember(dest => dest.EventName, opt => opt.MapFrom(src => src.EventName))
            .ForMember(dest => dest.ContextName, opt => opt.MapFrom(src => src.ContextName));
        CreateMap<Domain.Common.DTOs.Relations.AdminRelationDto, AdminRelationResource>();

        CreateMap<Domain.Common.DTOs.Relations.SectorRelationDto, SectorRelationResource>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameEn));

        CreateMap<FuelTypeDto, FuelTypeResource>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameEn));

        CreateMap<VehicleInsuranceDto, VehicleInsuranceResource>();

        CreateMap<VehicleBrandModelDto, VehicleBrandModelResource>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameEn))
            .ForMember(dest => dest.VehicleBrand, opt => opt.Ignore());

        CreateMap<ColorDto, ColorResource>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameEn));

        CreateMap<Domain.Common.DTOs.Relations.VehicleTypeRelationDto, VehicleTypeRelationResource>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameAr))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => (int)src.Category))
            .ForMember(dest => dest.VehicleUnitType, opt => opt.MapFrom(src => src.VehicleUnitType.HasValue ? (int?)src.VehicleUnitType.Value : null));

        CreateMap<Domain.Common.DTOs.Relations.CoupledVehicleRelationDto, CoupledVehicleRelationResource>();

        CreateMap<VehicleTypeDto, VehicleTypeResource>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.NameEn))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => (int)src.Category))
            .ForMember(dest => dest.VehicleUnitType, opt => opt.MapFrom(src => src.VehicleUnitType.HasValue ? (int?)src.VehicleUnitType.Value : null))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));

        CreateMap<VehicleCapacityUnitDto, VehicleCapacityUnitResource>()
            .ForMember(dest => dest.CapacityType, opt => opt.MapFrom(src => src.CapacityTypeEn));

        CreateMap<VehicleDetailDto, VehicleDetailResource>()
            .ForMember(dest => dest.LoadingMechanismType, opt => opt.MapFrom(src => src.LoadingMechanismType.HasValue ? (int?)src.LoadingMechanismType.Value : null));

        CreateMap<VehicleDocumentDataDto, VehicleDocumentRelationResource>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status));
    }
}
