import 'package:dartz/dartz.dart';

import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class DisableMfaUseCase implements UseCase<void, void> {
  DisableMfaUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, void>> call(void params) {
    return _repository.disableMfa();
  }
}
