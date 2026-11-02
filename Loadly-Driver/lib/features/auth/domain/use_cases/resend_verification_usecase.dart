import 'package:dartz/dartz.dart';

import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class ResendVerificationParams {
  const ResendVerificationParams({this.channel});

  /// 0 = Email, 1 = SMS. Null = default (Email).
  final int? channel;
}

class ResendVerificationUseCase
    implements UseCase<void, ResendVerificationParams> {
  ResendVerificationUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, void>> call(ResendVerificationParams params) {
    return _repository.resendVerification(channel: params.channel);
  }
}
