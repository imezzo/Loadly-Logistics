import 'package:equatable/equatable.dart';

class TrackTripsResult extends Equatable {
  final int? id;
  final String? companyName;
  final String? driverName;
  final String? driverMobile;
  final String? vehiclePlateNumber;
  final String? vehicleType;
  final String? vehicleColor;
  final CurrentLocation? currentLocation;
  final List<Waypoint>? waypoints;

  const TrackTripsResult({
    this.id,
    this.companyName,
    this.driverName,
    this.driverMobile,
    this.vehiclePlateNumber,
    this.vehicleType,
    this.vehicleColor,
    this.currentLocation,
    this.waypoints,
  });

  @override
  List<Object?> get props {
    return [
      id,
      companyName,
      driverName,
      driverMobile,
      vehiclePlateNumber,
      vehicleType,
      vehicleColor,
      currentLocation,
      waypoints,
    ];
  }
}

class CurrentLocation extends Equatable {
  final double? latitude;
  final double? longitude;
  final String? recordedAt;

  const CurrentLocation({this.latitude, this.longitude, this.recordedAt});

  @override
  List<Object?> get props => [latitude, longitude, recordedAt];
}

class Waypoint extends Equatable {
  final int? id;
  final int? sequence;
  final int? stopType;
  final int? status;
  final double? latitude;
  final double? longitude;
  final String? addressName;
  final String? googlePlaceId;
  final String? taskDetails;

  const Waypoint({
    this.id,
    this.sequence,
    this.stopType,
    this.status,
    this.latitude,
    this.longitude,
    this.addressName,
    this.googlePlaceId,
    this.taskDetails,
  });

  @override
  List<Object?> get props {
    return [
      id,
      sequence,
      stopType,
      status,
      latitude,
      longitude,
      addressName,
      googlePlaceId,
      taskDetails,
    ];
  }
}
