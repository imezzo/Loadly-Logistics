using FluentValidation;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.API.Requests.Admin.Contracts;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.Helpers;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class CreateContractRequest
{
    public const string Permission = "create_contract";

    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public long CompanyId { get; set; }
    public ContractType ContractType { get; set; }
    public decimal Price { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public short PaymentDueDays { get; set; }
    public ContractBillingCycles BillingCycle { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public ContractConsumptionTypes ConsumptionType { get; set; }
    public decimal? CargoCommission { get; set; }
    public int? MonthlyWorkingDays { get; set; }
    public int? VehiclesCount { get; set; }
    /// <summary>Optional. If true, contract is submitted for approval after creation; otherwise it remains in Draft (default).</summary>
    public bool SubmitForApproval { get; set; } = false;
    public string? TermsAndConditions { get; set; }
    public long? SalesRepId { get; set; }
    /// <summary>Optional. When set, creates a renewal: new contract is linked and the given contract is marked RENEWED. That contract must be ACTIVE or EXPIRED. The new contract must have the same CompanyId and ContractTypeId as the original.</summary>
    public long? RenewedFromContractId { get; set; }
    public bool AutoAssignMode { get; set; }
    public ContractKmOverageMode? KmOverageMode { get; set; }
    public List<VehicleTypeItemRequest>? VehicleTypes { get; set; }
    public List<DocumentItemRequest>? Documents { get; set; }
    public List<ExtraFeeItemRequest>? ExtraFees { get; set; }
    public List<ContractDestinationItemRequest>? Destinations { get; set; }
    public List<ContractSlotItemRequest>? Slots { get; set; }

    public CreateContractDto FromRequest() => new()
    {
        NameAr = NameAr,
        NameEn = NameEn,
        CompanyId = CompanyId,
        ContractType = ContractType,
        Price = Price,
        PaymentMethod = PaymentMethod,
        PaymentDueDays = PaymentDueDays,
        BillingCycle = BillingCycle,
        StartDate = StartDate,
        EndDate = EndDate,
        ConsumptionType = ConsumptionType,
        CargoCommission = CargoCommission,
        MonthlyWorkingDays = MonthlyWorkingDays,
        VehiclesCount = VehiclesCount,
        SubmitForApproval = SubmitForApproval,
        TermsAndConditions = TermsAndConditions,
        SalesRepId = SalesRepId,
        RenewedFromContractId = RenewedFromContractId,
        AutoAssignMode = AutoAssignMode,
        KmOverageMode = KmOverageMode,
        VehicleTypes = VehicleTypes?.Select(vt => new CreateContractVehicleTypeDto
        {
            VehicleTypeId = vt.VehicleTypeId,
            ConsumptionRate = vt.ConsumptionRate,
            TripsPerMonth = vt.TripsPerMonth,
            MaxKms = vt.MaxKms,
            MonthlyRental = vt.MonthlyRental,
            OverConsumption = vt.OverConsumption,
            Quantity = vt.Quantity,
            MinBillableWeightTons = vt.MinBillableWeightTons,
            MaxPayloadTons = vt.MaxPayloadTons,
            OverWeightPricePerTon = vt.OverWeightPricePerTon,
            ContractorOverWeightPricePerTon = vt.ContractorOverWeightPricePerTon,
            ExtraWorkingDayFees = vt.ExtraWorkingDayFees
        }).ToList(),
        Documents = Documents?.Select(d => new DocumentItemDto
        {
            DocumentId = d.DocumentId,
            File = d.File,
            ExpiryDate = d.ExpiryDate,
            Status = d.Status
        }).ToList(),
        ExtraFees = ExtraFees?.Select(ef => new ExtraFeeItemDto
        {
            ExtraFeeTypeId = ef.ExtraFeeTypeId,
            CalculationOption = ef.CalculationOption,
            Value = ef.Value,
            Threshold = ef.Threshold,
            Notes = ef.Notes,
            AllowOverride = ef.AllowOverride
        }).ToList(),
        Destinations = Destinations?.Select(d => new ContractDestinationItemDto
        {
            Start = ContractDestinationWaypointRequestMapping.ToWaypointDto(d.Start),
            End = ContractDestinationWaypointRequestMapping.ToWaypointDto(d.End),
            NameAr = d.NameAr,
            NameEn = d.NameEn,
            ExpectedKms = d.ExpectedKms,
            ReturnExpectedKms = d.ReturnExpectedKms,
            NumberOfDays = d.NumberOfDays,
            PricePerTon = d.PricePerTon,
            FixedTripPrice = d.FixedTripPrice,
            ContractorPricePerTon = d.ContractorPricePerTon,
            ContractorFixedTripPrice = d.ContractorFixedTripPrice,
            Description = d.Description
        }).ToList(),
        Slots = Slots?.Select(ContractSlotRequestMapping.ToDto).ToList()
    };
}

public class ContractSlotItemRequest
{
    public long? Id { get; set; }
    public long VehicleTypeId { get; set; }
    public long? VehicleId { get; set; }
    public DateTime AssignedAt { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public ContractSlotStatus Status { get; set; } = ContractSlotStatus.ACTIVE;
    public decimal? ConsumptionRate { get; set; }
    public int? TripsPerMonth { get; set; }
    public int? TripsPerDay { get; set; }
    public decimal? MaxKms { get; set; }
    public decimal? MonthlyRental { get; set; }
    public decimal? DailyValue { get; set; }
    public decimal? OverConsumption { get; set; }
    public decimal? MinBillableWeightTons { get; set; }
    public decimal? MaxPayloadTons { get; set; }
    public decimal? OverWeightPricePerTon { get; set; }
    public decimal? ContractorOverWeightPricePerTon { get; set; }
    public decimal? ExtraWorkingDayFees { get; set; }
    public long? ReplacedSlotId { get; set; }
}

internal static class ContractSlotRequestMapping
{
    public static ContractSlotItemDto ToDto(ContractSlotItemRequest slot) => new()
    {
        Id = slot.Id,
        VehicleTypeId = slot.VehicleTypeId,
        VehicleId = slot.VehicleId,
        AssignedAt = slot.AssignedAt,
        ArchivedAt = slot.ArchivedAt,
        Status = slot.Status,
        ConsumptionRate = slot.ConsumptionRate,
        TripsPerMonth = slot.TripsPerMonth,
        TripsPerDay = slot.TripsPerDay,
        MaxKms = slot.MaxKms,
        MonthlyRental = slot.MonthlyRental,
        DailyValue = slot.DailyValue,
        OverConsumption = slot.OverConsumption,
        MinBillableWeightTons = slot.MinBillableWeightTons,
        MaxPayloadTons = slot.MaxPayloadTons,
        OverWeightPricePerTon = slot.OverWeightPricePerTon,
        ExtraWorkingDayFees = slot.ExtraWorkingDayFees,
        ReplacedSlotId = slot.ReplacedSlotId,
        ContractorOverWeightPricePerTon = slot.ContractorOverWeightPricePerTon,
    };
}

public class ContractSlotItemRequestValidator : AbstractValidator<ContractSlotItemRequest>
{
    public ContractSlotItemRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .When(x => x.Id.HasValue)
            .WithMessage("contracts.contract_slot_id_invalid");

        RuleFor(x => x.VehicleTypeId)
            .IsGreaterThanZero("Vehicle type ID");

        RuleFor(x => x.VehicleId)
            .IsGreaterThanZero("Vehicle ID")
            .When(x => x.VehicleId.HasValue);

        RuleFor(x => x.AssignedAt)
            .IsRequiredDate("Assigned at");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("validation.invalid_contract_slot_status");

        RuleFor(x => x.MonthlyRental)
            .GreaterThan(0)
            .WithMessage("contracts.contract_vehicle_type_monthly_rental_invalid")
            .When(x => x.MonthlyRental.HasValue);

        RuleFor(x => x.OverConsumption)
            .GreaterThan(0)
            .WithMessage("contracts.contract_vehicle_type_over_consumption_invalid")
            .When(x => x.OverConsumption.HasValue);

        RuleFor(x => x.MinBillableWeightTons)
            .GreaterThan(0)
            .WithMessage("contracts.contract_vehicle_type_min_billable_weight_tons_invalid")
            .When(x => x.MinBillableWeightTons.HasValue);

        RuleFor(x => x.MaxPayloadTons)
            .GreaterThan(0)
            .WithMessage("contracts.contract_vehicle_type_max_payload_tons_invalid")
            .When(x => x.MaxPayloadTons.HasValue);

        RuleFor(x => x.ConsumptionRate)
            .GreaterThan(0)
            .WithMessage("contracts.contract_vehicle_type_consumption_rate_invalid")
            .When(x => x.ConsumptionRate.HasValue);

        RuleFor(x => x.OverWeightPricePerTon)
            .GreaterThan(0)
            .WithMessage("contracts.contract_vehicle_type_over_weight_price_per_ton_required")
            .When(x => x.OverWeightPricePerTon.HasValue);

        RuleFor(x => x.ContractorOverWeightPricePerTon)
            .GreaterThan(0)
            .WithMessage("contracts.contract_vehicle_type_over_weight_price_per_ton_required")
            .When(x => x.ContractorOverWeightPricePerTon.HasValue);

        RuleFor(x => x.ExtraWorkingDayFees)
            .GreaterThan(0)
            .WithMessage("contracts.contract_vehicle_type_extra_working_day_fees_invalid")
            .When(x => x.ExtraWorkingDayFees.HasValue);

        RuleFor(x => x.ReplacedSlotId)
            .IsOptionalId("Replaced slot");

        RuleFor(x => x.TripsPerMonth)
            .GreaterThan(0)
            .WithMessage("contracts.contract_vehicle_type_trips_per_month_invalid")
            .When(x => x.TripsPerMonth.HasValue);

        RuleFor(x => x.TripsPerDay)
            .GreaterThan(0)
            .WithMessage("contracts.contract_vehicle_type_trips_per_day_invalid")
            .When(x => x.TripsPerDay.HasValue);

        RuleFor(x => x.DailyValue)
            .GreaterThan(0)
            .WithMessage("contracts.contract_vehicle_type_daily_value_invalid")
            .When(x => x.DailyValue.HasValue);

        RuleFor(x => x.MaxKms)
            .GreaterThan(0)
            .WithMessage("contracts.contract_vehicle_type_max_kms_invalid")
            .When(x => x.MaxKms.HasValue);

        RuleFor(x => x.ArchivedAt)
            .Must((slot, archivedAt) => !archivedAt.HasValue || archivedAt.Value >= slot.AssignedAt)
            .WithMessage("contracts.archived_at_before_assigned_at")
            .When(x => x.Status == ContractSlotStatus.ARCHIVED && x.ArchivedAt.HasValue);

        RuleFor(x => x.ArchivedAt)
            .Must(archivedAt => !archivedAt.HasValue || archivedAt.Value <= DateTime.UtcNow)
            .WithMessage("contracts.archived_at_in_future")
            .When(x => x.Status == ContractSlotStatus.ARCHIVED && x.ArchivedAt.HasValue);
    }
}

public class VehicleTypeItemRequest
{
    public long VehicleTypeId { get; set; }
    public decimal? ConsumptionRate { get; set; }
    public int? TripsPerMonth { get; set; }
    public decimal? MaxKms { get; set; }
    public decimal? MonthlyRental { get; set; }
    public decimal? OverConsumption { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal? MinBillableWeightTons { get; set; }
    public decimal? MaxPayloadTons { get; set; }
    public decimal? OverWeightPricePerTon { get; set; }
    public decimal? ContractorOverWeightPricePerTon { get; set; }
    public decimal? ExtraWorkingDayFees { get; set; }
}

public class DocumentItemRequest
{
    public long DocumentId { get; set; }
    public FileUpload? File { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DocumentVerificationStatus Status { get; set; } = DocumentVerificationStatus.PENDING;
}

public class ExtraFeeItemRequest
{
    public long ExtraFeeTypeId { get; set; }
    public int? CalculationOption { get; set; }
    public decimal? Value { get; set; }
    public int? Threshold { get; set; }
    public string? Notes { get; set; }
    public bool? AllowOverride { get; set; }
}

public class ContractDestinationWaypointItemRequest
{
    /// <summary>When set and greater than zero, the existing waypoint is validated; otherwise a new waypoint is created.</summary>
    public long? Id { get; set; }
    public string? GooglePlaceId { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string AddressName { get; set; } = string.Empty;
}

public class ContractDestinationItemRequest
{
    public ContractDestinationWaypointItemRequest? Start { get; set; }
    public ContractDestinationWaypointItemRequest? End { get; set; }
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public decimal? ExpectedKms { get; set; }
    public decimal? ReturnExpectedKms { get; set; }
    public int? NumberOfDays { get; set; }
    public decimal? PricePerTon { get; set; }
    public decimal? FixedTripPrice { get; set; }
    public decimal? ContractorPricePerTon { get; set; }
    public decimal? ContractorFixedTripPrice { get; set; }
    public string? Description { get; set; }
}

internal static class ContractDestinationWaypointRequestMapping
{
    public static ContractDestinationWaypointItemDto ToWaypointDto(ContractDestinationWaypointItemRequest? w) =>
        w is null
            ? new ContractDestinationWaypointItemDto()
            : new ContractDestinationWaypointItemDto
            {
                Id = w.Id,
                GooglePlaceId = w.GooglePlaceId,
                Latitude = w.Latitude,
                Longitude = w.Longitude,
                AddressName = w.AddressName ?? string.Empty
            };
}

internal static class ContractDestinationRequestValidation
{
    public static string WaypointKey(ContractDestinationWaypointItemRequest? w)
    {
        if (w is null)
            return "";
        if (w.Id is { } id && id > 0)
            return $"id:{id}";
        return $"addr:{(w.AddressName ?? "").Trim().ToLowerInvariant()}";
    }
}

public class ContractDestinationWaypointItemRequestValidator : AbstractValidator<ContractDestinationWaypointItemRequest>
{
    public ContractDestinationWaypointItemRequestValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .When(x => x.Id.HasValue)
            .WithMessage("contracts.contract_route_waypoint_id_invalid");

        RuleFor(x => x.AddressName)
            .NotEmpty()
            .When(x => !x.Id.HasValue || x.Id.Value <= 0)
            .WithMessage("contracts.contract_route_waypoint_address_required");
    }
}

public class ContractDestinationItemRequestValidator : AbstractValidator<ContractDestinationItemRequest>
{
    public ContractDestinationItemRequestValidator()
    {
        RuleFor(x => x.Start)
            .NotNull()
            .WithMessage("contracts.contract_route_start_waypoint_required")
            .DependentRules(() =>
            {
                RuleFor(x => x.Start!)
                    .SetValidator(new ContractDestinationWaypointItemRequestValidator());
            });

        RuleFor(x => x.End)
            .NotNull()
            .WithMessage("contracts.contract_route_end_waypoint_required")
            .DependentRules(() =>
            {
                RuleFor(x => x.End!)
                    .SetValidator(new ContractDestinationWaypointItemRequestValidator());
            });

        RuleFor(x => x)
            .Must(x =>
                x.Start == null
                || x.End == null
                || ContractDestinationRequestValidation.WaypointKey(x.Start)
                    != ContractDestinationRequestValidation.WaypointKey(x.End))
            .WithMessage("contracts.contract_route_start_end_must_differ");

        RuleFor(x => x.NameAr)
            .IsRequiredArabicName();
        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName();

        RuleFor(x => x.NumberOfDays)
            .InclusiveBetween(1, 366)
            .When(x => x.NumberOfDays.HasValue)
            .WithMessage("validation.must_be_between|1|366");

        RuleFor(x => x.ExpectedKms)
            .Must(v => v!.Value > 0m && v.Value <= 100_000m)
            .When(x => x.ExpectedKms.HasValue)
            .WithMessage("contracts.contract_route_expected_kms_invalid");

        RuleFor(x => x.ReturnExpectedKms)
            .Must(v => v!.Value > 0m && v.Value <= 100_000m)
            .When(x => x.ReturnExpectedKms.HasValue)
            .WithMessage("contracts.contract_route_expected_kms_invalid");

        RuleFor(x => x.PricePerTon)
            .GreaterThan(0)
            .When(x => x.PricePerTon.HasValue)
            .WithMessage("contracts.contract_route_price_per_ton_invalid");

        RuleFor(x => x.FixedTripPrice)
            .GreaterThan(0)
            .When(x => x.FixedTripPrice.HasValue)
            .WithMessage("contracts.contract_route_fixed_trip_price_invalid");

        RuleFor(x => x.ContractorPricePerTon)
            .GreaterThan(0)
            .When(x => x.ContractorPricePerTon.HasValue)
            .WithMessage("contracts.contract_route_price_per_ton_invalid");

        RuleFor(x => x.ContractorFixedTripPrice)
            .GreaterThan(0)
            .When(x => x.ContractorFixedTripPrice.HasValue)
            .WithMessage("contracts.contract_route_fixed_trip_price_invalid");
    }
}

public class CreateContractRequestValidator : AbstractValidator<CreateContractRequest>
{
    public CreateContractRequestValidator()
    {
        RuleFor(x => x.NameAr)
            .IsRequiredArabicName()
            .Matches(@"^[\u0600-\u06FF\u0660-\u06690-9\s]+$").WithMessage("validation.contract_name_ar_chars")
            .Matches(@"[\u0600-\u06FF]{2,}").WithMessage("validation.contract_name_ar_min_two");

        RuleFor(x => x.NameEn)
            .IsRequiredEnglishName()
            .Matches(@"^[a-zA-Z0-9\s]+$").WithMessage("validation.contract_name_en_chars")
            .Matches(@"[a-zA-Z]{2,}").WithMessage("validation.contract_name_en_min_two");

        RuleFor(x => x.CompanyId)
            .IsGreaterThanZero("Company ID");

        RuleFor(x => x.ContractType)
            .IsInEnum().WithMessage("validation.invalid_contract_type");

        RuleFor(x => x.ConsumptionType)
            .IsInEnum().WithMessage("validation.invalid_consumption_type");

        // TODO: Better to be accumulatd at backend ( from 1 to n ( sum += quantity * dailyValue ) )
        RuleFor(x => x.Price)
            .IsGreaterThanOrEqualToZero("Price");

        RuleFor(x => x.CargoCommission)
            .GreaterThan(0)
            .LessThan(1).WithMessage("contracts.cargo_commission_invalid")
            .When(x => x.CargoCommission.HasValue);

        RuleFor(x => x.PaymentDueDays)
            .IsGreaterThanZero("Payment due days");

        RuleFor(x => x.StartDate)
            .IsRequiredDate("Start date");

        RuleFor(x => x.EndDate)
            .IsRequiredDate("End date")
            .IsAfterDate(x => x.StartDate, "End date", "Start date");

        RuleFor(x => x.MonthlyWorkingDays)
            .NotNull().WithMessage("validation.field_is_required|Monthly working days")
            .IsGreaterThanZero("Monthly working days")
            .When(x => x.ContractType is ContractType.Included or ContractType.Excluded);

        RuleFor(x => x.VehiclesCount)
            .IsGreaterThanZero("Vehicles count")
            .When(x => x.VehiclesCount.HasValue);

        RuleFor(x => x.SalesRepId)
            .NotNull().WithMessage("validation.sales_representative_required")
            .IsGreaterThanZero("Sales representative ID");

        When(x => x.VehicleTypes != null && x.VehicleTypes.Any(), () =>
        {
            RuleForEach(x => x.VehicleTypes).ChildRules(vt =>
            {
                vt.RuleFor(x => x.VehicleTypeId)
                    .IsGreaterThanZero("Vehicle type ID");

                vt.RuleFor(x => x.MonthlyRental)
                    .GreaterThan(0)
                    .WithMessage("contracts.contract_vehicle_type_monthly_rental_invalid")
                    .When(x => x.MonthlyRental.HasValue);

                vt.RuleFor(x => x.OverConsumption)
                    .GreaterThan(0)
                    .WithMessage("contracts.contract_vehicle_type_over_consumption_invalid")
                    .When(x => x.OverConsumption.HasValue);

                vt.RuleFor(x => x.Quantity)
                    .GreaterThan(0)
                    .WithMessage("contracts.contract_vehicle_type_quantity_must_be_greater_that_zero");

                vt.RuleFor(x => x.MinBillableWeightTons)
                    .GreaterThan(0)
                    .WithMessage("contracts.contract_vehicle_type_min_billable_weight_tons_invalid")
                    .When(x => x.MinBillableWeightTons.HasValue);

                vt.RuleFor(x => x.MaxPayloadTons)
                    .GreaterThan(0)
                    .WithMessage("contracts.contract_vehicle_type_max_payload_tons_invalid")
                    .When(x => x.MaxPayloadTons.HasValue);

                vt.RuleFor(x => x.ConsumptionRate)
                    .GreaterThan(0)
                    .WithMessage("contracts.contract_vehicle_type_consumption_rate_invalid")
                    .When(x => x.ConsumptionRate.HasValue);

                vt.RuleFor(x => x.OverWeightPricePerTon)
                    .GreaterThan(0)
                    .WithMessage("contracts.contract_vehicle_type_over_weight_price_per_ton_required")
                    .When(x => x.OverWeightPricePerTon.HasValue);

                vt.RuleFor(x => x.ContractorOverWeightPricePerTon)
                    .GreaterThan(0)
                    .WithMessage("contracts.contract_vehicle_type_over_weight_price_per_ton_required")
                    .When(x => x.ContractorOverWeightPricePerTon.HasValue);

                vt.RuleFor(x => x.ExtraWorkingDayFees)
                    .GreaterThan(0)
                    .WithMessage("contracts.contract_vehicle_type_extra_working_day_fees_invalid")
                    .When(x => x.ExtraWorkingDayFees.HasValue);

                vt.RuleFor(x => x.TripsPerMonth)
                    .GreaterThan(0)
                    .WithMessage("contracts.contract_vehicle_type_trips_per_month_invalid")
                    .When(x => x.TripsPerMonth.HasValue);

                vt.RuleFor(x => x.MaxKms)
                    .GreaterThan(0)
                    .WithMessage("contracts.contract_vehicle_type_max_kms_invalid")
                    .When(x => x.MaxKms.HasValue);

            });
        });

        When(x => x.SubmitForApproval && x.VehicleTypes != null && x.VehicleTypes.Any(), () =>
        {

            When(x => x.ConsumptionType == ContractConsumptionTypes.CARGO_WEIGHTS, () =>
            {
                RuleForEach(x => x.VehicleTypes).ChildRules(vt =>
                    {
                        vt.RuleFor(x => x.MinBillableWeightTons)
                            .NotNull()
                            .GreaterThan(0)
                            .WithMessage("contracts.contract_vehicle_type_min_billable_weight_tons_required");

                        vt.RuleFor(x => x.MaxPayloadTons)
                            .NotNull()
                            .GreaterThan(0)
                            .WithMessage("contracts.contract_vehicle_type_max_payload_tons_required");
                    });
            });

            When(x => !ContractFuelExtraFeeHelper.IsContractTripPricingConveringFuel(x.ContractType, x.ConsumptionType), () =>
            {
                RuleForEach(x => x.VehicleTypes).ChildRules(vt =>
                    {
                        vt.RuleFor(x => x.ConsumptionRate)
                            .NotNull().WithMessage("contracts.contract_vehicle_type_consumption_rate_required")
                            .GreaterThan(0).WithMessage("contracts.contract_vehicle_type_consumption_rate_required");
                    });
            });

        });

        When(x => x.Slots != null && x.Slots.Any(), () =>
        {
            RuleFor(x => x.Slots!)
                .Must(list => list
                    .Where(s => s.Status == ContractSlotStatus.ACTIVE && s.VehicleId.HasValue)
                    .Select(s => s.VehicleId!.Value)
                    .Distinct()
                    .Count() == list.Count(s => s.Status == ContractSlotStatus.ACTIVE && s.VehicleId.HasValue))
                .WithMessage("contracts.duplicate_slot_vehicle");

            RuleForEach(x => x.Slots).SetValidator(new ContractSlotItemRequestValidator());
        });

        When(x => x.SubmitForApproval && x.Slots != null && x.Slots.Any(), () =>
        {
            When(x => x.ConsumptionType == ContractConsumptionTypes.CARGO_WEIGHTS, () =>
            {
                RuleForEach(x => x.Slots).ChildRules(slot =>
                {
                    slot.RuleFor(x => x.MinBillableWeightTons)
                        .NotNull()
                        .GreaterThan(0)
                        .WithMessage("contracts.contract_vehicle_type_min_billable_weight_tons_required");

                    slot.RuleFor(x => x.MaxPayloadTons)
                        .NotNull()
                        .GreaterThan(0)
                        .WithMessage("contracts.contract_vehicle_type_max_payload_tons_required");
                });
            });

            When(x => !ContractFuelExtraFeeHelper.IsContractTripPricingConveringFuel(x.ContractType, x.ConsumptionType), () =>
            {
                RuleForEach(x => x.Slots).ChildRules(slot =>
                {
                    slot.RuleFor(x => x.ConsumptionRate)
                        .NotNull().WithMessage("contracts.contract_vehicle_type_consumption_rate_required")
                        .GreaterThan(0).WithMessage("contracts.contract_vehicle_type_consumption_rate_required");
                });
            });
        });

        When(x => x.Documents != null && x.Documents.Any(), () =>
        {
            RuleForEach(x => x.Documents).ChildRules(d =>
            {
                d.RuleFor(x => x.DocumentId)
                    .IsGreaterThanZero("Document ID");
            });
        });

        When(x => x.ExtraFees != null && x.ExtraFees.Any(), () =>
        {
            RuleForEach(x => x.ExtraFees).ChildRules(ef =>
            {
                ef.RuleFor(x => x.ExtraFeeTypeId)
                    .IsGreaterThanZero("Extra fee type ID");
            });
        });

        When(x => x.Destinations != null && x.Destinations.Any(), () =>
        {
            RuleFor(x => x.Destinations!)
                .Must(list => list
                    .Select(d => (ContractDestinationRequestValidation.WaypointKey(d.Start), ContractDestinationRequestValidation.WaypointKey(d.End)))
                    .Distinct()
                    .Count() == list.Count)
                .WithMessage("contracts.contract_route_duplicate_in_request");

            //RuleFor(x => x.Destinations!)
            //    .Must(list => list.All(d =>
            //        d.Start is { Latitude: not null, Longitude: not null }
            //        && d.End is { Latitude: not null, Longitude: not null }))
            //    .WithMessage("contracts.contract_route_waypoint_coordinates_required")
            //    .When(x => x.SubmitForApproval);

            RuleFor(x => x.Destinations!)
                .Must(list => list.All(d => d.PricePerTon.HasValue && d.PricePerTon.Value > 0))
                .WithMessage("contracts.contract_route_price_per_ton_required")
                .When(x => x.SubmitForApproval && x.ConsumptionType == ContractConsumptionTypes.CARGO_WEIGHTS);

            RuleFor(x => x.Destinations!)
                .Must(list => list.All(d => d.FixedTripPrice.HasValue && d.FixedTripPrice.Value > 0))
                .WithMessage("contracts.contract_route_fixed_trip_price_required")
                .When(x => x.SubmitForApproval && x.ConsumptionType == ContractConsumptionTypes.DESTINATION_FIXED_PRICE);

            RuleForEach(x => x.Destinations).SetValidator(new ContractDestinationItemRequestValidator());
        });

        RuleFor(x => x.CargoCommission)
            .NotNull()
            .GreaterThan(0)
            .LessThan(1)
            .WithMessage("contracts.cargo_commission_percentage_required")
            .When(x => x.SubmitForApproval && x.ConsumptionType == ContractConsumptionTypes.CARGO_PRICE_COMMISSION);
    }
}
