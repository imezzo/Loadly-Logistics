import 'dart:io';

import 'package:dartz/dartz.dart';

import '../entities/trip.dart';
import '../entities/trips_list_result.dart';
import '../failures/trips_failure.dart';

abstract interface class TripsRepository {
  Future<Either<TripsFailure, TripsListResult>> getTrips({
    int? page,
    int? limit,
    int? status,
    String? startDateFrom,
    String? startDateTo,
  });
  Future<Either<TripsFailure, Trip>> getTripDetails(int tripId);
  Future<Either<TripsFailure, String>> getDocumentDownloadUrl(
    int tripId,
    String documentPath,
  );
  Future<Either<TripsFailure, Trip>> startLoading(
    int tripId,
    int waypointId, {
    double? latitude,
    double? longitude,
  });
  Future<Either<TripsFailure, Trip>> completeLoading(
    int tripId,
    int waypointId, {
    required List<File> receiptNoteImages,
    required List<File> shipmentImages,
    String? note,
    double? latitude,
    double? longitude,
  });
  Future<Either<TripsFailure, Trip>> startUnloading(
    int tripId,
    int waypointId, {
    double? latitude,
    double? longitude,
  });
  Future<Either<TripsFailure, Trip>> completeUnloading(
    int tripId,
    int waypointId, {
    int? odometer,
    List<File>? odometerImages,
    required List<File> unloadingImages,
    required List<File> deliveryNoteImages,
    String? note,
    double? latitude,
    double? longitude,
  });
  Future<Either<TripsFailure, Trip>> startTrip(
    int tripId, {
    String? note,
    double? latitude,
    double? longitude,
  });
  Future<Either<TripsFailure, Trip>> arriveAtWaypoint(
    int tripId,
    int waypointId, {
    double? latitude,
    double? longitude,
  });
  Future<Either<TripsFailure, Trip>> departFromWaypoint(
    int tripId,
    int waypointId, {
    List<File>? deliveryNoteImages,
    String? note,
    double? latitude,
    double? longitude,
    int? odometer,
    List<File>? odometerImages,
  });
  Future<Either<TripsFailure, Trip>> enterCustoms(
    int tripId, {
    required List<File> customDocuments,
    String? note,
    double? latitude,
    double? longitude,
  });
  Future<Either<TripsFailure, Trip>> exitCustoms(
    int tripId, {
    required List<File> customsReceipts,
    double? customsTotalAmount,
    String? note,
    double? latitude,
    double? longitude,
  });
  Future<Either<TripsFailure, Trip>> completeTrip(
    int tripId, {
    required int odometer,
    required List<File> odometerImages,
    required List<File> tollgateImages,
    required List<File> tollgateWeightImages,
    required String note,
    double? latitude,
    double? longitude,
  });
  Future<Either<TripsFailure, ValidateOdometerResult>> validateOdometerImage(
    int tripId,
    File image,
  );
}
