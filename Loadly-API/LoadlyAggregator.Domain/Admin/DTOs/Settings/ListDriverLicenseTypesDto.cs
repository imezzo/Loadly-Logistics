using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.Domain.Admin.DTOs.Settings;

public class ListDriverLicenseTypesDto : BasePagedRequest
{
    public UserStatus? Status { get; set; }
}
