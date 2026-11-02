// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../failures/trips_failure.dart';
import '../repositories/trips_repository.dart';

class BulkConfirmDocumentsUsecase
    implements UseCase<void, BulkConfirmDocumentsParams> {
  BulkConfirmDocumentsUsecase(this._repository);

  final TripsRepository _repository;

  @override
  Future<Either<TripsFailure, void>> call(BulkConfirmDocumentsParams params) {
    return _repository.bulkConfirmTripDocuments(tripIds: params.tripIds);
  }
}

class BulkConfirmDocumentsParams {
  const BulkConfirmDocumentsParams(this.tripIds);

  final List<int> tripIds;
}
