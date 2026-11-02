import 'package:dartz/dartz.dart';

import '../entities/login_result.dart';
import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class VerifyMfaParams {
  const VerifyMfaParams({required this.code});

  final String code;
}

class VerifyMfaUseCase implements UseCase<LoginResult, VerifyMfaParams> {
  VerifyMfaUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, LoginResult>> call(VerifyMfaParams params) {
    return _repository.verifyMfa(code: params.code);
  }
}
