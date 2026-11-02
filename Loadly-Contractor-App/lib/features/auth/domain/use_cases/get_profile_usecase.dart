import 'package:dartz/dartz.dart';

import '../entities/contractor_profile.dart';
import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class GetProfileUseCase implements UseCaseNoParams<ContractorProfile> {
  GetProfileUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, ContractorProfile>> call() {
    return _repository.getProfile();
  }
}
