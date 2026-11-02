import 'package:dartz/dartz.dart';

import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class UploadDriverDocumentParams {
  const UploadDriverDocumentParams({
    required this.documentId,
    required this.filePath,
    this.expiryDate,
  });

  final int documentId;
  final String filePath;
  final DateTime? expiryDate;
}

class UploadDriverDocumentUseCase
    implements UseCase<void, UploadDriverDocumentParams> {
  UploadDriverDocumentUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, void>> call(UploadDriverDocumentParams params) {
    return _repository.uploadDriverDocument(
      documentId: params.documentId,
      filePath: params.filePath,
      expiryDate: params.expiryDate,
    );
  }
}
