import 'package:dartz/dartz.dart';

import '../entities/login_result.dart';
import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class LoginWithOtpParams {
  const LoginWithOtpParams({
    required this.token,
    required this.login,
    required this.code,
  });

  final String token;
  final String login;
  final String code;
}

class LoginWithOtpUseCase implements UseCase<LoginResult, LoginWithOtpParams> {
  LoginWithOtpUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, LoginResult>> call(LoginWithOtpParams params) {
    return _repository.loginWithOtp(
      token: params.token,
      login: params.login,
      code: params.code,
    );
  }
}
