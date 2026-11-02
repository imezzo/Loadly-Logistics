// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'lookup_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

LookupDto _$LookupDtoFromJson(Map<String, dynamic> json) => LookupDto(
  id: (json['id'] as num?)?.toInt(),
  value: json['value'] as String?,
);

Map<String, dynamic> _$LookupDtoToJson(LookupDto instance) => <String, dynamic>{
  'id': instance.id,
  'value': instance.value,
};
