import 'package:equatable/equatable.dart';

class DriverDocumentType extends Equatable {
  const DriverDocumentType({
    required this.id,
    required this.entityId,
    required this.nameEn,
    required this.nameAr,
    required this.required,
    required this.status,
  });

  final int id;
  final int entityId;
  final String nameEn;
  final String nameAr;
  final bool required;
  final int status;

  String localizedName({required bool isRtl}) => isRtl ? nameAr : nameEn;

  @override
  List<Object?> get props => [id, entityId, nameEn, nameAr, required, status];
}
