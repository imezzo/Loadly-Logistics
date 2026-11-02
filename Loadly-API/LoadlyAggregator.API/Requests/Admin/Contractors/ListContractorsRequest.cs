using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contractors;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Admin.Contractors;

public class ListContractorsRequest : BasePagedRequest
{
    public const string Permission = "view_contractor";

    public ContractorStatus? ContractorStatus { get; set; }
    public UserStatus? Status { get; set; }
    public DateTime? TripStartDate { get; set; }
    public DateTime? TripEndDate { get; set; }

    public ListContractorsDto FromRequest() => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        ContractorStatus = ContractorStatus,
        Status = Status,
        TripStartDate = TripStartDate,
        TripEndDate = TripEndDate
    };
}

public class ListContractorsRequestValidator : AbstractValidator<ListContractorsRequest>
{
    public ListContractorsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize();

        RuleFor(x => x.ContractorStatus)
            .IsInEnum()
            .When(x => x.ContractorStatus.HasValue);
        RuleFor(x => x.Status)
            .IsInEnum()
            .When(x => x.Status.HasValue);

        RuleFor(x => x.TripEndDate)
            .GreaterThanOrEqualTo(x => x.TripStartDate)
            .When(x => x.TripStartDate.HasValue && x.TripEndDate.HasValue)
            .WithMessage("validation.trips_end_on_or_after_start");
    }
}
