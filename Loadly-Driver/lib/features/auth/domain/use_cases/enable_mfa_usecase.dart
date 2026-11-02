import 'package:dartz/dartz.dart';

import '../failures/auth_failure.dart';
import '../repositories/auth_repository.dart';
import 'usecase.dart';

class EnableMfaParams {
  const EnableMfaParams({required this.channel});

  final int channel;
}

class EnableMfaUseCase implements UseCase<void, EnableMfaParams> {
  EnableMfaUseCase(this._repository);

  final AuthRepository _repository;

  @override
  Future<Either<AuthFailure, void>> call(EnableMfaParams params) {
    return _repository.enableMfa(channel: params.channel);
  }
}
