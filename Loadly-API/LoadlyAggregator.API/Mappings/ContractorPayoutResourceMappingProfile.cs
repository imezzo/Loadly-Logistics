using AutoMapper;
using LoadlyAggregator.API.Resources.Admin.ContractorPayouts;
using LoadlyAggregator.Domain.Admin.DTOs.ContractorPayouts;

namespace LoadlyAggregator.API.Mappings;

public class ContractorPayoutResourceMappingProfile : Profile
{
    public ContractorPayoutResourceMappingProfile()
    {
        CreateMap<ContractorPayoutTripLineDto, ContractorPayoutTripLineResource>();
        CreateMap<ContractorPayoutDto, ContractorPayoutResource>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => (int)src.Status))
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}
