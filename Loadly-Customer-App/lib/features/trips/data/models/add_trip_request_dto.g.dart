// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'add_trip_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

AddTripRequestDto _$AddTripRequestDtoFromJson(Map<String, dynamic> json) =>
    AddTripRequestDto(
      contractId: (json['contractId'] as num?)?.toInt(),
      destinationId: (json['destinationId'] as num?)?.toInt(),
      submitRequest: json['submitRequest'] as bool?,
      vehicleTypeId: (json['vehicleTypeId'] as num?)?.toInt(),
      startDate: json['startDate'] as String?,
      endDate: json['endDate'] as String?,
      contractDistanceKm: (json['contractDistanceKm'] as num?)?.toDouble(),
      actualCargoWeightTons: (json['actualCargoWeightTons'] as num?)
          ?.toDouble(),
      cargoPrice: (json['cargoPrice'] as num?)?.toDouble(),
      helpersCount: (json['helpersCount'] as num?)?.toInt(),
      contactPersonName: json['contactPersonName'] as String?,
      contactPersonMobile: json['contactPersonMobile'] as String?,
      waypoints: (json['waypoints'] as List<dynamic>?)
          ?.map((e) => WaypointDto.fromJson(e as Map<String, dynamic>))
          .toList(),
    );

Map<String, dynamic> _$AddTripRequestDtoToJson(AddTripRequestDto instance) =>
    <String, dynamic>{
      'contractId': instance.contractId,
      'destinationId': instance.destinationId,
      'submitRequest': instance.submitRequest,
      'vehicleTypeId': instance.vehicleTypeId,
      'startDate': instance.startDate,
      'endDate': instance.endDate,
      'contractDistanceKm': instance.contractDistanceKm,
      'actualCargoWeightTons': instance.actualCargoWeightTons,
      'cargoPrice': instance.cargoPrice,
      'helpersCount': instance.helpersCount,
      'contactPersonName': instance.contactPersonName,
      'contactPersonMobile': instance.contactPersonMobile,
      'waypoints': instance.waypoints?.map((e) => e.toJson()).toList(),
    };

WaypointDto _$WaypointDtoFromJson(Map<String, dynamic> json) => WaypointDto(
  sequence: (json['sequence'] as num?)?.toInt(),
  stopType: (json['stopType'] as num?)?.toInt(),
  latitude: (json['latitude'] as num?)?.toDouble(),
  longitude: (json['longitude'] as num?)?.toDouble(),
  addressName: json['addressName'] as String?,
  googlePlaceId: json['googlePlaceId'] as String?,
  taskDetails: json['taskDetails'] as String?,
);

Map<String, dynamic> _$WaypointDtoToJson(WaypointDto instance) =>
    <String, dynamic>{
      'sequence': instance.sequence,
      'stopType': instance.stopType,
      'latitude': instance.latitude,
      'longitude': instance.longitude,
      'addressName': instance.addressName,
      'googlePlaceId': instance.googlePlaceId,
      'taskDetails': instance.taskDetails,
    };
