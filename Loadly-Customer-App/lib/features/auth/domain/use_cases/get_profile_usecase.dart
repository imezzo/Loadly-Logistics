import 'package:dartz/dartz.dart';

import '../entities/customer_profile.dart';
import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class GetProfileUseCase implements UseCaseNoParams<CustomerProfile> {
  GetProfileUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, CustomerProfile>> call() {
    return _repository.getProfile();
  }
}
