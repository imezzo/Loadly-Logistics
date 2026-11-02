import 'package:equatable/equatable.dart';

import '../../domain/entities/login_result.dart';
import '../../domain/entities/document_definition.dart';

abstract class AuthState extends Equatable {
  const AuthState();

  @override
  List<Object?> get props => [];
}

class AuthInitial extends AuthState {
  const AuthInitial();
}

class AuthLoading extends AuthState {
  const AuthLoading();
}

class AuthLoginSuccess extends AuthState {
  const AuthLoginSuccess(this.loginResult);

  final LoginResult loginResult;

  @override
  List<Object?> get props => [loginResult];
}

class AuthForgetPasswordSuccess extends AuthState {
  const AuthForgetPasswordSuccess();
}

class AuthResetPasswordSuccess extends AuthState {
  const AuthResetPasswordSuccess();
}

class AuthSendLoginOtpSuccess extends AuthState {
  const AuthSendLoginOtpSuccess({required this.token, required this.login});

  final String token;
  final String login;

  @override
  List<Object?> get props => [token, login];
}

class AuthError extends AuthState {
  const AuthError(this.message, {this.errorCode});

  final String message;

  /// Optional error code for known errors (e.g. biometric) to resolve localized message in UI.
  final String? errorCode;

  @override
  List<Object?> get props => [message, errorCode];
}

/// Emitted when password login fails (e.g. 401 invalid credentials).
/// UI should show this inline under the password field instead of a dialog.
class AuthLoginError extends AuthState {
  const AuthLoginError(this.message);

  final String message;

  @override
  List<Object?> get props => [message];
}

class AuthAuthenticated extends AuthState {
  const AuthAuthenticated();
}

class AuthUnauthenticated extends AuthState {
  const AuthUnauthenticated();
}

class AuthSendMfaCodeSuccess extends AuthState {
  const AuthSendMfaCodeSuccess();
}

class AuthEnableMfaSuccess extends AuthState {
  const AuthEnableMfaSuccess();
}

class AuthDisableMfaSuccess extends AuthState {
  const AuthDisableMfaSuccess();
}

class AuthChangePasswordSuccess extends AuthState {
  const AuthChangePasswordSuccess();
}

class AuthEnableBiometricSuccess extends AuthState {
  const AuthEnableBiometricSuccess();
}

class AuthDisableBiometricSuccess extends AuthState {
  const AuthDisableBiometricSuccess();
}

class AuthValidatePasswordSuccess extends AuthState {
  const AuthValidatePasswordSuccess({required this.success});

  final bool success;
  @override
  List<Object?> get props => [success];
}

class AuthRegisterSuccess extends AuthState {
  const AuthRegisterSuccess();
}

class AuthDocumentsLoading extends AuthState {
  const AuthDocumentsLoading();
}

class AuthDocumentsLoaded extends AuthState {
  const AuthDocumentsLoaded(this.documents);

  final List<DocumentDefinition> documents;

  @override
  List<Object?> get props => [documents];
}

class AuthDocumentsError extends AuthState {
  const AuthDocumentsError(this.message);

  final String message;

  @override
  List<Object?> get props => [message];
}
