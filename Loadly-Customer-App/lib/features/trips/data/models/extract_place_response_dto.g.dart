// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'extract_place_response_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

ExtractPlaceResponseDto _$ExtractPlaceResponseDtoFromJson(
  Map<String, dynamic> json,
) => ExtractPlaceResponseDto(
  lat: (json['lat'] as num?)?.toDouble(),
  lng: (json['lng'] as num?)?.toDouble(),
  address: json['address'] as String?,
  placeId: json['placeId'] as String?,
);

Map<String, dynamic> _$ExtractPlaceResponseDtoToJson(
  ExtractPlaceResponseDto instance,
) => <String, dynamic>{
  'lat': instance.lat,
  'lng': instance.lng,
  'address': instance.address,
  'placeId': instance.placeId,
};
