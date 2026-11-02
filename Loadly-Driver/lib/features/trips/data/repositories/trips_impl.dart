import 'dart:io';

import 'package:dartz/dartz.dart';
import 'package:intl/intl.dart';

import '../../../../core/network/api_failure_mapper.dart';
import '../../../../core/network/safe_api_call.dart';
import '../../domain/entities/trip.dart';
import '../../domain/entities/trips_list_result.dart';
import '../../domain/failures/trips_failure.dart';
import '../../domain/repositories/trips_repository.dart';
import '../models/trip_action_params.dart';
import '../models/trip_dto.dart';
import '../sources/remote/trips_remote_data_source.dart';

/// Thrown when an image/document File selected earlier in the flow no
/// longer exists on disk (e.g. Android reclaimed the picker's cache dir)
/// by the time we go to upload it.
class MissingImageFileException implements Exception {
  MissingImageFileException(this.paths);
  final List<String> paths;

  @override
  String toString() => 'MissingImageFileException(paths: $paths)';
}

class TripsImpl implements TripsRepository {
  TripsImpl(this._remote);

  final TripsRemoteDataSource _remote;

  /// Verifies every file in [fileLists] still exists on disk before we
  /// attempt to read/upload it. Throws [MissingImageFileException] if any
  /// are missing, so we never hit a raw PathNotFoundException deep inside
  /// the multipart request.
  Future<void> _verifyFilesExist(List<List<File>?> fileLists) async {
    final missing = <String>[];
    for (final files in fileLists) {
      if (files == null) continue;
      for (final file in files) {
        if (!await file.exists()) {
          missing.add(file.path);
        }
      }
    }
    if (missing.isNotEmpty) {
      throw MissingImageFileException(missing);
    }
  }

  Future<Either<TripsFailure, T>> _run<T>(Future<T> Function() request) {
    return safeApiCall<T, TripsFailure>(
      request: request,
      mapFailure: (details) {
        return switch (details.kind) {
          ApiFailureKind.network => const TripsFailureNetwork(
            message: 'Network error',
          ),
          ApiFailureKind.server ||
          ApiFailureKind.unauthorized => TripsFailureServer(
            message: details.message,
            statusCode: details.statusCode,
          ),
          ApiFailureKind.unknown => TripsFailureUnknown(
            message: details.message,
          ),
        };
      },
      mapUnknownFailure: (error) {
        if (error is MissingImageFileException) {
          return const TripsFailureMissingFiles(
            message: 'trips.image_missing_please_retake',
          );
        }
        return TripsFailureUnknown(message: error.toString());
      },
    );
  }

  Trip _mapDtoToEntity(TripDto dto) {
    final waypoints = dto.waypoints ?? [];
    final sortedWaypoints = List<TripWaypointDto>.from(waypoints)
      ..sort((a, b) => (a.sequence ?? 0).compareTo(b.sequence ?? 0));
    final firstWaypoint = sortedWaypoints.isNotEmpty
        ? sortedWaypoints.first
        : null;
    final lastWaypoint = sortedWaypoints.length > 1
        ? sortedWaypoints.last
        : firstWaypoint;

    final pickupLocation = Location(
      address: firstWaypoint?.addressName ?? '—',
      latitude: firstWaypoint?.latitude,
      longitude: firstWaypoint?.longitude,
    );
    final deliveryLocation = Location(
      address: lastWaypoint?.addressName ?? '—',
      latitude: lastWaypoint?.latitude,
      longitude: lastWaypoint?.longitude,
    );

    final (startDate, startTime) = _parseStartDateTime(dto.startDate);

    final title =
        dto.contract?.nameEn ??
        dto.contract?.referenceNumber ??
        'Trip #${dto.id}';

    final stopPoints = sortedWaypoints.length > 2
        ? sortedWaypoints
              .sublist(1, sortedWaypoints.length - 1)
              .map(
                (wp) => StopPoint(
                  id: wp.id,
                  name: wp.addressName ?? 'Stop',
                  address: wp.addressName ?? '—',
                  latitude: wp.latitude,
                  longitude: wp.longitude,
                  order: wp.sequence,
                  stopType: wp.stopType,
                  status: wp.status,
                ),
              )
              .toList()
        : null;
    final waypointEntities = sortedWaypoints
        .map(
          (wp) => TripWaypoint(
            id: wp.id,
            sequence: wp.sequence ?? 0,
            stopType: wp.stopType ?? 0,
            status: wp.status ?? 0,
            address: wp.addressName ?? '—',
            latitude: wp.latitude,
            longitude: wp.longitude,
            taskDetails: wp.taskDetails,
          ),
        )
        .toList();

    final documents = (dto.documents ?? [])
        .map(
          (d) => TripDocument(
            type: d.type,
            typeName: d.typeName,
            paths: d.paths,
            waypointId: d.waypointId,
            createdAt: d.createdAt,
          ),
        )
        .toList();

    final vehicleDetails = dto.vehicle != null
        ? VehicleDetails(
            licensePlate: dto.vehicle!.plateNumber ?? '—',
            description:
                dto.vehicle!.vehicleTypeName ??
                dto.vehicle!.chassisNumber ??
                '—',
            color: null,
            imageUrl: null,
          )
        : null;

    final clientDetails =
        (dto.contactPersonName != null ||
            dto.contactPersonMobile != null ||
            dto.company != null)
        ? ClientDetails(
            name: dto.company?.nameEn ?? dto.contactPersonName ?? '—',
            phoneNumber: dto.contactPersonMobile,
            responsiblePersonName: dto.contactPersonName,
          )
        : null;

    final route = sortedWaypoints.isNotEmpty
        ? Route(
            polyline: null,
            waypoints: sortedWaypoints
                .map(
                  (wp) => Waypoint(
                    latitude: wp.latitude ?? 0,
                    longitude: wp.longitude ?? 0,
                  ),
                )
                .toList(),
          )
        : null;

    final startLocation = dto.startLocation != null
        ? TripStartLocation(
            latitude: dto.startLocation!.latitude,
            longitude: dto.startLocation!.longitude,
          )
        : null;

    return Trip(
      cutoffDuration: dto.cutoffDuration,
      id: dto.id,
      title: title,
      description: null,
      startDateTime: dto.startDate,
      startDate: startDate,
      startTime: startTime,
      pickupLocation: pickupLocation,
      deliveryLocation: deliveryLocation,
      shipmentDetails: null,
      vehicleDetails: vehicleDetails,
      stopPoints: stopPoints,
      waypoints: waypointEntities,
      documents: documents,
      clientDetails: clientDetails,
      status: dto.status?.toString(),
      statusCode: dto.status,
      customsStatus: dto.customsStatus,
      customsStatusName: dto.customsStatusName,
      route: route,
      companyNameEn: dto.company?.nameEn,
      companyNameAr: dto.company?.nameAr,
      startLocation: startLocation,
      completionTime: dto.completionTime,
      lifecycleActor: dto.lifecycleActor,
      lifecycleActorName: dto.lifecycleActorName,
    );
  }

  TripLocation? _location(double? lat, double? lng) {
    if (lat != null && lng != null) {
      return TripLocation(latitude: lat, longitude: lng);
    }
    return null;
  }

  (String, String) _parseStartDateTime(String isoString) {
    try {
      // Parse the ISO string (handles UTC offsets and Z) and convert to local time
      final dt = DateTime.parse(isoString).toLocal();

      // Format date as yyyy-MM-dd (local time)
      final datePart = DateFormat('yyyy-MM-dd').format(dt);

      // Format time as HH:mm (local time)
      final timePart = DateFormat('h:mm a').format(dt);

      return (datePart, timePart);
    } catch (_) {
      // Fallback: if parsing fails, keep original string for date and default time
      return (isoString, '00:00');
    }
  }

  @override
  Future<Either<TripsFailure, TripsListResult>> getTrips({
    int? page,
    int? limit,
    int? status,
    String? startDateFrom,
    String? startDateTo,
  }) async {
    return _run(() async {
      final response = await _remote.getTrips(
        page: page,
        limit: limit,
        status: status,
        startDateFrom: startDateFrom,
        startDateTo: startDateTo,
      );
      final trips = response.trips.map(_mapDtoToEntity).toList();
      return TripsListResult(
        trips: trips,
        total: response.total,
        page: response.page ?? 1,
        pageSize: response.limit ?? 10,
      );
    });
  }

  @override
  Future<Either<TripsFailure, Trip>> getTripDetails(int tripId) async {
    return _run(() async {
      final dto = await _remote.getTripDetails(tripId);
      return _mapDtoToEntity(dto);
    });
  }

  Future<Either<TripsFailure, T>> _handleTripAction<T>(
    Future<T> Function() action,
  ) async {
    return _run(action);
  }

  @override
  Future<Either<TripsFailure, String>> getDocumentDownloadUrl(
    int tripId,
    String documentPath,
  ) async {
    return _handleTripAction(
      () => _remote.getDocumentDownloadUrl(tripId, documentPath),
    );
  }

  @override
  Future<Either<TripsFailure, Trip>> startLoading(
    int tripId,
    int waypointId, {
    double? latitude,
    double? longitude,
  }) async {
    return _handleTripAction(() async {
      final dto = await _remote.startLoading(
        tripId,
        waypointId,
        _location(latitude, longitude),
      );
      return _mapDtoToEntity(dto);
    });
  }

  @override
  Future<Either<TripsFailure, Trip>> completeLoading(
    int tripId,
    int waypointId, {
    required List<File> receiptNoteImages,
    required List<File> shipmentImages,
    String? note,
    double? latitude,
    double? longitude,
  }) async {
    return _handleTripAction(() async {
      await _verifyFilesExist([receiptNoteImages, shipmentImages]);
      final dto = await _remote.completeLoading(
        tripId,
        waypointId,
        CompleteLoadingParams(
          receiptNoteImages: receiptNoteImages,
          shipmentImages: shipmentImages,
          note: note,
          location: _location(latitude, longitude),
        ),
      );
      return _mapDtoToEntity(dto);
    });
  }

  @override
  Future<Either<TripsFailure, Trip>> startUnloading(
    int tripId,
    int waypointId, {
    double? latitude,
    double? longitude,
  }) async {
    return _handleTripAction(() async {
      final dto = await _remote.startUnloading(
        tripId,
        waypointId,
        _location(latitude, longitude),
      );
      return _mapDtoToEntity(dto);
    });
  }

  @override
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
  }) async {
    return _handleTripAction(() async {
      await _verifyFilesExist([
        unloadingImages,
        deliveryNoteImages,
        ?odometerImages,
      ]);
      final dto = await _remote.completeUnloading(
        tripId,
        waypointId,
        CompleteUnloadingParams(
          odometer: odometer,
          odometerImages: odometerImages,
          unloadingImages: unloadingImages,
          deliveryNoteImages: deliveryNoteImages,
          note: note,
          location: _location(latitude, longitude),
        ),
      );
      return _mapDtoToEntity(dto);
    });
  }

  @override
  Future<Either<TripsFailure, Trip>> startTrip(
    int tripId, {
    String? note,
    double? latitude,
    double? longitude,
  }) async {
    return _handleTripAction(() async {
      final dto = await _remote.startTrip(
        tripId,
        StartTripParams(note: note, location: _location(latitude, longitude)),
      );
      return _mapDtoToEntity(dto);
    });
  }

  @override
  Future<Either<TripsFailure, Trip>> arriveAtWaypoint(
    int tripId,
    int waypointId, {
    double? latitude,
    double? longitude,
  }) async {
    return _handleTripAction(() async {
      final dto = await _remote.arriveAtWaypoint(
        tripId,
        waypointId,
        _location(latitude, longitude),
      );
      return _mapDtoToEntity(dto);
    });
  }

  @override
  Future<Either<TripsFailure, Trip>> departFromWaypoint(
    int tripId,
    int waypointId, {
    List<File>? deliveryNoteImages,
    String? note,
    double? latitude,
    double? longitude,
    int? odometer,
    List<File>? odometerImages,
  }) async {
    return _handleTripAction(() async {
      await _verifyFilesExist([deliveryNoteImages, odometerImages]);
      final dto = await _remote.departFromWaypoint(
        tripId,
        waypointId,
        DepartWaypointParams(
          deliveryNoteImages: deliveryNoteImages,
          note: note,
          location: _location(latitude, longitude),
          odometer: odometer,
          odometerImages: odometerImages,
        ),
      );
      return _mapDtoToEntity(dto);
    });
  }

  @override
  Future<Either<TripsFailure, Trip>> enterCustoms(
    int tripId, {
    required List<File> customDocuments,
    String? note,
    double? latitude,
    double? longitude,
  }) async {
    return _handleTripAction(() async {
      await _verifyFilesExist([customDocuments]);
      final dto = await _remote.enterCustoms(
        tripId,
        EnterCustomsParams(
          customDocuments: customDocuments,
          note: note,
          location: _location(latitude, longitude),
        ),
      );
      return _mapDtoToEntity(dto);
    });
  }

  @override
  Future<Either<TripsFailure, Trip>> exitCustoms(
    int tripId, {
    required List<File> customsReceipts,
    double? customsTotalAmount,
    String? note,
    double? latitude,
    double? longitude,
  }) async {
    return _handleTripAction(() async {
      await _verifyFilesExist([customsReceipts]);
      final dto = await _remote.exitCustoms(
        tripId,
        ExitCustomsParams(
          customsTotalAmount: customsTotalAmount,
          customsReceipts: customsReceipts,
          note: note,
          location: _location(latitude, longitude),
        ),
      );
      return _mapDtoToEntity(dto);
    });
  }

  @override
  Future<Either<TripsFailure, Trip>> completeTrip(
    int tripId, {
    required int odometer,
    required List<File> odometerImages,
    required List<File> tollgateImages,
    required List<File> tollgateWeightImages,
    required String note,
    double? latitude,
    double? longitude,
  }) async {
    return _handleTripAction(() async {
      await _verifyFilesExist([odometerImages]);
      final dto = await _remote.completeTrip(
        tripId,
        CompleteTripParams(
          odometer: odometer,
          odometerImages: odometerImages,
          tollgateImages: tollgateImages,
          tollgateWeightImages: tollgateWeightImages,
          note: note,
          location: _location(latitude, longitude),
        ),
      );
      return _mapDtoToEntity(dto);
    });
  }

  @override
  Future<Either<TripsFailure, ValidateOdometerResult>> validateOdometerImage(
    int tripId,
    File image,
  ) async {
    return _handleTripAction(() async {
      await _verifyFilesExist([
        [image],
      ]);
      final odometer = await _remote.validateOdometerImage(tripId, image);
      return ValidateOdometerResult(
        message: odometer.message,
        scanLogId: odometer.scanLogId,
        odometerReading: odometer.odometerReading,
        unit: odometer.unit,
        isDigital: odometer.isDigital,
        readingQuality: odometer.readingQuality,
      );
    });
  }
}
