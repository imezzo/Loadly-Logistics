import 'package:equatable/equatable.dart';

class DocumentDefinition extends Equatable {
  final int id;
  final int entityId;
  final String nameEn;
  final String nameAr;
  final bool required;
  final int status;

  const DocumentDefinition({
    required this.id,
    required this.entityId,
    required this.nameEn,
    required this.nameAr,
    required this.required,
    required this.status,
  });

  @override
  List<Object?> get props => [id, entityId, nameEn, nameAr, required, status];
}
