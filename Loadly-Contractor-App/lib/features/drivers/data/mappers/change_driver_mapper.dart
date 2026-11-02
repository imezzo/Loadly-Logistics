import '../../../trips/domain/entities/book_trip/book_trip_request.dart';
import '../models/change_driver_dto.dart';

extension BookTripDriverMapper on BookTripDriver {
  ApplyDriverDto toDto() {
    return ApplyDriverDto(
      fullNameEn: fullNameEn,
      fullNameAr: fullNameAr,
      phone: phone,
      nationalId: nationalId,
      documents: documents.map((document) => document.toDto()).toList(),
    );
  }
}

extension BookTripDriverDocumentMapper on BookTripDriverDocument {
  ApplyDriverDocumentDto toDto() {
    return ApplyDriverDocumentDto(
      documentId: documentId,
      filePath: filePath,
      expiryDate: expiryDate,
      status: status,
    );
  }
}
