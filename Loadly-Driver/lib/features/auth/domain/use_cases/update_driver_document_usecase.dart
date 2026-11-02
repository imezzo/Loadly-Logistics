import 'package:dartz/dartz.dart';

import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class UpdateDriverDocumentParams {
  const UpdateDriverDocumentParams({
    required this.id,
    this.filePath,
    this.expiryDate,
  });

  final int id;
  final String? filePath;
  final DateTime? expiryDate;
}

class UpdateDriverDocumentUseCase
    implements UseCase<void, UpdateDriverDocumentParams> {
  UpdateDriverDocumentUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, void>> call(UpdateDriverDocumentParams params) {
    return _repository.updateDriverDocument(
      id: params.id,
      filePath: params.filePath,
      expiryDate: params.expiryDate,
    );
  }
}
