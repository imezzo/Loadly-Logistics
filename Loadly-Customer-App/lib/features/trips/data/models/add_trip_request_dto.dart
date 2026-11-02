import 'package:json_annotation/json_annotation.dart';

part 'add_trip_request_dto.g.dart';

@JsonSerializable()
class AddTripRequestDto {
  final int? contractId;
  final int? destinationId;
  final bool? submitRequest;
  final int? vehicleTypeId;
  final String? startDate;
  final String? endDate;
  final double? contractDistanceKm;
  final double? actualCargoWeightTons;
  final double? cargoPrice;
  final int? helpersCount;
  final String? contactPersonName;
  final String? contactPersonMobile;
  final List<WaypointDto>? waypoints;

  AddTripRequestDto({
    this.contractId,
    this.destinationId,
    this.submitRequest,
    this.vehicleTypeId,
    this.startDate,
    this.endDate,
    this.contractDistanceKm,
    this.actualCargoWeightTons,
    this.cargoPrice,
    this.helpersCount,
    this.contactPersonName,
    this.contactPersonMobile,
    this.waypoints,
  });
  factory AddTripRequestDto.fromJson(Map<String, dynamic> json) =>
      _$AddTripRequestDtoFromJson(json);
  Map<String, dynamic> toJson() => _$AddTripRequestDtoToJson(this);
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
