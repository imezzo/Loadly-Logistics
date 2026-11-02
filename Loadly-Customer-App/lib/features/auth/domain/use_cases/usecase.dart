import 'package:dartz/dartz.dart';

/// Base use case interface for all use cases.

abstract interface class UseCase<T, Params> {
  Future<Either<dynamic, T>> call(Params params);
}

/// Use case with no parameters
abstract interface class UseCaseNoParams<T> {
  Future<Either<dynamic, T>> call();
}

/// Parameters class for use cases that don't need parameters
class NoParams {
  const NoParams();
}
