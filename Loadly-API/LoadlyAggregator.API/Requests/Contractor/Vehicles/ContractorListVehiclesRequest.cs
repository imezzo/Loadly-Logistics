using FluentValidation;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Common.DTOs.Common;
using LoadlyAggregator.Domain.Common.Enums;

namespace LoadlyAggregator.API.Requests.Contractor.Vehicles;

public class ContractorListVehiclesRequest : BasePagedRequest
{
    public long? VehicleTypeId { get; set; }
    public List<long>? VehicleTypeIds { get; set; }
    public VehicleUnitType? VehicleUnitType { get; set; }
    public List<VehicleTypeCategory>? VehicleTypeCategories { get; set; }
    public int? Status { get; set; }
    public bool? IsAvailableTail { get; set; }
    public long? DriverId { get; set; }
    public bool? HasDriver { get; set; }

    public Domain.Contractor.DTOs.Vehicles.ContractorListVehiclesDto FromRequest(long contractorId) => new()
    {
        Page = Page,
        PageSize = PageSize,
        Search = Search,
        ContractorId = contractorId,
        VehicleTypeId = VehicleTypeId,
        VehicleTypeIds = VehicleTypeIds,
        VehicleUnitType = VehicleUnitType,
        VehicleTypeCategories = VehicleTypeCategories,
        Status = Status,
        IsAvailableTail = IsAvailableTail,
        DriverId = DriverId,
        HasDriver = HasDriver
    };
}

public class ContractorListVehiclesRequestValidator : AbstractValidator<ContractorListVehiclesRequest>
{
    public ContractorListVehiclesRequestValidator()
    {
        RuleFor(x => x.Page).IsGreaterThanZero("Page");
        RuleFor(x => x.PageSize)
            .IsGreaterThanZero("Page size")
            .LessThanOrEqualTo(100).WithMessage("validation.page_size_must_not_exceed|100");

        RuleFor(x => x.VehicleTypeId)
            .IsOptionalId("Vehicle type ID")
            .When(x => x.VehicleTypeId.HasValue);

        RuleForEach(x => x.VehicleTypeIds)
            .GreaterThan(0L).WithMessage("validation.entity_must_be_greater_than_zero|Vehicle type ID")
            .When(x => x.VehicleTypeIds is { Count: > 0 });

        RuleFor(x => x.DriverId)
            .IsOptionalId("Driver ID")
            .When(x => x.DriverId.HasValue);
    }
}
