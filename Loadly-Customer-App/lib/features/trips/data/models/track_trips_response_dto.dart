import 'package:json_annotation/json_annotation.dart';

part 'track_trips_response_dto.g.dart';

@JsonSerializable()
class TrackTripsResponseDto {
  final int? id;
  final String? companyName;
  final String? driverName;
  final String? driverMobile;
  final String? vehiclePlateNumber;
  final String? vehicleType;
  final String? vehicleColor;
  final CurrentLocationDto? currentLocation;
  final List<WaypointDto>? waypoints;

  TrackTripsResponseDto({
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
  factory TrackTripsResponseDto.fromJson(Map<String, dynamic> json) =>
      _$TrackTripsResponseDtoFromJson(json);
  Map<String, dynamic> toJson() => _$TrackTripsResponseDtoToJson(this);
}

@JsonSerializable()
class CurrentLocationDto {
  final double? latitude;
  final double? longitude;
  final String? recordedAt;

  CurrentLocationDto({this.latitude, this.longitude, this.recordedAt});
  factory CurrentLocationDto.fromJson(Map<String, dynamic> json) =>
      _$CurrentLocationDtoFromJson(json);
  Map<String, dynamic> toJson() => _$CurrentLocationDtoToJson(this);
}

@JsonSerializable()
class WaypointDto {
  final int? id;
  final int? sequence;
  final int? stopType;
  final int? status;
  final double? latitude;
  final double? longitude;
  final String? addressName;
  final String? googlePlaceId;
  final String? taskDetails;

  WaypointDto({
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
  factory WaypointDto.fromJson(Map<String, dynamic> json) =>
      _$WaypointDtoFromJson(json);
  Map<String, dynamic> toJson() => _$WaypointDtoToJson(this);
}
