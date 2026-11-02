import 'package:dartz/dartz.dart';
import '../../data/models/collect_trip_documents_request_dto.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../failures/trips_failure.dart';
import '../repositories/trips_repository.dart';

class CollectDocumentsUsecase implements UseCase<void, CollectDocumentsParams> {
  CollectDocumentsUsecase(this._repository);

  final TripsRepository _repository;

  @override
  Future<Either<TripsFailure, void>> call(CollectDocumentsParams params) {
    return _repository.collectTripDocuments(
      tripId: params.tripId,
      collectorDocuments: params.collectorDocuments,
      submit: params.submit,
    );
  }
}

class CollectDocumentsParams {
  const CollectDocumentsParams(
    this.tripId,
    this.collectorDocuments,
    this.submit,
  );

  final int tripId;
  final List<CollectTripDocumentsRequestDto> collectorDocuments;
  final bool submit;
}
