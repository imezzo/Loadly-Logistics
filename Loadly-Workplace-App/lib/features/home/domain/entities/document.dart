import 'package:equatable/equatable.dart';

class Document extends Equatable {
  final int? id;
  final int? entityId;
  final String? nameEn;
  final String? nameAr;
  final bool? required;
  final int? status;
  final dynamic addedBy;
  final String? createdAt;
  final String? updatedAt;

  const Document({
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

  @override
  List<Object?> get props {
    return [
      id,
      entityId,
      nameEn,
      nameAr,
      required,
      status,
      addedBy,
      createdAt,
      updatedAt,
    ];
  }
}
