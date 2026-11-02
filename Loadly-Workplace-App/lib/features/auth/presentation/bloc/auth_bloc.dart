import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/errors/failure_message_mapper.dart';
import '../../../../core/errors/user_message_codes.dart';
import '../../../../core/extensions/permission_search.dart';
import '../../../../core/storage/auth_storage.dart';
import '../../../../core/services/biometric_service.dart';
import '../../domain/failures/auth_failure.dart';
import '../../domain/use_cases/forget_password_usecase.dart';
import '../../domain/use_cases/login_usecase.dart';
import '../../domain/use_cases/logout_usecase.dart';
import '../../domain/use_cases/reset_password_usecase.dart';
import '../../domain/use_cases/send_mfa_code_usecase.dart';
import '../../domain/use_cases/verify_mfa_usecase.dart';
import 'auth_event.dart';
import 'auth_state.dart';

class AuthBloc extends Bloc<AuthEvent, AuthState> {
  AuthBloc({
    required LoginUseCase loginUseCase,
    required ForgetPasswordUseCase forgetPasswordUseCase,
    required ResetPasswordUseCase resetPasswordUseCase,
    required ResetPasswordWithCodeUseCase resetPasswordWithCodeUseCase,
    required LogoutUseCase logoutUseCase,
    required SendMfaCodeUseCase sendMfaCodeUseCase,
    required VerifyMfaUseCase verifyMfaUseCase,
  }) : _loginUseCase = loginUseCase,
       _forgetPasswordUseCase = forgetPasswordUseCase,
       _resetPasswordUseCase = resetPasswordUseCase,
       _resetPasswordWithCodeUseCase = resetPasswordWithCodeUseCase,
       _logoutUseCase = logoutUseCase,
       _sendMfaCodeUseCase = sendMfaCodeUseCase,
       _verifyMfaUseCase = verifyMfaUseCase,
       super(const AuthInitial()) {
    on<AuthLoginRequested>(_onLoginRequested);
    on<AuthForgetPasswordRequested>(_onForgetPasswordRequested);
    on<AuthResetPasswordRequested>(_onResetPasswordRequested);
    on<AuthResetPasswordWithCodeRequested>(_onResetPasswordWithCodeRequested);
    on<AuthPasswordResetTokenSet>(_onPasswordResetTokenSet);
    on<AuthCheckSessionRequested>(_onCheckSessionRequested);
    on<AuthLogoutRequested>(_onLogoutRequested);
    on<AuthSendMfaCodeRequested>(_onSendMfaCodeRequested);
    on<AuthVerifyMfaRequested>(_onVerifyMfaRequested);
    on<AuthBiometricLoginRequested>(_onBiometricLoginRequested);
    on<AuthEnableBiometricRequested>(_onEnableBiometricRequested);
    on<AuthDisableBiometricRequested>(_onDisableBiometricRequested);
  }

  final LoginUseCase _loginUseCase;
  final ForgetPasswordUseCase _forgetPasswordUseCase;
  final ResetPasswordUseCase _resetPasswordUseCase;
  final ResetPasswordWithCodeUseCase _resetPasswordWithCodeUseCase;
  final LogoutUseCase _logoutUseCase;
  final SendMfaCodeUseCase _sendMfaCodeUseCase;
  final VerifyMfaUseCase _verifyMfaUseCase;
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
        emit(
          AuthLoginError(
            _messageForFailure(
              failure,
              invalidCredentialsMessage: UserMessageCodes.invalidLogin,
              serverFallback: UserMessageCodes.invalidLogin,
              unknownMessage: UserMessageCodes.invalidLogin,
            ),
          ),
        );
      },
      (loginResult) {
        // Save tokens to storage
        if (loginResult.accessToken != null) {
          AuthStorage.instance.saveAccessToken(loginResult.accessToken!);
        }
        if (loginResult.refreshToken != null) {
          AuthStorage.instance.saveRefreshToken(loginResult.refreshToken!);
        }
        if (loginResult.permissions != null &&
            loginResult.permissions!.hasPermission("confirm_trip_documents")) {
          AuthStorage.instance.saveisFinancialCollector(true);
        } else {
          AuthStorage.instance.saveisFinancialCollector(false);
        }
        if (event.password != null && event.password!.isNotEmpty) {
          AuthStorage.instance.saveauthPassword(event.password);
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
        emit(
          AuthError(
            _messageForFailure(
              failure,
              serverFallback: UserMessageCodes.forgetPassword,
              unknownMessage: UserMessageCodes.forgetPassword,
            ),
          ),
        );
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
        emit(
          AuthError(
            _messageForFailure(
              failure,
              serverFallback: UserMessageCodes.passwordReset,
              unknownMessage: UserMessageCodes.passwordReset,
            ),
          ),
        );
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
        emit(
          AuthError(
            _messageForFailure(
              failure,
              invalidCredentialsMessage: UserMessageCodes.invalidCode,
              serverFallback: UserMessageCodes.passwordReset,
              unknownMessage: UserMessageCodes.passwordReset,
            ),
          ),
        );
      },
      (_) {
        _passwordResetToken = null;
        emit(const AuthResetPasswordSuccess());
      },
    );
  }

  void _onPasswordResetTokenSet(
    AuthPasswordResetTokenSet event,
    Emitter<AuthState> emit,
  ) {
    _passwordResetToken = event.token;
  }

  String _messageForFailure(
    AuthFailure failure, {
    String invalidCredentialsMessage = UserMessageCodes.invalidLogin,
    String unknownMessage = UserMessageCodes.unexpected,
    String serverFallback = UserMessageCodes.unexpected,
  }) {
    return FailureMessageMapper.forAuth(
      failure,
      invalidCredentialsMessage: invalidCredentialsMessage,
      unknownMessage: unknownMessage,
      serverFallback: serverFallback,
    );
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
        emit(
          AuthError(
            _messageForFailure(
              failure,
              serverFallback: UserMessageCodes.requestFailed,
              unknownMessage: UserMessageCodes.requestFailed,
            ),
          ),
        );
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
        emit(
          AuthError(
            _messageForFailure(
              failure,
              invalidCredentialsMessage: UserMessageCodes.invalidCode,
              serverFallback: UserMessageCodes.invalidCode,
              unknownMessage: UserMessageCodes.invalidCode,
            ),
          ),
        );
      },
      (loginResult) {
        // Save tokens to storage
        if (loginResult.accessToken != null) {
          AuthStorage.instance.saveAccessToken(loginResult.accessToken!);
        }
        if (loginResult.refreshToken != null) {
          AuthStorage.instance.saveRefreshToken(loginResult.refreshToken!);
        }
        if (loginResult.permissions != null &&
            loginResult.permissions!.hasPermission("confirm_trip_documents")) {
          AuthStorage.instance.saveisFinancialCollector(true);
        } else {
          AuthStorage.instance.saveisFinancialCollector(false);
        }
        emit(AuthLoginSuccess(loginResult));
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
        .authenticateAndGetCredentials(reason: event.localizedReason);

    if (credentials == null) {
      emit(
        const AuthError(
          UserMessageCodes.unexpected,
          errorCode: 'biometric_failed',
        ),
      );
      return;
    }

    final locale = event.locale ?? 'ar';
    final result = await _loginUseCase(
      LoginParams(
        login: credentials['login']!,
        password: credentials['password']!,
        locale: locale,
      ),
    );

    result.fold(
      (failure) {
        emit(
          AuthError(
            _messageForFailure(
              failure,
              serverFallback: UserMessageCodes.invalidLogin,
              unknownMessage: UserMessageCodes.invalidLogin,
            ),
          ),
        );
      },
      (loginResult) {
        // Save tokens to storage
        if (loginResult.accessToken != null) {
          AuthStorage.instance.saveAccessToken(loginResult.accessToken!);
        }
        if (loginResult.refreshToken != null) {
          AuthStorage.instance.saveRefreshToken(loginResult.refreshToken!);
        }
        if (loginResult.permissions != null &&
            loginResult.permissions!.hasPermission("confirm_trip_documents")) {
          AuthStorage.instance.saveisFinancialCollector(true);
        } else {
          AuthStorage.instance.saveisFinancialCollector(false);
        }
        if (credentials['password'] != null &&
            credentials['password']!.isNotEmpty) {
          AuthStorage.instance.saveauthPassword(credentials['password']);
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
      authenticateReason: event.localizedAuthenticateReason,
    );

    if (success) {
      emit(const AuthEnableBiometricSuccess());
    } else {
      emit(
        const AuthError(
          UserMessageCodes.unexpected,
          errorCode: 'biometric_enable_failed',
        ),
      );
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
      emit(
        const AuthError(
          UserMessageCodes.unexpected,
          errorCode: 'biometric_disable_failed',
        ),
      );
    }
  }
}
