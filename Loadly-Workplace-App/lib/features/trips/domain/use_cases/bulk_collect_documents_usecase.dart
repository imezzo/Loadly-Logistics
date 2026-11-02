// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../failures/trips_failure.dart';
import '../repositories/trips_repository.dart';

class BulkCollectDocumentsUsecase
    implements UseCase<void, BulkCollectDocumentsParams> {
  BulkCollectDocumentsUsecase(this._repository);

  final TripsRepository _repository;

  @override
  Future<Either<TripsFailure, void>> call(BulkCollectDocumentsParams params) {
    return _repository.bulkCollectTripDocuments(tripIds: params.tripIds);
  }
}

class BulkCollectDocumentsParams {
  const BulkCollectDocumentsParams(this.tripIds);

  final List<int> tripIds;
}
