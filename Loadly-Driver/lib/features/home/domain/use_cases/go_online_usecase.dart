import 'package:dartz/dartz.dart';

import '../failures/home_failure.dart';
import '../repositories/home_repository.dart';
import '../../../auth/domain/use_cases/usecase.dart';

class GoOnlineParams {
  const GoOnlineParams({required this.latitude, required this.longitude});

  final double latitude;
  final double longitude;
}

class GoOnlineUseCase implements UseCase<void, GoOnlineParams> {
  GoOnlineUseCase(this._repository);

  final HomeRepository _repository;

  @override
  Future<Either<HomeFailure, void>> call(GoOnlineParams params) {
    return _repository.goOnline(
      latitude: params.latitude,
      longitude: params.longitude,
    );
  }
}
