import 'package:dartz/dartz.dart';

import '../entities/extract_place_result.dart';
import '../entities/lookup.dart';
import '../entities/track_trips_result.dart' as track;
import '../entities/trip.dart' hide Contract;
import '../entities/destination.dart';
import '../failures/trips_failure.dart';

abstract interface class TripsRepository {
  Future<Either<TripsFailure, TripsList>> getTrips({
    int? page,
    int? limit,
    int? status,
    String? search,
    int? vehicleTypeId,
    int? contractId,
    String? startDate,
    String? endDate,
  });
  Future<Either<TripsFailure, Trip>> getTripDetails(int tripId);
  Future<Either<TripsFailure, List<Lookup>>> getContractReferenceNumbers();
  Future<Either<TripsFailure, List<Lookup>>> getVehicleTypes({int? contractId});
  Future<Either<TripsFailure, int>> getConsumptionType({
    required int contractId,
  });
  Future<Either<TripsFailure, List<Lookup>>> getContractDestinations({
    required int contractId,
  });
  Future<Either<TripsFailure, Destination>> getContractDestination({
    required int destinationId,
  });
  Future<Either<TripsFailure, ExtractPlaceResult>> extractPlace(String url);
  Future<Either<TripsFailure, void>> addTrip({
    int? contractId,
    int? destinationId,
    bool? submitRequest,
    int? vehicleTypeId,
    String? startDate,
    String? endDate,
    double? contractDistanceKm,
    double? actualCargoWeightTons,
    double? cargoPrice,
    int? helpersCount,
    String? contactPersonName,
    String? contactPersonMobile,
    List<Waypoint>? waypoints,
  });
  Future<Either<TripsFailure, void>> updateTrip({
    int? tripId,
    int? contractId,
    int? destinationId,
    bool? submitRequest,
    int? vehicleTypeId,
    String? startDate,
    String? endDate,
    double? contractDistanceKm,
    double? actualCargoWeightTons,
    double? cargoPrice,
    int? helpersCount,
    String? contactPersonName,
    String? contactPersonMobile,
    List<Waypoint>? waypoints,
  });
  Future<Either<TripsFailure, void>> deleteTrip(int tripId);
  Future<Either<TripsFailure, List<track.TrackTripsResult>>> trackTrips();
}
