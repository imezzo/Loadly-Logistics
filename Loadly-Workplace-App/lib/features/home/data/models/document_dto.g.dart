// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'document_dto.dart';

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

DocumentDto _$DocumentDtoFromJson(Map<String, dynamic> json) => DocumentDto(
  id: (json['id'] as num?)?.toInt(),
  entityId: (json['entityId'] as num?)?.toInt(),
  nameEn: json['nameEn'] as String?,
  nameAr: json['nameAr'] as String?,
  required: json['required'] as bool?,
  status: (json['status'] as num?)?.toInt(),
  addedBy: json['addedBy'],
  createdAt: json['createdAt'] as String?,
  updatedAt: json['updatedAt'] as String?,
);

Map<String, dynamic> _$DocumentDtoToJson(DocumentDto instance) =>
    <String, dynamic>{
      'id': instance.id,
      'entityId': instance.entityId,
      'nameEn': instance.nameEn,
      'nameAr': instance.nameAr,
      'required': instance.required,
      'status': instance.status,
      'addedBy': instance.addedBy,
      'createdAt': instance.createdAt,
      'updatedAt': instance.updatedAt,
    };
