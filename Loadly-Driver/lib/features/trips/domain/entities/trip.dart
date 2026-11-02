// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:equatable/equatable.dart';

/// Trip waypoint status codes (API guide).
const int kTripWaypointPendingStatus = 0;
const int kTripWaypointUnloadingCompletedStatus = 8;

/// Driver trip lifecycle status (API). 7 is treated as completed (e.g. reviewed); 8 is cancelled.
const int kTripStatusCodeInProgress = 5;
const int kTripStatusCodeCompleted = 6;
const int kTripStatusCodeCompletedReviewed = 7;
const int kTripStatusCodeCancelled = 8;

bool tripStatusIsCompleted(int? code) =>
    code == kTripStatusCodeCompleted ||
    code == kTripStatusCodeCompletedReviewed;

bool tripStatusIsCancelled(int? code) => code == kTripStatusCodeCancelled;

bool tripStatusIsTerminal(int? code) =>
    tripStatusIsCompleted(code) || tripStatusIsCancelled(code);

/// API lifecycle actor when trip was completed (e.g. 1 = driver).
const int kTripLifecycleActorDriver = 1;

/// Upload local GPS route to backend only when the driver completed the trip.
bool tripGpsRouteSyncAllowed(int? lifecycleActor) =>
    lifecycleActor == kTripLifecycleActorDriver;

class Trip extends Equatable {
  const Trip({
    required this.cutoffDuration,
    required this.id,
    required this.title,
    this.description,
    required this.startDateTime,
    required this.startDate,
    required this.startTime,
    required this.pickupLocation,
    required this.deliveryLocation,
    this.shipmentDetails,
    this.vehicleDetails,
    this.stopPoints,
    this.waypoints,
    this.documents,
    this.clientDetails,
    this.status,
    this.statusCode,
    this.customsStatus,
    this.customsStatusName,
    this.route,
    this.companyNameEn,
    this.companyNameAr,
    this.startLocation,
    this.completionTime,
    this.lifecycleActor,
    this.lifecycleActorName,
  });
  final int cutoffDuration;
  final int id;
  final String title;
  final String? description;
  final String startDateTime;
  final String startDate;
  final String startTime;
  final Location pickupLocation;
  final Location deliveryLocation;
  final ShipmentDetails? shipmentDetails;
  final VehicleDetails? vehicleDetails;
  final List<StopPoint>? stopPoints;
  final List<TripWaypoint>? waypoints;
  final List<TripDocument>? documents;
  final ClientDetails? clientDetails;
  final String? status;

  /// Raw API status code (e.g. 2 = Scheduled, 5 = InProgress). Used for filtering tabs.
  final int? statusCode;
  final int? customsStatus;
  final String? customsStatusName;
  final Route? route;
  final String? companyNameEn;
  final String? companyNameAr;

  /// Recorded location when trip was started (from API startLocation).
  final TripStartLocation? startLocation;

  /// When the trip reached a terminal lifecycle state (UTC ISO-8601 from API).
  final String? completionTime;

  /// Who completed/closed the trip (API enum, e.g. 1 = driver).
  final int? lifecycleActor;

  /// Localized display name for [lifecycleActor].
  final String? lifecycleActorName;

  @override
  List<Object?> get props => [
    id,
    title,
    description,
    startDateTime,
    startDate,
    startTime,
    pickupLocation,
    deliveryLocation,
    shipmentDetails,
    vehicleDetails,
    stopPoints,
    waypoints,
    documents,
    clientDetails,
    status,
    statusCode,
    customsStatus,
    customsStatusName,
    route,
    companyNameEn,
    companyNameAr,
    startLocation,
    completionTime,
    lifecycleActor,
    lifecycleActorName,
  ];
}

/// Recorded start location when trip was started.
class TripStartLocation extends Equatable {
  const TripStartLocation({required this.latitude, required this.longitude});

  final double latitude;
  final double longitude;

  @override
  List<Object?> get props => [latitude, longitude];
}

/// Trip document (receipt, shipment, delivery note, odometer, tollgate).
class TripDocument extends Equatable {
  const TripDocument({
    required this.type,
    this.typeName,
    this.paths,
    this.waypointId,
    this.createdAt,
  });

  final String type;
  final String? typeName;
  final List<String>? paths;
  final int? waypointId;
  final String? createdAt;

  @override
  List<Object?> get props => [type, typeName, paths, waypointId, createdAt];
}

class Location extends Equatable {
  const Location({required this.address, this.latitude, this.longitude});

  final String address;
  final double? latitude;
  final double? longitude;

  @override
  List<Object?> get props => [address, latitude, longitude];
}

class ShipmentDetails extends Equatable {
  const ShipmentDetails({
    required this.weight,
    required this.type,
    required this.duration,
    this.dimensions,
  });

  final double weight;
  final String type;
  final int duration;
  final String? dimensions;

  @override
  List<Object?> get props => [weight, type, duration, dimensions];
}

class VehicleDetails extends Equatable {
  const VehicleDetails({
    required this.licensePlate,
    required this.description,
    this.color,
    this.imageUrl,
  });

  final String licensePlate;
  final String description;
  final String? color;
  final String? imageUrl;

  @override
  List<Object?> get props => [licensePlate, description, color, imageUrl];
}

class StopPoint extends Equatable {
  const StopPoint({
    required this.id,
    required this.name,
    required this.address,
    this.latitude,
    this.longitude,
    this.order,
    this.stopType,
    this.status,
  });

  final int id;
  final String name;
  final String address;
  final double? latitude;
  final double? longitude;
  final int? order;

  /// 0 = Pickup, 1 = Dropoff, 2 = Depot
  final int? stopType;

  /// Uses [kTripWaypointPendingStatus]..[kTripWaypointUnloadingCompletedStatus].
  final int? status;

  @override
  List<Object?> get props => [
    id,
    name,
    address,
    latitude,
    longitude,
    order,
    stopType,
    status,
  ];
}

/// Full waypoint details for trip flow (id, sequence, stopType, status).
class TripWaypoint extends Equatable {
  const TripWaypoint({
    required this.id,
    required this.sequence,
    required this.stopType,
    required this.status,
    required this.address,
    this.latitude,
    this.longitude,
    this.taskDetails,
  });

  final int id;
  final int sequence;

  /// 0 = Pickup, 1 = Dropoff, 2 = Depot
  final int stopType;

  /// Uses [kTripWaypointPendingStatus]..[kTripWaypointUnloadingCompletedStatus].
  final int status;
  final String address;
  final double? latitude;
  final double? longitude;
  final String? taskDetails;

  bool get isPickup => stopType == 0;
  bool get isDropoff => stopType == 1;
  bool get isDepot => stopType == 2;

  @override
  List<Object?> get props => [
    id,
    sequence,
    stopType,
    status,
    address,
    latitude,
    longitude,
    taskDetails,
  ];
}

class ClientDetails extends Equatable {
  const ClientDetails({
    required this.name,
    this.phoneNumber,
    this.responsiblePersonName,
  });

  final String name;
  final String? phoneNumber;
  final String? responsiblePersonName;

  @override
  List<Object?> get props => [name, phoneNumber, responsiblePersonName];
}

class Route extends Equatable {
  const Route({this.polyline, this.waypoints});

  final String? polyline;
  final List<Waypoint>? waypoints;

  @override
  List<Object?> get props => [polyline, waypoints];
}

class Waypoint extends Equatable {
  const Waypoint({required this.latitude, required this.longitude});

  final double latitude;
  final double longitude;

  @override
  List<Object?> get props => [latitude, longitude];
}

class ValidateOdometerResult extends Equatable {
  final String? message;
  final int? scanLogId;
  final int? odometerReading;
  final String? unit;
  final bool? isDigital;
  final String? readingQuality;

  const ValidateOdometerResult({
    this.message,
    this.scanLogId,
    this.odometerReading,
    this.unit,
    this.isDigital,
    this.readingQuality,
  });

  @override
  List<Object?> get props {
    return [
      message,
      scanLogId,
      odometerReading,
      unit,
      isDigital,
      readingQuality,
    ];
  }
}
