import 'package:json_annotation/json_annotation.dart';

part 'document_dto.g.dart';

@JsonSerializable()
class DocumentDto {
  final int? id;
  final int? entityId;
  final String? nameEn;
  final String? nameAr;
  final bool? required;
  final int? status;
  final dynamic addedBy;
  final String? createdAt;
  final String? updatedAt;

  const DocumentDto({
    this.id,
    this.entityId,
    this.nameEn,
    this.nameAr,
    this.required,
    this.status,
    this.addedBy,
    this.createdAt,
    this.updatedAt,
  });

  factory DocumentDto.fromJson(Map<String, dynamic> json) =>
      _$DocumentDtoFromJson(json);
  Map<String, dynamic> toJson() => _$DocumentDtoToJson(this);
}
