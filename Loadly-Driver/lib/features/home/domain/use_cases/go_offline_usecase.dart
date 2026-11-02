import 'package:dartz/dartz.dart';

import '../failures/home_failure.dart';
import '../repositories/home_repository.dart';
import '../../../auth/domain/use_cases/usecase.dart';

class GoOfflineParams {
  const GoOfflineParams({required this.latitude, required this.longitude});

  final double latitude;
  final double longitude;
}

class GoOfflineUseCase implements UseCase<void, GoOfflineParams> {
  GoOfflineUseCase(this._repository);

  final HomeRepository _repository;

  @override
  Future<Either<HomeFailure, void>> call(GoOfflineParams params) {
    return _repository.goOffline(
      latitude: params.latitude,
      longitude: params.longitude,
    );
  }
}
