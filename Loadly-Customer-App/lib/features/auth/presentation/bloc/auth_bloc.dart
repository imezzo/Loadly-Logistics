import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/storage/auth_storage.dart';
import '../../../../core/services/biometric_service.dart';
import '../../domain/failures/auth_failure.dart';
import '../../domain/use_cases/forget_password_usecase.dart';
import '../../domain/use_cases/login_usecase.dart';
import '../../domain/use_cases/login_with_otp_usecase.dart';
import '../../domain/use_cases/logout_usecase.dart';
import '../../domain/use_cases/reset_password_usecase.dart';
import '../../domain/use_cases/send_login_otp_usecase.dart';
import '../../domain/use_cases/send_mfa_code_usecase.dart';
import '../../domain/use_cases/validate_password_usecase.dart';
import '../../domain/use_cases/verify_mfa_usecase.dart';
import '../../domain/use_cases/enable_mfa_usecase.dart';
import '../../domain/use_cases/disable_mfa_usecase.dart';
import '../../domain/use_cases/change_password_usecase.dart';
import '../../domain/use_cases/verify_email_usecase.dart';
import '../../domain/use_cases/resend_verification_usecase.dart';
import 'auth_event.dart';
import 'auth_state.dart';

class AuthBloc extends Bloc<AuthEvent, AuthState> {
  AuthBloc({
    required LoginUseCase loginUseCase,
    required SendLoginOtpUseCase sendLoginOtpUseCase,
    required LoginWithOtpUseCase loginWithOtpUseCase,
    required ForgetPasswordUseCase forgetPasswordUseCase,
    required ResetPasswordUseCase resetPasswordUseCase,
    required ResetPasswordWithCodeUseCase resetPasswordWithCodeUseCase,
    required LogoutUseCase logoutUseCase,
    required SendMfaCodeUseCase sendMfaCodeUseCase,
    required VerifyMfaUseCase verifyMfaUseCase,
    required EnableMfaUseCase enableMfaUseCase,
    required DisableMfaUseCase disableMfaUseCase,
    required ChangePasswordUseCase changePasswordUseCase,
    required VerifyEmailUseCase verifyEmailUseCase,
    required ResendVerificationUseCase resendVerificationUseCase,
    required ValidatePasswordUseCase validatePasswordUseCase,
  }) : _loginUseCase = loginUseCase,
       _sendLoginOtpUseCase = sendLoginOtpUseCase,
       _loginWithOtpUseCase = loginWithOtpUseCase,
       _forgetPasswordUseCase = forgetPasswordUseCase,
       _resetPasswordUseCase = resetPasswordUseCase,
       _resetPasswordWithCodeUseCase = resetPasswordWithCodeUseCase,
       _logoutUseCase = logoutUseCase,
       _sendMfaCodeUseCase = sendMfaCodeUseCase,
       _verifyMfaUseCase = verifyMfaUseCase,
       _enableMfaUseCase = enableMfaUseCase,
       _disableMfaUseCase = disableMfaUseCase,
       _changePasswordUseCase = changePasswordUseCase,
       _verifyEmailUseCase = verifyEmailUseCase,
       _resendVerificationUseCase = resendVerificationUseCase,
       _validatePasswordUseCase = validatePasswordUseCase,
       super(const AuthInitial()) {
    on<AuthLoginRequested>(_onLoginRequested);
    on<AuthSendLoginOtpRequested>(_onSendLoginOtpRequested);
    on<AuthLoginOtpTokenSet>(_onLoginOtpTokenSet);
    on<AuthLoginWithOtpRequested>(_onLoginWithOtpRequested);
    on<AuthForgetPasswordRequested>(_onForgetPasswordRequested);
    on<AuthResetPasswordRequested>(_onResetPasswordRequested);
    on<AuthResetPasswordWithCodeRequested>(_onResetPasswordWithCodeRequested);
    on<AuthPasswordResetTokenSet>(_onPasswordResetTokenSet);
    on<AuthCheckSessionRequested>(_onCheckSessionRequested);
    on<AuthLogoutRequested>(_onLogoutRequested);
    on<AuthSendMfaCodeRequested>(_onSendMfaCodeRequested);
    on<AuthVerifyMfaRequested>(_onVerifyMfaRequested);
    on<AuthEnableMfaRequested>(_onEnableMfaRequested);
    on<AuthDisableMfaRequested>(_onDisableMfaRequested);
    on<AuthChangePasswordRequested>(_onChangePasswordRequested);
    on<AuthVerifyEmailRequested>(_onVerifyEmailRequested);
    on<AuthResendVerificationRequested>(_onResendVerificationRequested);
    on<AuthBiometricLoginRequested>(_onBiometricLoginRequested);
    on<AuthEnableBiometricRequested>(_onEnableBiometricRequested);
    on<AuthDisableBiometricRequested>(_onDisableBiometricRequested);
    on<AuthValidatePasswordRequested>(_onValidatePasswordRequested);
    on<AuthValidatePasswordAndEnableBiometricRequested>(
      _onValidatePasswordAndEnableBiometricRequested,
    );
  }

  final LoginUseCase _loginUseCase;
  final SendLoginOtpUseCase _sendLoginOtpUseCase;
  final LoginWithOtpUseCase _loginWithOtpUseCase;
  final ForgetPasswordUseCase _forgetPasswordUseCase;
  final ResetPasswordUseCase _resetPasswordUseCase;
  final ResetPasswordWithCodeUseCase _resetPasswordWithCodeUseCase;
  final LogoutUseCase _logoutUseCase;
  final SendMfaCodeUseCase _sendMfaCodeUseCase;
  final VerifyMfaUseCase _verifyMfaUseCase;
  final EnableMfaUseCase _enableMfaUseCase;
  final DisableMfaUseCase _disableMfaUseCase;
  final ChangePasswordUseCase _changePasswordUseCase;
  final VerifyEmailUseCase _verifyEmailUseCase;
  final ResendVerificationUseCase _resendVerificationUseCase;
  final ValidatePasswordUseCase _validatePasswordUseCase;
  String? _passwordResetToken;
  String? _loginOtpToken;
  String? _loginOtpPhone;

  String? get passwordResetToken => _passwordResetToken;
  String? get loginOtpToken => _loginOtpToken;
  String? get loginOtpPhone => _loginOtpPhone;

  Future<void> _onLoginRequested(
    AuthLoginRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());
    final result = await _loginUseCase(
      LoginParams(
        login: event.login,
        password: event.password,
        locale: event.locale,
      ),
    );

    result.fold(
      (failure) {
        emit(AuthLoginError(_messageForFailure(failure)));
      },
      (loginResult) {
        // Save tokens to storage
        if (loginResult.accessToken != null) {
          AuthStorage.instance.saveAccessToken(loginResult.accessToken!);
        }
        if (loginResult.refreshToken != null) {
          AuthStorage.instance.saveRefreshToken(loginResult.refreshToken!);
        }
        emit(AuthLoginSuccess(loginResult));
      },
    );
  }

  Future<void> _onForgetPasswordRequested(
    AuthForgetPasswordRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());
    final result = await _forgetPasswordUseCase(
      ForgetPasswordParams(
        identifier: event.identifier,
        channel: event.channel,
      ),
    );

    result.fold(
      (failure) {
        emit(AuthError(_messageForFailure(failure)));
      },
      (_) {
        emit(const AuthForgetPasswordSuccess());
      },
    );
  }

  Future<void> _onResetPasswordRequested(
    AuthResetPasswordRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());
    final result = await _resetPasswordUseCase(
      ResetPasswordParams(
        token: event.token,
        newPassword: event.newPassword,
        confirmPassword: event.confirmPassword,
      ),
    );

    result.fold(
      (failure) {
        emit(AuthError(_messageForFailure(failure)));
      },
      (_) {
        _passwordResetToken = null;
        emit(const AuthResetPasswordSuccess());
      },
    );
  }

  Future<void> _onResetPasswordWithCodeRequested(
    AuthResetPasswordWithCodeRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());
    final result = await _resetPasswordWithCodeUseCase(
      ResetPasswordWithCodeParams(
        code: event.code,
        login: event.login,
        newPassword: event.newPassword,
        confirmPassword: event.confirmPassword,
      ),
    );

    result.fold(
      (failure) {
        emit(AuthError(_messageForFailure(failure)));
      },
      (_) {
        _passwordResetToken = null;
        emit(const AuthResetPasswordSuccess());
      },
    );
  }

  Future<void> _onSendLoginOtpRequested(
    AuthSendLoginOtpRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());
    final result = await _sendLoginOtpUseCase(
      SendLoginOtpParams(login: event.login),
    );

    result.fold(
      (failure) {
        emit(AuthError(_messageForFailure(failure)));
      },
      (otpResult) {
        _loginOtpToken = otpResult.token;
        _loginOtpPhone = event.login;
        emit(
          AuthSendLoginOtpSuccess(token: otpResult.token, login: event.login),
        );
      },
    );
  }

  void _onLoginOtpTokenSet(
    AuthLoginOtpTokenSet event,
    Emitter<AuthState> emit,
  ) {
    _loginOtpToken = event.token;
    _loginOtpPhone = event.login;
  }

  Future<void> _onLoginWithOtpRequested(
    AuthLoginWithOtpRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());
    final result = await _loginWithOtpUseCase(
      LoginWithOtpParams(
        token: event.token,
        login: event.login,
        code: event.code,
      ),
    );

    result.fold(
      (failure) {
        emit(AuthError(_messageForFailure(failure)));
      },
      (loginResult) {
        _loginOtpToken = null;
        _loginOtpPhone = null;
        // Save tokens to storage
        if (loginResult.accessToken != null) {
          AuthStorage.instance.saveAccessToken(loginResult.accessToken!);
        }
        if (loginResult.refreshToken != null) {
          AuthStorage.instance.saveRefreshToken(loginResult.refreshToken!);
        }
        emit(AuthLoginSuccess(loginResult));
      },
    );
  }

  void _onPasswordResetTokenSet(
    AuthPasswordResetTokenSet event,
    Emitter<AuthState> emit,
  ) {
    _passwordResetToken = event.token;
  }

  String _messageForFailure(AuthFailure failure) {
    return switch (failure) {
      AuthFailureInvalidCredentials(message: final m) =>
        m ?? 'Invalid Credentials.',
      AuthFailureNetwork() => 'No internet connection.',
      AuthFailureServer(message: final m, statusCode: final code) =>
        m ?? _defaultMessageForStatusCode(code),
      AuthFailureUnknown(message: final m) => m ?? 'Something went wrong.',
    };
  }

  Future<void> _onCheckSessionRequested(
    AuthCheckSessionRequested event,
    Emitter<AuthState> emit,
  ) async {
    if (AuthStorage.instance.isLoggedIn()) {
      emit(const AuthAuthenticated());
    } else {
      emit(const AuthUnauthenticated());
    }
  }

  Future<void> _onLogoutRequested(
    AuthLogoutRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());

    // Call logout API
    final result = await _logoutUseCase();

    // Clear tokens regardless of API result
    await AuthStorage.instance.clearTokens();

    result.fold(
      (failure) {
        // Even if API fails, clear local session
        emit(const AuthUnauthenticated());
      },
      (_) {
        emit(const AuthUnauthenticated());
      },
    );
  }

  Future<void> _onSendMfaCodeRequested(
    AuthSendMfaCodeRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());
    final result = await _sendMfaCodeUseCase(
      SendMfaCodeParams(channel: event.channel),
    );

    result.fold(
      (failure) {
        emit(AuthError(_messageForFailure(failure)));
      },
      (_) {
        emit(const AuthSendMfaCodeSuccess());
      },
    );
  }

  Future<void> _onVerifyMfaRequested(
    AuthVerifyMfaRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());
    final result = await _verifyMfaUseCase(VerifyMfaParams(code: event.code));

    result.fold(
      (failure) {
        emit(AuthError(_messageForFailure(failure)));
      },
      (loginResult) {
        // Save tokens to storage
        if (loginResult.accessToken != null) {
          AuthStorage.instance.saveAccessToken(loginResult.accessToken!);
        }
        if (loginResult.refreshToken != null) {
          AuthStorage.instance.saveRefreshToken(loginResult.refreshToken!);
        }
        emit(AuthLoginSuccess(loginResult));
      },
    );
  }

  Future<void> _onEnableMfaRequested(
    AuthEnableMfaRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());
    final result = await _enableMfaUseCase(
      EnableMfaParams(channel: event.channel),
    );

    result.fold(
      (failure) {
        emit(AuthError(_messageForFailure(failure)));
      },
      (_) {
        emit(const AuthEnableMfaSuccess());
      },
    );
  }

  Future<void> _onDisableMfaRequested(
    AuthDisableMfaRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());
    final result = await _disableMfaUseCase(null);

    result.fold(
      (failure) {
        emit(AuthError(_messageForFailure(failure)));
      },
      (_) {
        emit(const AuthDisableMfaSuccess());
      },
    );
  }

  Future<void> _onChangePasswordRequested(
    AuthChangePasswordRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());
    final result = await _changePasswordUseCase(
      ChangePasswordParams(
        currentPassword: event.currentPassword,
        newPassword: event.newPassword,
        confirmPassword: event.confirmPassword,
      ),
    );

    result.fold(
      (failure) {
        emit(AuthError(_messageForFailure(failure)));
      },
      (_) {
        emit(const AuthChangePasswordSuccess());
      },
    );
  }

  Future<void> _onVerifyEmailRequested(
    AuthVerifyEmailRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());
    final result = await _verifyEmailUseCase(
      VerifyEmailParams(code: event.code),
    );

    result.fold(
      (failure) {
        emit(AuthError(_messageForFailure(failure)));
      },
      (_) {
        emit(const AuthVerifyEmailSuccess());
      },
    );
  }

  Future<void> _onResendVerificationRequested(
    AuthResendVerificationRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());
    final result = await _resendVerificationUseCase(
      ResendVerificationParams(channel: event.channel),
    );

    result.fold(
      (failure) {
        emit(AuthError(_messageForFailure(failure)));
      },
      (_) {
        emit(const AuthResendVerificationSuccess());
      },
    );
  }

  Future<void> _onBiometricLoginRequested(
    AuthBiometricLoginRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());

    // Authenticate with biometric and get saved credentials
    final credentials = await BiometricService.instance
        .authenticateAndGetCredentials(reason: 'Authenticate to login');

    if (credentials == null) {
      emit(const AuthError('Biometric authentication failed or cancelled'));
      return;
    }

    // Use the saved credentials to login
    final locale = 'en'; // Default locale, can be improved later
    final result = await _loginUseCase(
      LoginParams(
        login: credentials['login']!,
        password: credentials['password']!,
        locale: locale,
      ),
    );

    result.fold(
      (failure) {
        emit(AuthError(_messageForFailure(failure)));
      },
      (loginResult) {
        // Save tokens to storage
        if (loginResult.accessToken != null) {
          AuthStorage.instance.saveAccessToken(loginResult.accessToken!);
        }
        if (loginResult.refreshToken != null) {
          AuthStorage.instance.saveRefreshToken(loginResult.refreshToken!);
        }
        emit(AuthLoginSuccess(loginResult));
      },
    );
  }

  Future<void> _onEnableBiometricRequested(
    AuthEnableBiometricRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());

    final success = await BiometricService.instance.enableBiometric(
      login: event.login,
      password: event.password,
      reason: event.reason,
    );

    if (success) {
      emit(const AuthEnableBiometricSuccess());
    } else {
      emit(const AuthError('Failed to enable biometric login'));
    }
  }

  Future<void> _onDisableBiometricRequested(
    AuthDisableBiometricRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());

    final success = await BiometricService.instance.disableBiometric();

    if (success) {
      emit(const AuthDisableBiometricSuccess());
    } else {
      emit(const AuthError('Failed to disable biometric login'));
    }
  }

  static String _defaultMessageForStatusCode(int? code) {
    return switch (code) {
      400 => 'Invalid request. Please check your input.',
      401 => 'Invalid or expired session. Please sign in again.',
      404 => 'Resource not found.',
      405 => 'This action is not allowed.',
      422 => 'The given data was invalid.',
      500 => 'Server error. Please try again later.',
      _ => 'Something went wrong. Please try again.',
    };
  }

  Future<void> _onValidatePasswordRequested(
    AuthValidatePasswordRequested event,
    Emitter<AuthState> emit,
  ) async {
    emit(const AuthLoading());

    final result = await _validatePasswordUseCase(event.password);

    result.fold(
      (failure) {
        emit(AuthError(_messageForFailure(failure)));
      },
      (success) {
        emit(AuthValidatePasswordSuccess(success: success));
      },
    );
  }

  Future<void> _onValidatePasswordAndEnableBiometricRequested(
    AuthValidatePasswordAndEnableBiometricRequested event,
    Emitter<AuthState> emit,
  ) async {
    if (state is AuthLoading) return;
    emit(const AuthLoading());

    final validation = await _validatePasswordUseCase(event.password);

    final isValid = validation.fold((failure) {
      emit(AuthError(_messageForFailure(failure)));
      return false;
    }, (success) => success);
    if (!isValid) return;

    final success = await BiometricService.instance.enableBiometric(
      login: event.login,
      password: event.password,
      reason: event.reason,
    );

    if (success) {
      emit(const AuthEnableBiometricSuccess());
    } else {
      emit(const AuthError('Failed to enable biometric login'));
    }
  }
}
