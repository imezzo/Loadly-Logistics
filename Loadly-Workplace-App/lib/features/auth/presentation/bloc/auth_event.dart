import 'package:equatable/equatable.dart';

abstract class AuthEvent extends Equatable {
  const AuthEvent();

  @override
  List<Object?> get props => [];
}

class AuthLoginRequested extends AuthEvent {
  const AuthLoginRequested({required this.login, this.password, this.locale});

  final String login;
  final String? password;
  final String? locale;

  @override
  List<Object?> get props => [login, password, locale];
}

class AuthForgetPasswordRequested extends AuthEvent {
  const AuthForgetPasswordRequested({
    required this.identifier,
    required this.channel,
  });

  final String identifier;
  final int channel;

  @override
  List<Object?> get props => [identifier, channel];
}

class AuthResetPasswordRequested extends AuthEvent {
  const AuthResetPasswordRequested({
    required this.token,
    required this.newPassword,
    required this.confirmPassword,
  });

  final String token;
  final String newPassword;
  final String confirmPassword;

  @override
  List<Object?> get props => [token, newPassword, confirmPassword];
}

class AuthResetPasswordWithCodeRequested extends AuthEvent {
  const AuthResetPasswordWithCodeRequested({
    required this.code,
    required this.login,
    required this.newPassword,
    required this.confirmPassword,
  });

  final String code;
  final String login;
  final String newPassword;
  final String confirmPassword;

  @override
  List<Object?> get props => [code, login, newPassword, confirmPassword];
}

class AuthPasswordResetTokenSet extends AuthEvent {
  const AuthPasswordResetTokenSet(this.token);

  final String? token;

  @override
  List<Object?> get props => [token];
}

class AuthCheckSessionRequested extends AuthEvent {
  const AuthCheckSessionRequested();
}

class AuthLogoutRequested extends AuthEvent {
  const AuthLogoutRequested();
}

class AuthSendMfaCodeRequested extends AuthEvent {
  const AuthSendMfaCodeRequested({required this.channel});

  final int channel;

  @override
  List<Object?> get props => [channel];
}

class AuthVerifyMfaRequested extends AuthEvent {
  const AuthVerifyMfaRequested({required this.code});

  final String code;

  @override
  List<Object?> get props => [code];
}

class AuthBiometricLoginRequested extends AuthEvent {
  const AuthBiometricLoginRequested({
    required this.localizedReason,
    this.locale,
  });

  /// Shown in the OS biometric sheet (Face ID / fingerprint).
  final String localizedReason;

  /// App language sent with the login request. Defaults to Arabic.
  final String? locale;

  @override
  List<Object?> get props => [localizedReason, locale];
}

class AuthEnableBiometricRequested extends AuthEvent {
  const AuthEnableBiometricRequested({
    required this.login,
    required this.password,
    required this.localizedAuthenticateReason,
  });

  final String login;
  final String password;

  /// Shown in the OS biometric sheet when confirming enrollment.
  final String localizedAuthenticateReason;

  @override
  List<Object?> get props => [login, password, localizedAuthenticateReason];
}

class AuthDisableBiometricRequested extends AuthEvent {
  const AuthDisableBiometricRequested();

  @override
  List<Object?> get props => [];
}
