import 'package:equatable/equatable.dart';

abstract class HomeFailure extends Equatable {
  const HomeFailure();

  @override
  List<Object?> get props => [];
}

class HomeFailureNetwork extends HomeFailure {
  const HomeFailureNetwork({this.message});

  final String? message;

  @override
  List<Object?> get props => [message];
}

class HomeFailureServer extends HomeFailure {
  const HomeFailureServer({this.message, this.statusCode});

  final String? message;
  final int? statusCode;

  @override
  List<Object?> get props => [message, statusCode];
}

class HomeFailureUnknown extends HomeFailure {
  const HomeFailureUnknown({this.message});

  final String? message;

  @override
  List<Object?> get props => [message];
}
