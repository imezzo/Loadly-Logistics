using AutoMapper;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;

namespace LoadlyAggregator.API.Mappings;

public class ContractExtraFeeToExtraFeeTypeDtoResolver : IValueResolver<ContractExtraFee, ContractExtraFeeDto, ExtraFeeTypeDto?>
{
    public ExtraFeeTypeDto? Resolve(ContractExtraFee source, ContractExtraFeeDto destination, ExtraFeeTypeDto? destMember, ResolutionContext context)
    {
        var e = (ExtraFeeTypes)source.ExtraFeeTypeId;
        var nameEn = ExtraFeeTypeDisplayNames.GetNameEn(e);
        var nameAr = ExtraFeeTypeDisplayNames.GetNameAr(e);
        return new ExtraFeeTypeDto
        {
            Id = (long)(int)e,
            Code = e.ToString(),
            NameEn = nameEn,
            NameAr = nameAr,
            Status = 1
        };
    }
}
