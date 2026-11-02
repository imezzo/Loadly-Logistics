using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using LoadlyAggregator.API.Requests.Admin.Contracts;
using LoadlyAggregator.API.Validators.Extensions;
using LoadlyAggregator.Domain.Admin.DTOs.Contracts;
using LoadlyAggregator.Domain.Admin.Helpers;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Models;

namespace LoadlyAggregator.API.Requests.Admin.Contracts;

public class UpdateContractRequest
{
    public const string Permission = "edit_contract";

    [FromRoute]
    public long Id { get; set; }
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
    /// <summary>Optional. If true and contract is DRAFT, submit for approval after update; otherwise leave as DRAFT (default).</summary>
    public bool SubmitForApproval { get; set; } = false;
    public string? TermsAndConditions { get; set; }
    public long? SalesRepId { get; set; }
    public bool AutoAssignMode { get; set; }
    public ContractKmOverageMode? KmOverageMode { get; set; }
    public List<VehicleTypeItemRequest>? VehicleTypes { get; set; }
    public List<DocumentItemRequest>? Documents { get; set; }
    public List<ExtraFeeItemRequest>? ExtraFees { get; set; }
    public List<ContractDestinationItemRequest>? Destinations { get; set; }
    public List<ContractSlotItemRequest>? Slots { get; set; }

    public UpdateContractDto FromRequest(long id) => new()
    {
        Id = id,
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
        AutoAssignMode = AutoAssignMode,
        KmOverageMode = KmOverageMode,
        VehicleTypes = VehicleTypes?.Select(vt => new ContractVehicleTypeDto
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

public class UpdateContractRequestValidator : AbstractValidator<UpdateContractRequest>
{
    public UpdateContractRequestValidator()
    {
        RuleFor(x => x.Id).IsRequiredId("Contract ID");

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
            .IsAfterDate(x => x.StartDate, "End date", "Start date")
            .IsFutureDate("End date");

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
