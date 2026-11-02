import '../../domain/entities/driver_document_type/driver_document_type.dart';
import '../models/driver_document_type_dto.dart';

extension DriverDocumentTypeDtoMapper on DriverDocumentTypeDto {
  DriverDocumentType toEntity() {
    return DriverDocumentType(
      id: id,
      entityId: entityId,
      nameEn: nameEn,
      nameAr: nameAr,
      required: required,
      status: status,
    );
  }
}
