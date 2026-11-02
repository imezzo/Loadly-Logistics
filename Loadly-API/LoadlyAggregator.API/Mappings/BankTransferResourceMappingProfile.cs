using AutoMapper;
using LoadlyAggregator.API.Resources.Admin.Invoices;
using LoadlyAggregator.Domain.Admin.DTOs.Invoices;

namespace LoadlyAggregator.API.Mappings;

public class BankTransferResourceMappingProfile : Profile
{
    public BankTransferResourceMappingProfile()
    {
        CreateMap<BankTransferDto, BankTransferResource>()
            .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => (int)src.PaymentMethod));
    }
}
