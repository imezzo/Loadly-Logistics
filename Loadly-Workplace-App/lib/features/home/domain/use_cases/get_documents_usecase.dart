// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:dartz/dartz.dart';
import '../entities/document.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../failures/home_failure.dart';
import '../repositories/home_repository.dart';

class GetDocumentsUsecase
    implements UseCase<List<Document>, GetDocumentsParams> {
  GetDocumentsUsecase(this._repository);

  final HomeRepository _repository;

  @override
  Future<Either<HomeFailure, List<Document>>> call(GetDocumentsParams params) {
    return _repository.getDocuments();
  }
}

class GetDocumentsParams {
  const GetDocumentsParams();
}
