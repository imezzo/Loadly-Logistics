// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'dart:io';

/// Optional location for trip POST actions (X-Latitude, X-Longitude headers).
class TripLocation {
  const TripLocation({required this.latitude, required this.longitude});

  final double latitude;
  final double longitude;

  Map<String, String> toHeaders() => {
    'X-Latitude': latitude.toString(),
    'X-Longitude': longitude.toString(),
  };
}

/// Params for Complete Unloading at dropoff.
class CompleteUnloadingParams {
  const CompleteUnloadingParams({
    this.odometer,
    this.odometerImages,
    required this.unloadingImages,
    required this.deliveryNoteImages,
    this.note,
    this.location,
  });

  /// Required only for last dropoff when a depot waypoint follows.
  final int? odometer;
  final List<File>? odometerImages;
  final List<File> unloadingImages;

  /// Required delivery note images at dropoff.
  final List<File> deliveryNoteImages;
  final String? note;
  final TripLocation? location;
}

/// Params for Complete Loading Cargo.
class CompleteLoadingParams {
  const CompleteLoadingParams({
    required this.receiptNoteImages,
    required this.shipmentImages,
    this.note,
    this.location,
  });

  final List<File> receiptNoteImages;
  final List<File> shipmentImages;
  final String? note;
  final TripLocation? location;
}

/// Params for Start Trip.
class StartTripParams {
  const StartTripParams({this.note, this.location});

  final String? note;
  final TripLocation? location;
}

/// Params for Depart from Waypoint.
class DepartWaypointParams {
  const DepartWaypointParams({
    this.deliveryNoteImages,
    this.note,
    this.location,
    this.odometer,
    this.odometerImages,
  });

  /// Optional delivery note images when departing (e.g. if required by a waypoint).
  final List<File>? deliveryNoteImages;
  final String? note;
  final TripLocation? location;

  /// Required when departing from first pickup (start-of-trip odometer reading).
  final int? odometer;
  final List<File>? odometerImages;
}

/// Params for Enter Customs.
class EnterCustomsParams {
  const EnterCustomsParams({
    required this.customDocuments,
    this.note,
    this.location,
  });

  final List<File> customDocuments;
  final String? note;
  final TripLocation? location;
}

/// Params for Exit Customs.
class ExitCustomsParams {
  const ExitCustomsParams({
    required this.customsReceipts,
    required this.customsTotalAmount,
    this.note,
    this.location,
  });

  final List<File> customsReceipts;
  final double? customsTotalAmount;
  final String? note;
  final TripLocation? location;
}

/// Params for Complete Trip.
class CompleteTripParams {
  const CompleteTripParams({
    required this.odometer,
    required this.odometerImages,
    required this.tollgateImages,
    required this.tollgateWeightImages,
    required this.note,
    this.location,
  });

  final int odometer;
  final List<File> odometerImages;
  final List<File> tollgateImages;
  final List<File> tollgateWeightImages;
  final String note;
  final TripLocation? location;
}
