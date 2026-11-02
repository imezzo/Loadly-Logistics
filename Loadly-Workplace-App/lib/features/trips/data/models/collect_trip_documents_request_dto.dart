// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'dart:io';

import 'package:dio/dio.dart';

class CollectorDocuments {
  final List<CollectTripDocumentsRequestDto> collectorDocuments;
  final bool submit;

  CollectorDocuments(this.collectorDocuments, this.submit);

  Future<FormData> toFormData() async {
    final map = <String, dynamic>{};

    for (int i = 0; i < collectorDocuments.length; i++) {
      final doc = collectorDocuments[i];

      map['CollectorDocuments[$i].documentId'] = doc.documentId;
      map['CollectorDocuments[$i].Status'] = doc.status;
      map['CollectorDocuments[$i].Note'] = doc.note;

      map['CollectorDocuments[$i].Files'] = await Future.wait(
        doc.files!.map(
          (file) => MultipartFile.fromFile(
            file.path,
            filename: file.path.split('/').last,
          ),
        ),
      );
    }
    map['submit'] = submit;

    return FormData.fromMap(map);
  }
}

class CollectTripDocumentsRequestDto {
  final int? documentId;
  final int? status;
  final List<File>? files;
  final String? note;

  CollectTripDocumentsRequestDto(
    this.documentId,
    this.status,
    this.files,
    this.note,
  );
}

// Create extension for easy reuse
extension CollectTripDocumentsListExtension
    on List<CollectTripDocumentsRequestDto> {
  // Check if document with specific ID exists
  bool hasDocumentId(int id) {
    return any((doc) => doc.documentId == id);
  }

  // Find document by ID
  CollectTripDocumentsRequestDto? findById(int id) {
    try {
      return firstWhere((doc) => doc.documentId == id);
    } catch (e) {
      return null;
    }
  }

  // Find documents by status
  List<CollectTripDocumentsRequestDto>? findByStatus(int status) {
    try {
      return where((doc) => doc.status == status).toList();
    } catch (e) {
      return null;
    }
  }

  // Get index of document with specific ID
  int indexOfDocumentId(int id) {
    return indexWhere((doc) => doc.documentId == id);
  }

  // Check if all documents have a certain status
  bool allHaveStatus(int status) {
    return every((doc) => doc.status == status);
  }

  // Check if any document has a certain status
  bool anyHasStatus(int status) {
    return any((doc) => doc.status == status);
  }

  // Count documents with specific status
  int countByStatus(int status) {
    return where((doc) => doc.status == status).length;
  }

  // Get all document IDs
  List<int> getAllDocumentIds() {
    return where(
      (doc) => doc.documentId != null,
    ).map((doc) => doc.documentId!).toList();
  }
}
