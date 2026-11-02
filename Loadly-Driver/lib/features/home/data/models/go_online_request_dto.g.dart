// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'go_online_request_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

GoOnlineRequestDto _$GoOnlineRequestDtoFromJson(Map<String, dynamic> json) =>
    GoOnlineRequestDto(
      latitude: (json['latitude'] as num).toDouble(),
      longitude: (json['longitude'] as num).toDouble(),
    );

Map<String, dynamic> _$GoOnlineRequestDtoToJson(GoOnlineRequestDto instance) =>
    <String, dynamic>{
      'latitude': instance.latitude,
      'longitude': instance.longitude,
    };
