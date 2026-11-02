import 'package:dartz/dartz.dart';

import '../entities/document_definition.dart';
import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class GetDocumentsUseCase
    implements UseCase<List<DocumentDefinition>, int?> {
  GetDocumentsUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, List<DocumentDefinition>>> call(int? entityId) {
    return _repository.getDocuments(entityId: entityId);
  }
}
