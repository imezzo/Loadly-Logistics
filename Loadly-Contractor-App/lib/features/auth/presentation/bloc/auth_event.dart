import 'package:equatable/equatable.dart';
import '../../domain/entities/register_document.dart';

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

class AuthSendLoginOtpRequested extends AuthEvent {
  const AuthSendLoginOtpRequested({required this.login});

  final String login;

  @override
  List<Object?> get props => [login];
}

class AuthLoginOtpTokenSet extends AuthEvent {
  const AuthLoginOtpTokenSet({required this.token, required this.login});

  final String token;
  final String login;

  @override
  List<Object?> get props => [token, login];
}

class AuthLoginWithOtpRequested extends AuthEvent {
  const AuthLoginWithOtpRequested({
    required this.token,
    required this.login,
    required this.code,
  });

  final String token;
  final String login;
  final String code;

  @override
  List<Object?> get props => [token, login, code];
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

class AuthEnableMfaRequested extends AuthEvent {
  const AuthEnableMfaRequested({required this.channel});

  final int channel;

  @override
  List<Object?> get props => [channel];
}

class AuthDisableMfaRequested extends AuthEvent {
  const AuthDisableMfaRequested();
}

class AuthChangePasswordRequested extends AuthEvent {
  const AuthChangePasswordRequested({
    required this.currentPassword,
    required this.newPassword,
    required this.confirmPassword,
  });

  final String currentPassword;
  final String newPassword;
  final String confirmPassword;

  @override
  List<Object?> get props => [currentPassword, newPassword, confirmPassword];
}

class AuthBiometricLoginRequested extends AuthEvent {
  const AuthBiometricLoginRequested({required this.localizedReason});

  /// Shown in the OS biometric sheet (Face ID / fingerprint).
  final String localizedReason;

  @override
  List<Object?> get props => [localizedReason];
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

class AuthValidatePasswordRequested extends AuthEvent {
  const AuthValidatePasswordRequested({required this.password});

  final String password;

  @override
  List<Object?> get props => [password];
}

class AuthValidatePasswordAndEnableBiometricRequested extends AuthEvent {
  const AuthValidatePasswordAndEnableBiometricRequested({
    required this.password,
    required this.login,
    required this.reason,
  });
  final String password;
  final String login;
  final String reason;

  @override
  List<Object?> get props => [password, login, reason];
}

class AuthRegisterRequested extends AuthEvent {
  const AuthRegisterRequested({
    required this.fullName,
    required this.phone,
    required this.address,
    required this.nationalId,
    required this.documents,
  });

  final String fullName;
  final String phone;
  final String address;
  final String nationalId;
  final List<RegisterDocument> documents;

  @override
  List<Object?> get props => [fullName, phone, address, nationalId, documents];
}

class AuthGetDocumentsRequested extends AuthEvent {
  const AuthGetDocumentsRequested({this.entityId});

  final int? entityId;

  @override
  List<Object?> get props => [entityId];
}
