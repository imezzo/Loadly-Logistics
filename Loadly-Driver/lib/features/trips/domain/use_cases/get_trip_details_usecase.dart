import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../entities/trip.dart';
import '../failures/trips_failure.dart';
import '../repositories/trips_repository.dart';

class GetTripDetailsUseCase implements UseCase<Trip, GetTripDetailsParams> {
  GetTripDetailsUseCase(this._repository);

  final TripsRepository _repository;

  @override
  Future<Either<TripsFailure, Trip>> call(GetTripDetailsParams params) {
    return _repository.getTripDetails(params.tripId);
  }
}

class GetTripDetailsParams {
  const GetTripDetailsParams({required this.tripId});

  final int tripId;
}
