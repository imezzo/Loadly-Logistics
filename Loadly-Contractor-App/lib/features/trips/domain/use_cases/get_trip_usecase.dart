import 'package:dartz/dartz.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/usecase/usecase.dart';
import '../entities/trip/trip.dart';
import '../repositories/trip_repository.dart';

class GetTripParams {
  const GetTripParams(this.tripId);

  final int tripId;
}

class GetTripUseCase implements UseCase<Trip, GetTripParams> {
  GetTripUseCase(this._repository);

  final TripRepository _repository;

  @override
  Future<Either<Failure, Trip>> call(GetTripParams params) {
    return _repository.getTrip(params.tripId);
  }
}
