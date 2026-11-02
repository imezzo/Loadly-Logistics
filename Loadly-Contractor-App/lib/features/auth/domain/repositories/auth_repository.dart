import '../entities/register_document.dart';
import 'package:dartz/dartz.dart';

import '../entities/contractor_profile.dart';
import '../entities/login_result.dart';
import '../entities/send_login_otp_result.dart';
import '../failures/auth_failure.dart';
import '../entities/document_definition.dart';

abstract interface class AuthRepository {
  /// Returns [LoginResult] on success. [locale] is used for the lang header (e.g. 'ar' for RTL).
  Future<Either<AuthFailure, LoginResult>> login({
    required String login,
    String? password,
    String? locale,
  });

  Future<Either<AuthFailure, void>> register({
    required String fullName,
    required String phone,
    required String address,
    required String nationalId,
    required List<RegisterDocument> documents,
  });

  /// Sends OTP to phone number and returns token for login-with-otp
  Future<Either<AuthFailure, SendLoginOtpResult>> sendLoginOtp({
    required String login,
  });

  /// Logs in with OTP code using token from sendLoginOtp
  Future<Either<AuthFailure, LoginResult>> loginWithOtp({
    required String token,
    required String login,
    required String code,
  });

  Future<Either<AuthFailure, void>> logout();

  Future<Either<AuthFailure, ContractorProfile>> getProfile();

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

  Future<Either<AuthFailure, void>> changePassword({
    required String currentPassword,
    required String newPassword,
    required String confirmPassword,
  });

  Future<Either<AuthFailure, void>> sendMfaCode({required int channel});

  Future<Either<AuthFailure, LoginResult>> verifyMfa({required String code});

  Future<Either<AuthFailure, void>> enableMfa({required int channel});

  Future<Either<AuthFailure, void>> disableMfa();

  Future<Either<AuthFailure, ContractorProfile>> updateProfile({
    String? fullName,
    String? fullNameAr,
    String? userName,
    String? email,
    String? phone,
    String? profilePicturePath,
    bool? biometricFingerprint,
  });
  Future<Either<AuthFailure, bool>> validatePassword({
    required String password,
  });
  Future<Either<AuthFailure, List<DocumentDefinition>>> getDocuments({
    int? entityId,
  });
}
