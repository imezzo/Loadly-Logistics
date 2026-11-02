// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'destination_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DestinationDto _$DestinationDtoFromJson(Map<String, dynamic> json) =>
    DestinationDto(
      contractId: (json['contractId'] as num?)?.toInt(),
      id: (json['id'] as num?)?.toInt(),
      start: json['start'] == null
          ? null
          : StartDto.fromJson(json['start'] as Map<String, dynamic>),
      end: json['end'] == null
          ? null
          : EndDto.fromJson(json['end'] as Map<String, dynamic>),
      nameAr: json['nameAr'] as String?,
      nameEn: json['nameEn'] as String?,
      expectedKms: (json['expectedKms'] as num?)?.toInt(),
      numberOfDays: (json['numberOfDays'] as num?)?.toInt(),
      pricePerTon: json['pricePerTon'],
      fixedTripPrice: json['fixedTripPrice'],
      description: json['description'],
      status: (json['status'] as num?)?.toInt(),
      statusName: json['statusName'] as String?,
    );

Map<String, dynamic> _$DestinationDtoToJson(DestinationDto instance) =>
    <String, dynamic>{
      'contractId': instance.contractId,
      'id': instance.id,
      'start': instance.start?.toJson(),
      'end': instance.end?.toJson(),
      'nameAr': instance.nameAr,
      'nameEn': instance.nameEn,
      'expectedKms': instance.expectedKms,
      'numberOfDays': instance.numberOfDays,
      'pricePerTon': instance.pricePerTon,
      'fixedTripPrice': instance.fixedTripPrice,
      'description': instance.description,
      'status': instance.status,
      'statusName': instance.statusName,
    };

StartDto _$StartDtoFromJson(Map<String, dynamic> json) => StartDto(
  id: (json['id'] as num?)?.toInt(),
  googlePlaceId: json['googlePlaceId'] as String?,
  latitude: (json['latitude'] as num?)?.toDouble(),
  longitude: (json['longitude'] as num?)?.toDouble(),
  addressName: json['addressName'] as String?,
);

Map<String, dynamic> _$StartDtoToJson(StartDto instance) => <String, dynamic>{
  'id': instance.id,
  'googlePlaceId': instance.googlePlaceId,
  'latitude': instance.latitude,
  'longitude': instance.longitude,
  'addressName': instance.addressName,
};

EndDto _$EndDtoFromJson(Map<String, dynamic> json) => EndDto(
  id: (json['id'] as num?)?.toInt(),
  googlePlaceId: json['googlePlaceId'] as String?,
  latitude: (json['latitude'] as num?)?.toDouble(),
  longitude: (json['longitude'] as num?)?.toDouble(),
  addressName: json['addressName'] as String?,
);

Map<String, dynamic> _$EndDtoToJson(EndDto instance) => <String, dynamic>{
  'id': instance.id,
  'googlePlaceId': instance.googlePlaceId,
  'latitude': instance.latitude,
  'longitude': instance.longitude,
  'addressName': instance.addressName,
};
