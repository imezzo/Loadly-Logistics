import 'package:dartz/dartz.dart';

import '../entities/login_result.dart';
import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class LoginParams {
  const LoginParams({required this.login, this.password, this.locale});

  final String login;
  final String? password;
  final String? locale;
}

class LoginUseCase implements UseCase<LoginResult, LoginParams> {
  LoginUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, LoginResult>> call(LoginParams params) {
    return _repository.login(
      login: params.login,
      password: params.password,
      locale: params.locale,
    );
  }
}
