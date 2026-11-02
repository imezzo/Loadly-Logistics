import 'package:dartz/dartz.dart';

import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class ForgetPasswordParams {
  const ForgetPasswordParams({required this.identifier, required this.channel});

  final String identifier;
  final int channel;
}

class ForgetPasswordUseCase implements UseCase<void, ForgetPasswordParams> {
  ForgetPasswordUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, void>> call(ForgetPasswordParams params) {
    return _repository.forgetPassword(
      identifier: params.identifier,
      channel: params.channel,
    );
  }
}
