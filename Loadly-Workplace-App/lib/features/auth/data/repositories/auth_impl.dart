import 'package:dartz/dartz.dart';

import '../../../../core/errors/user_message_codes.dart';
import '../../../../core/network/api_failure_mapper.dart';
import '../../../../core/network/safe_api_call.dart';
import '../../domain/entities/collector_profile.dart';
import '../../domain/entities/login_result.dart';
import '../../domain/failures/auth_failure.dart';
import '../../domain/repositories/auth_repository.dart';
import '../models/login_response_dto.dart';
import '../sources/remote/auth_remote_data_source.dart';
import '../models/login_request_dto.dart';

/// Trims "+2" prefix from phone numbers (e.g. Egyptian +20) when displayed in profile.
String? _trimPhonePrefixPlus2(String? phone) {
  if (phone == null || phone.isEmpty) return phone;
  final t = phone.trim();
  if (t.startsWith('+2')) return t.substring(2).trim();
  return phone;
}

CollectorProfile _mapDriverProfile(CollectorProfileDto dto) {
  return CollectorProfile(
    id: dto.id,
    email: dto.email,
    fullName: dto.fullName,
    fullNameAr: dto.fullNameAr,
    nationalId: dto.nationalId,
    userName: dto.userName,
    phone: _trimPhonePrefixPlus2(dto.phone),
    userType: dto.userType,
    mfaChannel: dto.mfaChannel,
    emailVerified: dto.emailVerified,
    phoneVerified: dto.phoneVerified,
    status: dto.status,
    profilePicture: dto.profilePicture,
    driverRating: dto.driverRating,
    tripsCount: dto.tripsCount,
    biometricFingerprint: dto.biometricFingerprint,
    isOnline: dto.isOnline,
    documents: (dto.documents ?? const [])
        .map<DriverProfileDocument>(
          (doc) => DriverProfileDocument(
            id: doc.id,
            documentId: doc.documentId,
            fileUrl: doc.fileUrl,
            expiryDate: doc.expiryDate,
            status: doc.status,
            document: doc.document == null
                ? null
                : DriverProfileDocumentType(
                    id: doc.document!.id,
                    nameEn: doc.document!.nameEn,
                    nameAr: doc.document!.nameAr,
                  ),
          ),
        )
        .toList(),
  );
}

class AuthImpl implements AuthRepository {
  AuthImpl(this._remote);

  final AuthRemoteDataSource _remote;

  Future<Either<AuthFailure, T>> _run<T>(
    Future<T> Function() request, {
    bool invalidCredentialsOnUnauthorized = false,
    String? unauthorizedServerMessage,
  }) {
    return safeApiCall<T, AuthFailure>(
      request: request,
      mapFailure: (details) {
        return switch (details.kind) {
          ApiFailureKind.network => const AuthFailureNetwork(),
          ApiFailureKind.unauthorized when invalidCredentialsOnUnauthorized =>
            const AuthFailureInvalidCredentials(),
          ApiFailureKind.unauthorized ||
          ApiFailureKind.server => AuthFailureServer(
            message: details.message ?? unauthorizedServerMessage,
            statusCode: details.statusCode,
          ),
          ApiFailureKind.unknown => AuthFailureUnknown(
            message: details.message,
          ),
        };
      },
      mapUnknownFailure: (_) => const AuthFailureUnknown(),
    );
  }

  @override
  Future<Either<AuthFailure, LoginResult>> login({
    required String login,
    String? password,
    String? locale,
  }) async {
    return _run(() async {
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
        refreshToken: response.refreshToken,
        redirect: response.redirect,
        mfaChannel: response.user?.mfaChannel,
        permissions: response.user?.permissions,
        phoneForOtp:
            (response.redirect == 'forget_password' ||
                response.redirect == 'mfa')
            ? login
            : null,
        email: response.redirect == 'mfa'
            ? (response.user?.email ?? (isPhone ? null : login))
            : null,
        phone: response.redirect == 'mfa' ? (isPhone ? login : null) : null,
        isOnline: response.isOnline,
      );
      return result;
    }, unauthorizedServerMessage: UserMessageCodes.invalidLogin);
  }

  @override
  Future<Either<AuthFailure, void>> logout() async {
    return _run(() async {
      await _remote.logout();
    });
  }

  @override
  Future<Either<AuthFailure, CollectorProfile>> getProfile() async {
    return _run(() async {
      final dto = await _remote.getProfile();
      return _mapDriverProfile(dto);
    });
  }

  @override
  Future<Either<AuthFailure, void>> forgetPassword({
    required String identifier,
    required int channel,
  }) async {
    return _run(() async {
      await _remote.forgetPassword(identifier: identifier, channel: channel);
    });
  }

  @override
  Future<Either<AuthFailure, void>> resetPassword({
    required String token,
    required String newPassword,
    required String confirmPassword,
  }) async {
    return _run(() async {
      await _remote.resetPassword(
        token: token,
        newPassword: newPassword,
        confirmPassword: confirmPassword,
      );
    });
  }

  @override
  Future<Either<AuthFailure, void>> resetPasswordWithCode({
    required String code,
    required String login,
    required String newPassword,
    required String confirmPassword,
  }) async {
    return _run(() async {
      await _remote.resetPasswordWithCode(
        code: code,
        login: login,
        newPassword: newPassword,
        confirmPassword: confirmPassword,
      );
    });
  }

  @override
  Future<Either<AuthFailure, void>> sendMfaCode({required int channel}) async {
    return _run(() async {
      await _remote.sendMfaCode(channel: channel);
    });
  }

  @override
  Future<Either<AuthFailure, LoginResult>> verifyMfa({
    required String code,
  }) async {
    return _run(() async {
      final response = await _remote.verifyMfa(code: code);
      final result = LoginResult(
        message: 'MFA verification successful',
        accessToken: response.accessToken,
        refreshToken: response.refreshToken,
        redirect: response.redirect,
        mfaChannel: response.user?.mfaChannel,
        permissions: response.user?.permissions,
        phoneForOtp: response.redirect == 'forget_password' ? null : null,
        isOnline: response.isOnline,
      );
      return result;
    }, invalidCredentialsOnUnauthorized: true);
  }
}
