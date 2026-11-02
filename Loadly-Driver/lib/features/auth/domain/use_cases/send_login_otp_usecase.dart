import 'package:dartz/dartz.dart';

import '../entities/send_login_otp_result.dart';
import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class SendLoginOtpParams {
  const SendLoginOtpParams({required this.login});

  final String login;
}

class SendLoginOtpUseCase
    implements UseCase<SendLoginOtpResult, SendLoginOtpParams> {
  SendLoginOtpUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, SendLoginOtpResult>> call(
    SendLoginOtpParams params,
  ) {
    return _repository.sendLoginOtp(login: params.login);
  }
}
