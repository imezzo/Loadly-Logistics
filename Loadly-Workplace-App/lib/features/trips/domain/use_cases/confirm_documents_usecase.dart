import 'package:dartz/dartz.dart';
import '../../data/models/collect_trip_documents_request_dto.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../failures/trips_failure.dart';
import '../repositories/trips_repository.dart';

class ConfirmDocumentsUsecase implements UseCase<void, ConfirmDocumentsParams> {
  ConfirmDocumentsUsecase(this._repository);

  final TripsRepository _repository;

  @override
  Future<Either<TripsFailure, void>> call(ConfirmDocumentsParams params) {
    return _repository.confirmTripDocuments(
      tripId: params.tripId,
      collectorDocuments: params.collectorDocuments,
      submit: params.submit,
    );
  }
}

class ConfirmDocumentsParams {
  const ConfirmDocumentsParams(
    this.tripId,
    this.collectorDocuments,
    this.submit,
  );

  final int tripId;
  final List<CollectTripDocumentsRequestDto> collectorDocuments;
  final bool submit;
}
