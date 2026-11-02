import '../../domain/entities/register_document.dart';

class RegisterDocumentModel extends RegisterDocument {
  const RegisterDocumentModel({
    required super.documentId,
    required super.filePath,
    required super.expiryDate,
  });
}
