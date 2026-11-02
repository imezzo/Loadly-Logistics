import '../models/document_dto.dart';
import '../../domain/entities/document.dart';

extension DocumentDtoMapper on DocumentDto {
  Document toEntity() {
    return Document(
      id: id,
      entityId: entityId,
      nameEn: nameEn,
      nameAr: nameAr,
      required: required,
      status: status,
      addedBy: addedBy,
      createdAt: createdAt,
      updatedAt: updatedAt,
    );
  }
}

// Extension for List mapping
extension DocumentDtoListMapper on List<DocumentDto> {
  List<Document> toEntityList() {
    return map((dto) => dto.toEntity()).toList();
  }
}
