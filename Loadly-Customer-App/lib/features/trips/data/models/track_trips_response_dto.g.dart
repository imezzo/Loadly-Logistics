// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'track_trips_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

TrackTripsResponseDto _$TrackTripsResponseDtoFromJson(
  Map<String, dynamic> json,
) => TrackTripsResponseDto(
  id: (json['id'] as num?)?.toInt(),
  companyName: json['companyName'] as String?,
  driverName: json['driverName'] as String?,
  driverMobile: json['driverMobile'] as String?,
  vehiclePlateNumber: json['vehiclePlateNumber'] as String?,
  vehicleType: json['vehicleType'] as String?,
  vehicleColor: json['vehicleColor'] as String?,
  currentLocation: json['currentLocation'] == null
      ? null
      : CurrentLocationDto.fromJson(
          json['currentLocation'] as Map<String, dynamic>,
        ),
  waypoints: (json['waypoints'] as List<dynamic>?)
      ?.map((e) => WaypointDto.fromJson(e as Map<String, dynamic>))
      .toList(),
);

Map<String, dynamic> _$TrackTripsResponseDtoToJson(
  TrackTripsResponseDto instance,
) => <String, dynamic>{
  'id': instance.id,
  'companyName': instance.companyName,
  'driverName': instance.driverName,
  'driverMobile': instance.driverMobile,
  'vehiclePlateNumber': instance.vehiclePlateNumber,
  'vehicleType': instance.vehicleType,
  'vehicleColor': instance.vehicleColor,
  'currentLocation': instance.currentLocation?.toJson(),
  'waypoints': instance.waypoints?.map((e) => e.toJson()).toList(),
};

CurrentLocationDto _$CurrentLocationDtoFromJson(Map<String, dynamic> json) =>
    CurrentLocationDto(
      latitude: (json['latitude'] as num?)?.toDouble(),
      longitude: (json['longitude'] as num?)?.toDouble(),
      recordedAt: json['recordedAt'] as String?,
    );

Map<String, dynamic> _$CurrentLocationDtoToJson(CurrentLocationDto instance) =>
    <String, dynamic>{
      'latitude': instance.latitude,
      'longitude': instance.longitude,
      'recordedAt': instance.recordedAt,
    };

WaypointDto _$WaypointDtoFromJson(Map<String, dynamic> json) => WaypointDto(
  id: (json['id'] as num?)?.toInt(),
  sequence: (json['sequence'] as num?)?.toInt(),
  stopType: (json['stopType'] as num?)?.toInt(),
  status: (json['status'] as num?)?.toInt(),
  latitude: (json['latitude'] as num?)?.toDouble(),
  longitude: (json['longitude'] as num?)?.toDouble(),
  addressName: json['addressName'] as String?,
  googlePlaceId: json['googlePlaceId'] as String?,
  taskDetails: json['taskDetails'] as String?,
);

Map<String, dynamic> _$WaypointDtoToJson(WaypointDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'sequence': instance.sequence,
      'stopType': instance.stopType,
      'status': instance.status,
      'latitude': instance.latitude,
      'longitude': instance.longitude,
      'addressName': instance.addressName,
      'googlePlaceId': instance.googlePlaceId,
      'taskDetails': instance.taskDetails,
    };
