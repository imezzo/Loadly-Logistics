import 'dart:io';

import 'package:dartz/dartz.dart';

import '../entities/trip.dart';
import '../failures/trips_failure.dart';
import '../repositories/trips_repository.dart';

class GetDocumentDownloadUrlUseCase {
  GetDocumentDownloadUrlUseCase(this._repository);

  final TripsRepository _repository;

  Future<Either<TripsFailure, String>> call({
    required int tripId,
    required String documentPath,
  }) => _repository.getDocumentDownloadUrl(tripId, documentPath);
}

class StartLoadingUseCase {
  StartLoadingUseCase(this._repository);

  final TripsRepository _repository;

  Future<Either<TripsFailure, Trip>> call({
    required int tripId,
    required int waypointId,
    double? latitude,
    double? longitude,
  }) => _repository.startLoading(
    tripId,
    waypointId,
    latitude: latitude,
    longitude: longitude,
  );
}

class CompleteLoadingUseCase {
  CompleteLoadingUseCase(this._repository);

  final TripsRepository _repository;

  Future<Either<TripsFailure, Trip>> call({
    required int tripId,
    required int waypointId,
    required List<File> receiptNoteImages,
    required List<File> shipmentImages,
    String? note,
    double? latitude,
    double? longitude,
  }) => _repository.completeLoading(
    tripId,
    waypointId,
    receiptNoteImages: receiptNoteImages,
    shipmentImages: shipmentImages,
    note: note,
    latitude: latitude,
    longitude: longitude,
  );
}

class StartUnloadingUseCase {
  StartUnloadingUseCase(this._repository);

  final TripsRepository _repository;

  Future<Either<TripsFailure, Trip>> call({
    required int tripId,
    required int waypointId,
    double? latitude,
    double? longitude,
  }) => _repository.startUnloading(
    tripId,
    waypointId,
    latitude: latitude,
    longitude: longitude,
  );
}

class CompleteUnloadingUseCase {
  CompleteUnloadingUseCase(this._repository);

  final TripsRepository _repository;

  Future<Either<TripsFailure, Trip>> call({
    required int tripId,
    required int waypointId,
    int? odometer,
    List<File>? odometerImages,
    required List<File> unloadingImages,
    required List<File> deliveryNoteImages,
    String? note,
    double? latitude,
    double? longitude,
  }) => _repository.completeUnloading(
    tripId,
    waypointId,
    odometer: odometer,
    odometerImages: odometerImages,
    unloadingImages: unloadingImages,
    deliveryNoteImages: deliveryNoteImages,
    note: note,
    latitude: latitude,
    longitude: longitude,
  );
}

class StartTripUseCase {
  StartTripUseCase(this._repository);

  final TripsRepository _repository;

  Future<Either<TripsFailure, Trip>> call({
    required int tripId,
    String? note,
    double? latitude,
    double? longitude,
  }) => _repository.startTrip(
    tripId,
    note: note,
    latitude: latitude,
    longitude: longitude,
  );
}

class ArriveAtWaypointUseCase {
  ArriveAtWaypointUseCase(this._repository);

  final TripsRepository _repository;

  Future<Either<TripsFailure, Trip>> call({
    required int tripId,
    required int waypointId,
    double? latitude,
    double? longitude,
  }) => _repository.arriveAtWaypoint(
    tripId,
    waypointId,
    latitude: latitude,
    longitude: longitude,
  );
}

class DepartFromWaypointUseCase {
  DepartFromWaypointUseCase(this._repository);

  final TripsRepository _repository;

  Future<Either<TripsFailure, Trip>> call({
    required int tripId,
    required int waypointId,
    List<File>? deliveryNoteImages,
    String? note,
    double? latitude,
    double? longitude,
    int? odometer,
    List<File>? odometerImages,
  }) => _repository.departFromWaypoint(
    tripId,
    waypointId,
    deliveryNoteImages: deliveryNoteImages,
    note: note,
    latitude: latitude,
    longitude: longitude,
    odometer: odometer,
    odometerImages: odometerImages,
  );
}

class EnterCustomsUseCase {
  EnterCustomsUseCase(this._repository);

  final TripsRepository _repository;

  Future<Either<TripsFailure, Trip>> call({
    required int tripId,
    required List<File> customDocuments,
    String? note,
    double? latitude,
    double? longitude,
  }) => _repository.enterCustoms(
    tripId,
    customDocuments: customDocuments,
    note: note,
    latitude: latitude,
    longitude: longitude,
  );
}

class ExitCustomsUseCase {
  ExitCustomsUseCase(this._repository);

  final TripsRepository _repository;

  Future<Either<TripsFailure, Trip>> call({
    required int tripId,
    double? customsTotalAmount,
    required List<File> customsReceipts,
    String? note,
    double? latitude,
    double? longitude,
  }) => _repository.exitCustoms(
    tripId,
    customsTotalAmount: customsTotalAmount,
    customsReceipts: customsReceipts,
    note: note,
    latitude: latitude,
    longitude: longitude,
  );
}

class CompleteTripUseCase {
  CompleteTripUseCase(this._repository);

  final TripsRepository _repository;

  Future<Either<TripsFailure, Trip>> call({
    required int tripId,
    required int odometer,
    required List<File> odometerImages,
    required List<File> tollgateImages,
    required List<File> tollgateWeightImages,
    required String note,
    double? latitude,
    double? longitude,
  }) => _repository.completeTrip(
    tripId,
    odometer: odometer,
    odometerImages: odometerImages,
    tollgateImages: tollgateImages,
    tollgateWeightImages: tollgateWeightImages,
    note: note,
    latitude: latitude,
    longitude: longitude,
  );
}

class ValidateOdometerImageUseCase {
  ValidateOdometerImageUseCase(this._repository);

  final TripsRepository _repository;

  Future<Either<TripsFailure, ValidateOdometerResult>> call({
    required int tripId,
    required File image,
  }) => _repository.validateOdometerImage(tripId, image);
}
