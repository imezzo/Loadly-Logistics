import 'package:dartz/dartz.dart';

import '../entities/collector_profile.dart';
import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class GetProfileUseCase implements UseCaseNoParams<CollectorProfile> {
  GetProfileUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, CollectorProfile>> call() {
    return _repository.getProfile();
  }
}
