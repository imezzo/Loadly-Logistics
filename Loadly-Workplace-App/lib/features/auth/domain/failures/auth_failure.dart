import 'package:equatable/equatable.dart';

sealed class AuthFailure extends Equatable {
  const AuthFailure();

  @override
  List<Object?> get props => [];
}

@pragma('vm:keep')
final class AuthFailureServer extends AuthFailure {
  const AuthFailureServer({this.message, this.statusCode});
  final String? message;
  final int? statusCode;
  @override
  List<Object?> get props => [message, statusCode];
}

@pragma('vm:keep')
final class AuthFailureNetwork extends AuthFailure {
  const AuthFailureNetwork();
}

@pragma('vm:keep')
final class AuthFailureInvalidCredentials extends AuthFailure {
  const AuthFailureInvalidCredentials();
}

final class AuthFailureUnknown extends AuthFailure {
  const AuthFailureUnknown({this.message});
  final String? message;
  @override
  List<Object?> get props => [message];
}
