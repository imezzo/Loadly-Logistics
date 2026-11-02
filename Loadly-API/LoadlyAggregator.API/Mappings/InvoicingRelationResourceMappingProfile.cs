using AutoMapper;
using LoadlyAggregator.API.Resources.RelationResources;
using LoadlyAggregator.Domain.Common.DTOs.Relations;

namespace LoadlyAggregator.API.Mappings;

public class InvoicingRelationResourceMappingProfile : Profile
{
    public InvoicingRelationResourceMappingProfile()
    {
        CreateMap<InvoiceRelationDto, InvoiceRelationResource>()
            .ForMember(dest => dest.InvoiceType, opt => opt.MapFrom(src => (int)src.InvoiceType))
            .ForMember(dest => dest.InvoiceTypeName, opt => opt.MapFrom(src => src.InvoiceType.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<InvoicePaymentRelationDto, InvoicePaymentRelationResource>();

        CreateMap<BankTransferRelationDto, BankTransferRelationResource>()
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => (int)src.PaymentMethod));

        CreateMap<TripRelationDto, TripRelationResource>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.TripCostWithoutAddon, opt => opt.MapFrom(src =>
                src.TotalTripCost.HasValue
                    ? src.TotalTripCost.Value - (src.TotalAddonsCost ?? 0m)
                    : (decimal?)null));

        CreateMap<TripChargeRelationDto, TripChargeRelationResource>();
    }
}
