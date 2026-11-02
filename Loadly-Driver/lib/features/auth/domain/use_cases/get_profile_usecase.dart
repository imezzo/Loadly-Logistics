import 'package:dartz/dartz.dart';

import '../entities/driver_profile.dart';
import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class GetProfileUseCase implements UseCaseNoParams<DriverProfile> {
  GetProfileUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, DriverProfile>> call() {
    return _repository.getProfile();
  }
}
