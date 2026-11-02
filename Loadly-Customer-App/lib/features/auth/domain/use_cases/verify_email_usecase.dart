import 'package:dartz/dartz.dart';

import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class VerifyEmailParams {
  const VerifyEmailParams({required this.code});

  final String code;
}

class VerifyEmailUseCase implements UseCase<void, VerifyEmailParams> {
  VerifyEmailUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, void>> call(VerifyEmailParams params) {
    return _repository.verifyEmail(code: params.code);
  }
}
