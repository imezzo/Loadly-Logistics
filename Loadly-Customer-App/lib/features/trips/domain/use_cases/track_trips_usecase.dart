import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../entities/track_trips_result.dart';
import '../failures/trips_failure.dart';
import '../repositories/trips_repository.dart';

class TrackTripsUsecase
    implements UseCase<List<TrackTripsResult>, TrackTripsParams> {
  TrackTripsUsecase(this._repository);

  final TripsRepository _repository;

  @override
  Future<Either<TripsFailure, List<TrackTripsResult>>> call(
    TrackTripsParams params,
  ) {
    return _repository.trackTrips();
  }
}

class TrackTripsParams {}
