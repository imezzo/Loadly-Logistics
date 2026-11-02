import 'package:dartz/dartz.dart';

import '../failures/home_failure.dart';
import '../repositories/home_repository.dart';
import '../../../auth/domain/use_cases/usecase.dart';

class UpdateLocationParams {
  const UpdateLocationParams({required this.latitude, required this.longitude});

  final double latitude;
  final double longitude;
}

class UpdateLocationUseCase implements UseCase<void, UpdateLocationParams> {
  UpdateLocationUseCase(this._repository);

  final HomeRepository _repository;

  @override
  Future<Either<HomeFailure, void>> call(UpdateLocationParams params) {
    return _repository.updateLocation(
      latitude: params.latitude,
      longitude: params.longitude,
    );
  }
}
