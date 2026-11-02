import 'package:dartz/dartz.dart';

import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class SendMfaCodeParams {
  const SendMfaCodeParams({required this.channel});

  final int channel;
}

class SendMfaCodeUseCase implements UseCase<void, SendMfaCodeParams> {
  SendMfaCodeUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, void>> call(SendMfaCodeParams params) {
    return _repository.sendMfaCode(channel: params.channel);
  }
}
