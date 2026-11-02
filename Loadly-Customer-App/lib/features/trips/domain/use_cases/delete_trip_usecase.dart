import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../failures/trips_failure.dart';
import '../repositories/trips_repository.dart';

class DeleteTripUsecase implements UseCase<void, DeleteTripParams> {
  DeleteTripUsecase(this._repository);

  final TripsRepository _repository;

  @override
  Future<Either<TripsFailure, void>> call(DeleteTripParams params) {
    return _repository.deleteTrip(params.tripId);
  }
}

class DeleteTripParams {
  const DeleteTripParams({required this.tripId});

  final int tripId;
}
