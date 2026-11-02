using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Export;

namespace LoadlyAggregator.Infrastructure.Repositories.Helpers;

internal static class TripColumnIncludes
{
    public static IQueryable<TripExportRow> Apply(IQueryable<Trip> query, IReadOnlyCollection<string> columns)
    {
        HashSet<string> requested = new(columns, StringComparer.OrdinalIgnoreCase);
        bool Has(params string[] keys) => keys.Any(requested.Contains);
        ParameterExpression trip = Expression.Parameter(typeof(Trip), "trip");
        List<MemberBinding> bindings =
        [
            FromTrip(trip, nameof(TripExportRow.Id), nameof(Trip.Id)),
            FromTrip(trip, nameof(TripExportRow.CreatedAt), nameof(Trip.CreatedAt)),
            FromTrip(trip, nameof(TripExportRow.StartDate), nameof(Trip.StartDate)),
            FromTrip(trip, nameof(TripExportRow.EndDate), nameof(Trip.EndDate)),
            FromTrip(trip, nameof(TripExportRow.FinancialDate), nameof(Trip.FinancialDate)),
            FromTrip(trip, nameof(TripExportRow.Status), nameof(Trip.Status)),
            FromTrip(trip, nameof(TripExportRow.TripDocumentStatus), nameof(Trip.TripDocumentStatus)),
            FromTrip(trip, nameof(TripExportRow.NumberOfDays), nameof(Trip.NumberOfDays)),
            FromTrip(trip, nameof(TripExportRow.IsExternalTrip), nameof(Trip.IsExternalTrip)),
            FromTrip(trip, nameof(TripExportRow.IsBackfill), nameof(Trip.IsBackfill)),
            FromTrip(trip, nameof(TripExportRow.ContactPersonName), nameof(Trip.ContactPersonName)),
            FromTrip(trip, nameof(TripExportRow.ContactPersonMobile), nameof(Trip.ContactPersonMobile)),
            FromTrip(trip, nameof(TripExportRow.HelpersCount), nameof(Trip.HelpersCount)),
            FromTrip(trip, nameof(TripExportRow.ContractDistanceKm), nameof(Trip.ContractDistanceKm)),
            FromTrip(trip, nameof(TripExportRow.ActualDistanceKm), nameof(Trip.ActualDistanceKm)),
            FromTrip(trip, nameof(TripExportRow.ActualCargoWeightTons), nameof(Trip.ActualCargoWeightTons)),
            FromTrip(trip, nameof(TripExportRow.CargoPrice), nameof(Trip.CargoPrice)),
            FromTrip(trip, nameof(TripExportRow.SpotPrice), nameof(Trip.SpotPrice)),
            FromTrip(trip, nameof(TripExportRow.TotalTripCost), nameof(Trip.TotalTripCost)),
            FromTrip(trip, nameof(TripExportRow.TotalAddonsCost), nameof(Trip.TotalAddonsCost)),
            FromTrip(trip, nameof(TripExportRow.OverNightCostBearer), nameof(Trip.OverNightCostBearer)),
            FromTrip(trip, nameof(TripExportRow.CustomsStatus), nameof(Trip.CustomsStatus)),
            FromTrip(trip, nameof(TripExportRow.CustomsTotalAmount), nameof(Trip.CustomsTotalAmount)),
            FromTrip(trip, nameof(TripExportRow.DispatcherNotes), nameof(Trip.DispatcherNotes))
        ];

        if (Has("company"))
        {
            bindings.Add(Bind(nameof(TripExportRow.CompanyNameEn), Path(trip, nameof(Trip.Contract), nameof(Contract.Company), nameof(Company.CompanyDetail), nameof(CompanyDetail.NameEn))));
            bindings.Add(Bind(nameof(TripExportRow.CompanyNameAr), Path(trip, nameof(Trip.Contract), nameof(Contract.Company), nameof(Company.CompanyDetail), nameof(CompanyDetail.NameAr))));
        }

        if (Has("contract"))
            bindings.Add(Bind(nameof(TripExportRow.ContractReferenceNumber), Path(trip, nameof(Trip.Contract), nameof(Contract.ReferenceNumber))));

        if (Has("driver"))
        {
            bindings.Add(Bind(nameof(TripExportRow.DriverName), Path(trip, nameof(Trip.Driver), nameof(Driver.FullName))));
            bindings.Add(Bind(nameof(TripExportRow.DriverNameAr), Path(trip, nameof(Trip.Driver), nameof(Driver.FullNameAr))));
        }

        if (Has("dispatcher"))
            bindings.Add(Bind(nameof(TripExportRow.DispatcherName), Path(trip, nameof(Trip.Dispatcher), nameof(Admin.FullName))));

        if (Has("vehicle"))
        {
            bindings.Add(Bind(nameof(TripExportRow.HeadPlateNo), Path(trip, nameof(Trip.VehicleAssignmentSnapshot), nameof(TripVehicleAssignmentSnapshot.HeadPlateNo))));
            bindings.Add(Bind(nameof(TripExportRow.HeadPlateLetters), Path(trip, nameof(Trip.VehicleAssignmentSnapshot), nameof(TripVehicleAssignmentSnapshot.HeadPlateLetters))));
            bindings.Add(Bind(nameof(TripExportRow.VehiclePlateNo), Path(trip, nameof(Trip.Vehicle), nameof(Vehicle.PlateNo))));
            bindings.Add(Bind(nameof(TripExportRow.VehiclePlateLetters), Path(trip, nameof(Trip.Vehicle), nameof(Vehicle.PlateLetters))));
        }

        if (Has("vehicleType"))
        {
            bindings.Add(Bind(nameof(TripExportRow.HasContractVehicleType), Expression.NotEqual(
                Expression.Property(trip, nameof(Trip.ContractVehicleType)),
                Expression.Constant(null, typeof(ContractVehicleType)))));
            bindings.Add(Bind(nameof(TripExportRow.ContractVehicleTypeNameEn), Path(trip, nameof(Trip.ContractVehicleType), nameof(ContractVehicleType.VehicleType), nameof(VehicleType.NameEn))));
            bindings.Add(Bind(nameof(TripExportRow.ContractVehicleTypeNameAr), Path(trip, nameof(Trip.ContractVehicleType), nameof(ContractVehicleType.VehicleType), nameof(VehicleType.NameAr))));
            bindings.Add(Bind(nameof(TripExportRow.VehicleTypeNameEn), Path(trip, nameof(Trip.Vehicle), nameof(Vehicle.VehicleType), nameof(VehicleType.NameEn))));
            bindings.Add(Bind(nameof(TripExportRow.VehicleTypeNameAr), Path(trip, nameof(Trip.Vehicle), nameof(Vehicle.VehicleType), nameof(VehicleType.NameAr))));
        }

        if (Has("consumptionRate", "totalFuelAmount"))
            bindings.Add(Bind(nameof(TripExportRow.ConsumptionRate), Path(trip, nameof(Trip.ContractVehicleType), nameof(ContractVehicleType.ConsumptionRate))));

        if (Has("sector"))
        {
            bindings.Add(Bind(nameof(TripExportRow.SectorNameEn), Path(trip, nameof(Trip.Vehicle), nameof(Vehicle.Sector), nameof(Sector.NameEn))));
            bindings.Add(Bind(nameof(TripExportRow.SectorNameAr), Path(trip, nameof(Trip.Vehicle), nameof(Vehicle.Sector), nameof(Sector.NameAr))));
        }

        if (Has("contractDestination"))
        {
            bindings.Add(Bind(nameof(TripExportRow.ContractDestinationNameEn), CurrentContractDestinationName(trip, nameof(ContractDestination.NameEn))));
            bindings.Add(Bind(nameof(TripExportRow.ContractDestinationNameAr), CurrentContractDestinationName(trip, nameof(ContractDestination.NameAr))));
        }

        if (Has("contractor"))
            bindings.Add(Bind(nameof(TripExportRow.ContractorName), Path(trip, nameof(Trip.Contractor), nameof(Contractor.FullName))));

        if (Has("lead"))
            bindings.Add(Bind(nameof(TripExportRow.LeadName), Path(trip, nameof(Trip.Lead), nameof(Lead.FullName))));

        bool split = false;
        if (Has("fuelPricingCalculationOption"))
            bindings.Add(Bind(nameof(TripExportRow.FuelCalculationOption), FuelOption(trip)));

        if (Has("fuelPriceCharge"))
        {
            bindings.Add(Bind(nameof(TripExportRow.TripCharges), ToList<TripCharge>(Expression.Property(trip, nameof(Trip.TripCharges)))));
            split = true;
        }

        if (Has("manualDestinations", "currentDriverStatus"))
        {
            bindings.Add(Bind(nameof(TripExportRow.Waypoints), Waypoints(trip)));
            split = true;
        }

        if (Has("duration", "currentDriverStatus", "lifecycleActorName", "cancellationReason"))
        {
            bindings.Add(Bind(nameof(TripExportRow.Recordings), Recordings(trip)));
            split = true;
        }

        if (split)
            query = query.AsSplitQuery();

        Expression<Func<Trip, TripExportRow>> projection = Expression.Lambda<Func<Trip, TripExportRow>>(
            Expression.MemberInit(Expression.New(typeof(TripExportRow)), bindings),
            trip);

        return query.Select(projection);
    }

    private static MemberAssignment FromTrip(ParameterExpression trip, string rowProperty, string tripProperty) =>
        Bind(rowProperty, Expression.Property(trip, tripProperty));

    private static MemberAssignment Bind(string rowProperty, Expression value) =>
        Expression.Bind(typeof(TripExportRow).GetProperty(rowProperty)!, value);

    private static Expression Path(Expression root, params string[] names)
    {
        Expression current = root;
        foreach (string name in names)
            current = Expression.Property(current, name);

        return current;
    }

    private static Expression CurrentContractDestinationName(ParameterExpression trip, string destinationProperty)
    {
        Expression assignment = Expression.Property(trip, nameof(Trip.ContractDestinationAssignment));
        Expression isActive = Expression.AndAlso(
            Expression.NotEqual(assignment, Expression.Constant(null, typeof(TripContractDestination))),
            Expression.Equal(
                Expression.Property(assignment, nameof(TripContractDestination.DeletedAt)),
                Expression.Constant(null, typeof(DateTime?))));

        Expression destination = Expression.Condition(
            isActive,
            Expression.Property(assignment, nameof(TripContractDestination.Destination)),
            Expression.Constant(null, typeof(ContractDestination)));

        return Expression.Property(destination, destinationProperty);
    }

    private static Expression FuelOption(ParameterExpression trip)
    {
        ParameterExpression fee = Expression.Parameter(typeof(ContractExtraFee), "fee");
        Expression filtered = Expression.Call(
            EnumerableMethod(nameof(Enumerable.Where), typeof(ContractExtraFee)),
            Path(trip, nameof(Trip.Contract), nameof(Contract.ContractExtraFees)),
            Expression.Lambda(
                Expression.Equal(
                    Expression.Property(fee, nameof(ContractExtraFee.ExtraFeeTypeId)),
                    Expression.Constant((int)ExtraFeeTypes.Fuel)),
                fee));

        return Expression.Call(
            EnumerableMethod(nameof(Enumerable.FirstOrDefault), typeof(int?)),
            Expression.Call(
                EnumerableMethod(nameof(Enumerable.Select), typeof(ContractExtraFee), typeof(int?)),
                filtered,
                Expression.Lambda(Expression.Property(fee, nameof(ContractExtraFee.CalculationOption)), fee)));
    }

    private static Expression Waypoints(ParameterExpression trip)
    {
        ParameterExpression waypoint = Expression.Parameter(typeof(TripWaypoint), "waypoint");
        Expression projected = Expression.MemberInit(
            Expression.New(typeof(TripWaypoint)),
            Expression.Bind(typeof(TripWaypoint).GetProperty(nameof(TripWaypoint.Id))!, Expression.Property(waypoint, nameof(TripWaypoint.Id))),
            Expression.Bind(typeof(TripWaypoint).GetProperty(nameof(TripWaypoint.Sequence))!, Expression.Property(waypoint, nameof(TripWaypoint.Sequence))),
            Expression.Bind(typeof(TripWaypoint).GetProperty(nameof(TripWaypoint.StopType))!, Expression.Property(waypoint, nameof(TripWaypoint.StopType))),
            Expression.Bind(typeof(TripWaypoint).GetProperty(nameof(TripWaypoint.AddressName))!, Expression.Property(waypoint, nameof(TripWaypoint.AddressName))),
            Expression.Bind(typeof(TripWaypoint).GetProperty(nameof(TripWaypoint.Status))!, Expression.Property(waypoint, nameof(TripWaypoint.Status))));

        return ToList<TripWaypoint>(Expression.Call(
            EnumerableMethod(nameof(Enumerable.Select), typeof(TripWaypoint), typeof(TripWaypoint)),
            Expression.Property(trip, nameof(Trip.Waypoints)),
            Expression.Lambda(projected, waypoint)));
    }

    private static Expression Recordings(ParameterExpression trip)
    {
        ParameterExpression recording = Expression.Parameter(typeof(Recording), "recording");
        Expression match = Expression.OrElse(
            Expression.Equal(Expression.Property(recording, nameof(Recording.RecordableType)), Expression.Constant(RecordableType.TRIP)),
            Expression.Equal(Expression.Property(recording, nameof(Recording.RecordableType)), Expression.Constant(RecordableType.TRIP_WAYPOINT)));
        Expression filtered = Expression.Call(
            EnumerableMethod(nameof(Enumerable.Where), typeof(Recording)),
            Expression.Property(trip, nameof(Trip.Recordings)),
            Expression.Lambda(match, recording));
        Expression projected = Expression.MemberInit(
            Expression.New(typeof(Recording)),
            Expression.Bind(typeof(Recording).GetProperty(nameof(Recording.Id))!, Expression.Property(recording, nameof(Recording.Id))),
            Expression.Bind(typeof(Recording).GetProperty(nameof(Recording.CreatedAt))!, Expression.Property(recording, nameof(Recording.CreatedAt))),
            Expression.Bind(typeof(Recording).GetProperty(nameof(Recording.RecordableType))!, Expression.Property(recording, nameof(Recording.RecordableType))),
            Expression.Bind(typeof(Recording).GetProperty(nameof(Recording.RecordableEventType))!, Expression.Property(recording, nameof(Recording.RecordableEventType))),
            Expression.Bind(typeof(Recording).GetProperty(nameof(Recording.AddedByModelType))!, Expression.Property(recording, nameof(Recording.AddedByModelType))),
            Expression.Bind(typeof(Recording).GetProperty(nameof(Recording.Data))!, Expression.Property(recording, nameof(Recording.Data))));

        return ToList<Recording>(Expression.Call(
            EnumerableMethod(nameof(Enumerable.Select), typeof(Recording), typeof(Recording)),
            filtered,
            Expression.Lambda(projected, recording)));
    }

    private static Expression ToList<T>(Expression source) =>
        Expression.Call(EnumerableMethod(nameof(Enumerable.ToList), typeof(T)), source);

    private static MethodInfo EnumerableMethod(string name, params Type[] typeArguments)
    {
        return typeof(Enumerable).GetMethods()
            .Single(method =>
                method.Name == name
                && method.IsGenericMethodDefinition
                && method.GetGenericArguments().Length == typeArguments.Length
                && method.GetParameters().Length == (name == nameof(Enumerable.ToList) || name == nameof(Enumerable.FirstOrDefault) ? 1 : 2)
                && (method.GetParameters().Length == 1
                    || method.GetParameters()[1].ParameterType.GetGenericArguments().Length == 2))
            .MakeGenericMethod(typeArguments);
    }
}
