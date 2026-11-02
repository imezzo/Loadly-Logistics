using AutoMapper;
using LoadlyAggregator.API.Resources.Admin.Contractors;
using LoadlyAggregator.API.Resources.Admin.Dashboard;
using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Domain.Admin.DTOs.Dashboard;

namespace LoadlyAggregator.API.Mappings;

public class ContractorDashboardStatisticsResourceMappingProfile : Profile
{
    public ContractorDashboardStatisticsResourceMappingProfile()
    {
        CreateMap<PeriodComparisonDto, PeriodComparisonResource>()
            .ForMember(dest => dest.Direction, opt => opt.MapFrom(src => src.Direction.ToString().ToLowerInvariant()));

        CreateMap<ContractorDashboardTripsMetricDto, ContractorDashboardTripsMetricResource>();
        CreateMap<ContractorDashboardLoadlyProfileMarginDto, ContractorDashboardLoadlyProfileMarginResource>();
        CreateMap<ContractorDashboardStatisticsDto, ContractorDashboardStatisticsResource>();
    }
}
