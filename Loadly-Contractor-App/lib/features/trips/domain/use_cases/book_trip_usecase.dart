import 'package:dartz/dartz.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/usecase/usecase.dart';
import '../repositories/trip_repository.dart';

class BookTripUseCase implements UseCase<void, int> {
  BookTripUseCase(this._repository);

  final TripRepository _repository;

  @override
  Future<Either<Failure, void>> call(int tripId) {
    return _repository.bookTrip(tripId);
  }
}
