import 'package:dartz/dartz.dart';

import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class LogoutUseCase implements UseCaseNoParams<void> {
  LogoutUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, void>> call() {
    return _repository.logout();
  }
}
