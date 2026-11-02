import 'package:dartz/dartz.dart';
import 'package:dio/dio.dart';

import 'dart:io';

import '../../domain/entities/customer_profile.dart';
import '../../domain/entities/login_result.dart';
import '../../domain/entities/send_login_otp_result.dart';
import '../../domain/failures/auth_failure.dart';
import '../../domain/repositories/auth_repository.dart';
import '../sources/remote/auth_remote_data_source.dart';
import '../models/login_request_dto.dart';
import '../models/update_profile_request_dto.dart';
import '../models/login_with_otp_request_dto.dart';

/// Trims "+2" prefix from phone numbers (e.g. Egyptian +20) when displayed in profile.
String? _trimPhonePrefixPlus2(String? phone) {
  if (phone == null || phone.isEmpty) return phone;
  final t = phone.trim();
  if (t.startsWith('+2')) return t.substring(2).trim();
  return phone;
}

/// Extracts a user-facing message from API error response (message, errors.message, or errors list).
String? _messageFromResponse(dynamic data) {
  if (data == null) return null;
  if (data is! Map) return null;
  final message = data['message'];
  if (message is String && message.isNotEmpty) return message;
  final errors = data['errors'];
  if (errors is Map && errors['message'] != null) {
    return errors['message'].toString();
  }
  if (errors is List && errors.isNotEmpty) return errors.first.toString();
  return null;
}

class AuthImpl implements AuthRepository {
  AuthImpl(this._remote);

  final AuthRemoteDataSource _remote;

  AuthFailure _serverFailure(DioException e) {
    final statusCode = e.response?.statusCode;
    final message = e.error is String
        ? e.error as String
        : _messageFromResponse(e.response?.data);
    return AuthFailureServer(message: message, statusCode: statusCode);
  }

  @override
  Future<Either<AuthFailure, LoginResult>> login({
    required String login,
    String? password,
    String? locale,
  }) async {
    try {
      final lang = locale != null
          ? (locale.toLowerCase().startsWith('ar') ? 'ar' : 'en')
          : null;
      final response = await _remote.login(
        LoginRequestDto(login: login, password: password),
        lang: lang,
      );
      final isPhone = !login.contains('@');
      final result = LoginResult(
        message: 'Login successful',
        accessToken: response.accessToken,
        redirect: response.redirect,
        mfaChannel: response.user?.mfaChannel,
        phoneForOtp:
            (response.redirect == 'forget_password' ||
                response.redirect == 'mfa')
            ? login
            : null,
        email: response.redirect == 'mfa'
            ? (response.user?.email ?? (isPhone ? null : login))
            : null,
        phone: response.redirect == 'mfa' ? (isPhone ? login : null) : null,
        refreshToken: response.refreshToken,
      );
      return right(result);
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) {
        final message = _messageFromResponse(e.response?.data);
        return left(
          AuthFailureServer(
            message: message ?? 'Invalid credentials. Please try again.',
            statusCode: 401,
          ),
        );
      }
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }

  @override
  Future<Either<AuthFailure, SendLoginOtpResult>> sendLoginOtp({
    required String login,
  }) async {
    try {
      final response = await _remote.sendLoginOtp(
        LoginRequestDto(login: login),
      );
      return right(SendLoginOtpResult(token: response.token));
    } on DioException catch (e) {
      // if (e.response?.statusCode == 401) {
      //   return left(const AuthFailureInvalidCredentials());
      // }
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }

  @override
  Future<Either<AuthFailure, LoginResult>> loginWithOtp({
    required String token,
    required String login,
    required String code,
  }) async {
    try {
      final response = await _remote.loginWithOtp(
        LoginWithOtpRequestDto(token: token, login: login, code: code),
      );
      final isPhone = !login.contains('@');
      final result = LoginResult(
        message: 'Login successful',
        accessToken: response.accessToken,
        redirect: response.redirect,
        mfaChannel: response.user?.mfaChannel,
        phoneForOtp:
            (response.redirect == 'forget_password' ||
                response.redirect == 'mfa')
            ? login
            : null,
        email: response.redirect == 'mfa'
            ? (response.user?.email ?? (isPhone ? null : login))
            : null,
        phone: response.redirect == 'mfa' ? (isPhone ? login : null) : null,
        refreshToken: response.refreshToken,
      );
      return right(result);
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) {
        return left(const AuthFailureInvalidCredentials());
      }
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }

  @override
  Future<Either<AuthFailure, void>> logout() async {
    try {
      await _remote.logout();
      return right(null);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }

  @override
  Future<Either<AuthFailure, CustomerProfile>> getProfile() async {
    try {
      final dto = await _remote.getProfile();
      return right(
        CustomerProfile(
          id: dto.id,
          email: dto.email,
          fullName: dto.fullName,
          userName: dto.userName,
          phone: _trimPhonePrefixPlus2(dto.phone),
          userType: dto.userType,
          mfaChannel: dto.mfaChannel,
          emailVerified: dto.emailVerified,
          phoneVerified: dto.phoneVerified,
          status: dto.status,
          profilePicture: dto.profilePicture,
          biometricFingerprint: dto.biometricFingerprint,
        ),
      );
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }

  @override
  Future<Either<AuthFailure, void>> forgetPassword({
    required String identifier,
    required int channel,
  }) async {
    try {
      await _remote.forgetPassword(identifier: identifier, channel: channel);
      return right(null);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }

  @override
  Future<Either<AuthFailure, void>> resetPassword({
    required String token,
    required String newPassword,
    required String confirmPassword,
  }) async {
    try {
      await _remote.resetPassword(
        token: token,
        newPassword: newPassword,
        confirmPassword: confirmPassword,
      );
      return right(null);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }

  @override
  Future<Either<AuthFailure, void>> resetPasswordWithCode({
    required String code,
    required String login,
    required String newPassword,
    required String confirmPassword,
  }) async {
    try {
      await _remote.resetPasswordWithCode(
        code: code,
        identifier: login,
        newPassword: newPassword,
        confirmPassword: confirmPassword,
      );
      return right(null);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }

  @override
  Future<Either<AuthFailure, void>> changePassword({
    required String currentPassword,
    required String newPassword,
    required String confirmPassword,
  }) async {
    try {
      await _remote.changePassword(
        currentPassword: currentPassword,
        newPassword: newPassword,
        confirmPassword: confirmPassword,
      );
      return right(null);
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) {
        return left(
          AuthFailureInvalidCredentials(
            message: e.error is String
                ? e.error as String
                : _messageFromResponse(e.response?.data),
          ),
        );
      }
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }

  @override
  Future<Either<AuthFailure, void>> sendMfaCode({required int channel}) async {
    try {
      await _remote.sendMfaCode(channel: channel);
      return right(null);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }

  @override
  Future<Either<AuthFailure, LoginResult>> verifyMfa({
    required String code,
  }) async {
    try {
      final response = await _remote.verifyMfa(code: code);
      final result = LoginResult(
        message: 'MFA verification successful',
        accessToken: response.accessToken,
        redirect: response.redirect,
        mfaChannel: response.user?.mfaChannel,
        phoneForOtp: response.redirect == 'forget_password' ? null : null,
        refreshToken: response.refreshToken,
      );
      return right(result);
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) {
        return left(const AuthFailureInvalidCredentials());
      }
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }

  @override
  Future<Either<AuthFailure, void>> verifyEmail({required String code}) async {
    try {
      await _remote.verifyEmail(code: code);
      return right(null);
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) {
        return left(const AuthFailureInvalidCredentials());
      }
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }

  @override
  Future<Either<AuthFailure, void>> resendVerification({int? channel}) async {
    try {
      await _remote.resendVerification(channel: channel);
      return right(null);
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) {
        return left(const AuthFailureInvalidCredentials());
      }
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }

  @override
  Future<Either<AuthFailure, void>> enableMfa({required int channel}) async {
    try {
      await _remote.enableMfa(channel: channel);
      return right(null);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }

  @override
  Future<Either<AuthFailure, void>> disableMfa() async {
    try {
      await _remote.disableMfa();
      return right(null);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }

  @override
  Future<Either<AuthFailure, CustomerProfile>> updateProfile({
    String? fullName,
    String? fullNameAr,
    String? userName,
    String? email,
    String? phone,
    String? profilePicturePath,
    bool? biometricFingerprint,
  }) async {
    try {
      final profilePictureFile =
          profilePicturePath != null && profilePicturePath.isNotEmpty
          ? File(profilePicturePath)
          : null;

      final dto = await _remote.updateProfile(
        UpdateProfileRequestDto(
          fullName: fullName,
          fullNameAr: fullNameAr,
          userName: userName,
          email: email,
          phone: phone,
          profilePicture: profilePictureFile,
          biometricFingerprint: biometricFingerprint,
        ),
      );

      return right(
        CustomerProfile(
          id: dto.id,
          email: dto.email,
          fullName: dto.fullName,
          userName: dto.userName,
          phone: _trimPhonePrefixPlus2(dto.phone),
          userType: dto.userType,
          mfaChannel: dto.mfaChannel,
          emailVerified: dto.emailVerified,
          phoneVerified: dto.phoneVerified,
          status: dto.status,
          profilePicture: dto.profilePicture,
          biometricFingerprint: dto.biometricFingerprint,
        ),
      );
    } on DioException catch (e) {
      if (e.response?.statusCode == 401) {
        return left(const AuthFailureInvalidCredentials());
      }
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }

  @override
  Future<Either<AuthFailure, bool>> validatePassword({
    required String password,
  }) async {
    try {
      final success = await _remote.validatePassword(password: password);
      return right(success);
    } on DioException catch (e) {
      if (e.type == DioExceptionType.connectionTimeout ||
          e.type == DioExceptionType.connectionError) {
        return left(const AuthFailureNetwork());
      }
      return left(_serverFailure(e));
    } catch (_) {
      return left(const AuthFailureUnknown());
    }
  }
}
