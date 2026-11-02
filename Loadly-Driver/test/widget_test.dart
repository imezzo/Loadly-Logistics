import 'package:flutter/cupertino.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:flutter_bloc/flutter_bloc.dart';

import 'package:loadly_driver/core/locale/bloc/locale_bloc.dart';
import 'package:loadly_driver/core/network/api_client.dart';
import 'package:loadly_driver/features/auth/data/sources/remote/auth_remote_data_source.dart';
import 'package:loadly_driver/features/auth/data/repositories/auth_impl.dart';
import 'package:loadly_driver/features/auth/domain/use_cases/forget_password_usecase.dart';
import 'package:loadly_driver/features/auth/domain/use_cases/login_usecase.dart';
import 'package:loadly_driver/features/auth/domain/use_cases/login_with_otp_usecase.dart';
import 'package:loadly_driver/features/auth/domain/use_cases/logout_usecase.dart';
import 'package:loadly_driver/features/auth/domain/use_cases/reset_password_usecase.dart';
import 'package:loadly_driver/features/auth/domain/use_cases/send_login_otp_usecase.dart';
import 'package:loadly_driver/features/auth/domain/use_cases/send_mfa_code_usecase.dart';
import 'package:loadly_driver/features/auth/domain/use_cases/validate_password_usecase.dart';
import 'package:loadly_driver/features/auth/domain/use_cases/verify_mfa_usecase.dart';
import 'package:loadly_driver/features/auth/domain/use_cases/enable_mfa_usecase.dart';
import 'package:loadly_driver/features/auth/domain/use_cases/disable_mfa_usecase.dart';
import 'package:loadly_driver/features/auth/domain/use_cases/change_password_usecase.dart';
import 'package:loadly_driver/features/auth/domain/use_cases/verify_email_usecase.dart';
import 'package:loadly_driver/features/auth/domain/use_cases/resend_verification_usecase.dart';
import 'package:loadly_driver/features/auth/presentation/bloc/auth_bloc.dart';
import 'package:loadly_driver/main.dart';

void main() {
  testWidgets('App starts with Cupertino', (WidgetTester tester) async {
    final dio = createDriverApiClient();
    final authRemoteDataSource = AuthRemoteDataSourceImpl(dio);
    final authRepository = AuthImpl(authRemoteDataSource);

    // Initialize use cases
    final loginUseCase = LoginUseCase(authRepository);
    final sendLoginOtpUseCase = SendLoginOtpUseCase(authRepository);
    final loginWithOtpUseCase = LoginWithOtpUseCase(authRepository);
    final forgetPasswordUseCase = ForgetPasswordUseCase(authRepository);
    final resetPasswordUseCase = ResetPasswordUseCase(authRepository);
    final resetPasswordWithCodeUseCase = ResetPasswordWithCodeUseCase(
      authRepository,
    );
    final logoutUseCase = LogoutUseCase(authRepository);
    final sendMfaCodeUseCase = SendMfaCodeUseCase(authRepository);
    final verifyMfaUseCase = VerifyMfaUseCase(authRepository);
    final enableMfaUseCase = EnableMfaUseCase(authRepository);
    final disableMfaUseCase = DisableMfaUseCase(authRepository);
    final changePasswordUseCase = ChangePasswordUseCase(authRepository);
    final verifyEmailUseCase = VerifyEmailUseCase(authRepository);
    final resendVerificationUseCase = ResendVerificationUseCase(authRepository);
    final validatePasswordUseCase = ValidatePasswordUseCase(authRepository);

    // Initialize BloC with use cases
    final authBloc = AuthBloc(
      loginUseCase: loginUseCase,
      sendLoginOtpUseCase: sendLoginOtpUseCase,
      loginWithOtpUseCase: loginWithOtpUseCase,
      forgetPasswordUseCase: forgetPasswordUseCase,
      resetPasswordUseCase: resetPasswordUseCase,
      resetPasswordWithCodeUseCase: resetPasswordWithCodeUseCase,
      logoutUseCase: logoutUseCase,
      sendMfaCodeUseCase: sendMfaCodeUseCase,
      verifyMfaUseCase: verifyMfaUseCase,
      enableMfaUseCase: enableMfaUseCase,
      disableMfaUseCase: disableMfaUseCase,
      changePasswordUseCase: changePasswordUseCase,
      verifyEmailUseCase: verifyEmailUseCase,
      resendVerificationUseCase: resendVerificationUseCase,
      validatePasswordUseCase: validatePasswordUseCase,
    );
    final localeBloc = LocaleBloc();

    await tester.pumpWidget(
      MultiBlocProvider(
        providers: [
          BlocProvider.value(value: authBloc),
          BlocProvider.value(value: localeBloc),
        ],
        child: const LoadlyDriverApp(),
      ),
    );
    await tester.pump();
    // Pump enough time to let the splash screen timer complete (2 seconds)
    await tester.pump(const Duration(seconds: 2));
    await tester.pump(); // Pump one more frame to process navigation
    expect(find.byType(CupertinoApp), findsOneWidget);
  });
}
