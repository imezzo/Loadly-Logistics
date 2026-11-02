import 'package:json_annotation/json_annotation.dart';

part 'update_trip_request_dto.g.dart';

@JsonSerializable()
class UpdateTripRequestDto {
  final int? tripId;
  final int? contractId;
  final int? destinationId;
  final bool? submitRequest;
  final int? vehicleTypeId;
  final String? startDate;
  final String? endDate;
  final double? contractDistanceKm;
  final int? helpersCount;
  final String? contactPersonName;
  final String? contactPersonMobile;
  final List<WaypointDto>? waypoints;

  UpdateTripRequestDto({
    this.tripId,
    this.contractId,
    this.destinationId,
    this.submitRequest,
    this.vehicleTypeId,
    this.startDate,
    this.endDate,
    this.contractDistanceKm,
    this.helpersCount,
    this.contactPersonName,
    this.contactPersonMobile,
    this.waypoints,
  });
  factory UpdateTripRequestDto.fromJson(Map<String, dynamic> json) =>
      _$UpdateTripRequestDtoFromJson(json);
  Map<String, dynamic> toJson() => _$UpdateTripRequestDtoToJson(this);
}

@JsonSerializable()
class WaypointDto {
  final int? sequence;
  final int? stopType;
  final double? latitude;
  final double? longitude;
  final String? addressName;
  final String? googlePlaceId;
  final String? taskDetails;

  WaypointDto({
    this.sequence,
    this.stopType,
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
