import 'package:dartz/dartz.dart';

import '../entities/collector_profile.dart';
import '../entities/login_result.dart';
import '../failures/auth_failure.dart';

abstract interface class AuthRepository {
  /// Returns [LoginResult] on success. [locale] is used for the lang header (e.g. 'ar' for RTL).
  Future<Either<AuthFailure, LoginResult>> login({
    required String login,
    String? password,
    String? locale,
  });

  Future<Either<AuthFailure, void>> logout();

  Future<Either<AuthFailure, CollectorProfile>> getProfile();

  Future<Either<AuthFailure, void>> forgetPassword({
    required String identifier,
    required int channel,
  });
  Future<Either<AuthFailure, void>> resetPassword({
    required String token,
    required String newPassword,
    required String confirmPassword,
  });
  Future<Either<AuthFailure, void>> resetPasswordWithCode({
    required String code,
    required String login,
    required String newPassword,
    required String confirmPassword,
  });
  Future<Either<AuthFailure, void>> sendMfaCode({required int channel});
  Future<Either<AuthFailure, LoginResult>> verifyMfa({required String code});
}
