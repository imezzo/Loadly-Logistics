import 'package:dartz/dartz.dart';

import '../../../../core/errors/failure.dart';
import '../../../../core/usecase/usecase.dart';
import '../entities/update_trip_driver/update_trip_driver.dart';
import '../repositories/driver_repository.dart';

class UpdateTripDriverUseCase implements UseCase<void, UpdateTripDriver> {
  UpdateTripDriverUseCase(this._repository);

  final DriverRepository _repository;

  @override
  Future<Either<Failure, void>> call(UpdateTripDriver params) {
    return _repository.updateTripDriver(params);
  }
}
