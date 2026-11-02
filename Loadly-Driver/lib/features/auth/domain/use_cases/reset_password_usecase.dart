import 'package:dartz/dartz.dart';

import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class ResetPasswordParams {
  const ResetPasswordParams({
    required this.token,
    required this.newPassword,
    required this.confirmPassword,
  });

  final String token;
  final String newPassword;
  final String confirmPassword;
}

class ResetPasswordUseCase implements UseCase<void, ResetPasswordParams> {
  ResetPasswordUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, void>> call(ResetPasswordParams params) {
    return _repository.resetPassword(
      token: params.token,
      newPassword: params.newPassword,
      confirmPassword: params.confirmPassword,
    );
  }
}

class ResetPasswordWithCodeParams {
  const ResetPasswordWithCodeParams({
    required this.code,
    required this.login,
    required this.newPassword,
    required this.confirmPassword,
  });

  final String code;
  final String login;
  final String newPassword;
  final String confirmPassword;
}

class ResetPasswordWithCodeUseCase
    implements UseCase<void, ResetPasswordWithCodeParams> {
  ResetPasswordWithCodeUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, void>> call(ResetPasswordWithCodeParams params) {
    return _repository.resetPasswordWithCode(
      code: params.code,
      login: params.login,
      newPassword: params.newPassword,
      confirmPassword: params.confirmPassword,
    );
  }
}
