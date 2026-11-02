import 'package:dartz/dartz.dart';

import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class ValidatePasswordUseCase implements UseCase<bool, String> {
  ValidatePasswordUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, bool>> call(String password) {
    return _repository.validatePassword(password: password);
  }
}
