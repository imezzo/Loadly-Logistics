using FluentValidation;
using LoadlyAggregator.API.Requests.Admin.Settings.Sectors;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Settings;
using LoadlyAggregator.Domain.Common.DTOs.Common;

namespace LoadlyAggregator.API.Requests.Admin.Settings.Sectors;

public class ListSectorsRequest : BasePagedRequest
{
    public const string Permission = "view_sector";

    public ListSectorsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search
    };
}

public class ListSectorsRequestValidator : AbstractValidator<ListSectorsRequest>
{
    public ListSectorsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();
    }
}
