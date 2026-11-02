import 'dart:io';

import 'package:dartz/dartz.dart';

import '../../../../core/errors/api_fallback_keys.dart';
import '../../../../core/network/api_failure_mapper.dart';
import '../../../../core/network/safe_api_call.dart';
import '../../../../core/utils/string_utils.dart';
import '../../domain/entities/contractor_profile.dart';
import '../../domain/entities/login_result.dart';
import '../../domain/entities/send_login_otp_result.dart';
import '../../domain/failures/auth_failure.dart';
import '../../domain/repositories/auth_repository.dart';
import '../models/login_response_dto.dart';
import '../sources/remote/auth_remote_data_source.dart';
import '../models/login_request_dto.dart';
import '../models/update_profile_request_dto.dart';
import '../models/login_with_otp_request_dto.dart';
import '../../domain/entities/register_document.dart';
import '../models/register_document_model.dart';
import '../../domain/entities/document_definition.dart';

/// Trims "+2" prefix from phone numbers (e.g. Egyptian +20) when displayed in profile.
String? _trimPhonePrefixPlus2(String? phone) {
  if (phone == null || phone.isEmpty) return phone;
  final t = phone.trim();
  if (t.startsWith('+2')) return t.substring(2).trim();
  return phone;
}

/// Ensures phone is sent with +2 prefix to the API.
String? _phoneWithPlus2(String? phone) {
  if (phone == null || phone.isEmpty) return null;
  final digits = extractDigits(phone);
  if (digits.isEmpty) return null;
  // Already has country code 20 (e.g. +201234567890)
  if (digits.startsWith('20')) return '+$digits';
  return '+2$digits';
}

ContractorProfile _mapContractorProfile(ContractorProfileDto dto) {
  return ContractorProfile(
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
        .map<ContractorProfileDocument>(
          (doc) => ContractorProfileDocument(
            id: doc.id,
            documentId: doc.documentId,
            fileUrl: doc.fileUrl,
            expiryDate: doc.expiryDate,
            status: doc.status,
            document: doc.document == null
                ? null
                : ContractorProfileDocumentType(
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
            AuthFailureInvalidCredentials(message: details.message),
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
        userId: response.user?.id,
      );
      return result;
    }, unauthorizedServerMessage: ApiFallbackKeys.invalidCredentials);
  }

  @override
  Future<Either<AuthFailure, SendLoginOtpResult>> sendLoginOtp({
    required String login,
  }) async {
    return _run(() async {
      final response = await _remote.sendLoginOtp(
        LoginRequestDto(login: login),
      );
      return SendLoginOtpResult(token: response.token);
    }, invalidCredentialsOnUnauthorized: true);
  }

  @override
  Future<Either<AuthFailure, LoginResult>> loginWithOtp({
    required String token,
    required String login,
    required String code,
  }) async {
    return _run(() async {
      final response = await _remote.loginWithOtp(
        LoginWithOtpRequestDto(token: token, login: login, code: code),
      );
      final isPhone = !login.contains('@');
      final result = LoginResult(
        message: 'Login successful',
        accessToken: response.accessToken,
        refreshToken: response.refreshToken,
        redirect: response.redirect,
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
        userId: response.user?.id,
      );
      return result;
    }, invalidCredentialsOnUnauthorized: true);
  }

  @override
  Future<Either<AuthFailure, void>> logout() async {
    return _run(() async {
      await _remote.logout();
    });
  }

  @override
  Future<Either<AuthFailure, ContractorProfile>> getProfile() async {
    return _run(() async {
      final dto = await _remote.getProfile();
      return _mapContractorProfile(dto);
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
  Future<Either<AuthFailure, void>> changePassword({
    required String currentPassword,
    required String newPassword,
    required String confirmPassword,
  }) async {
    return _run(() async {
      await _remote.changePassword(
        currentPassword: currentPassword,
        newPassword: newPassword,
        confirmPassword: confirmPassword,
      );
    }, invalidCredentialsOnUnauthorized: true);
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
        phoneForOtp: response.redirect == 'forget_password' ? null : null,
        isOnline: response.isOnline,
        userId: response.user?.id,
      );
      return result;
    }, invalidCredentialsOnUnauthorized: true);
  }

  @override
  Future<Either<AuthFailure, void>> enableMfa({required int channel}) async {
    return _run(() async {
      await _remote.enableMfa(channel: channel);
    });
  }

  @override
  Future<Either<AuthFailure, void>> disableMfa() async {
    return _run(() async {
      await _remote.disableMfa();
    });
  }

  @override
  Future<Either<AuthFailure, bool>> validatePassword({
    required String password,
  }) async {
    return _run(() async {
      return await _remote.validatePassword(password: password);
    });
  }

  @override
  Future<Either<AuthFailure, void>> register({
    required String fullName,
    required String phone,
    required String address,
    required String nationalId,
    required List<RegisterDocument> documents,
  }) async {
    return _run(() async {
      final documentModels = documents
          .map(
            (document) => RegisterDocumentModel(
              documentId: document.documentId,
              filePath: document.filePath,
              expiryDate: document.expiryDate,
            ),
          )
          .toList();

      await _remote.register(
        fullName: fullName,
        phone: phone,
        address: address,
        nationalId: nationalId,
        documents: documentModels,
      );
    });
  }

  @override
  Future<Either<AuthFailure, ContractorProfile>> updateProfile({
    String? fullName,
    String? fullNameAr,
    String? userName,
    String? email,
    String? phone,
    String? profilePicturePath,
    bool? biometricFingerprint,
  }) async {
    return _run(() async {
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
          phone: _phoneWithPlus2(phone),
          profilePicture: profilePictureFile,
          biometricFingerprint: biometricFingerprint,
        ),
      );
      return _mapContractorProfile(dto);
    }, invalidCredentialsOnUnauthorized: true);
  }

  @override
  Future<Either<AuthFailure, List<DocumentDefinition>>> getDocuments({
    int? entityId,
  }) async {
    return _run(() async {
      final dtos = await _remote.getDocuments(entityId: entityId);

      return dtos
          .map(
            (dto) => DocumentDefinition(
              id: dto.id,
              entityId: dto.entityId,
              nameEn: dto.nameEn,
              nameAr: dto.nameAr,
              required: dto.required,
              status: dto.status,
            ),
          )
          .toList();
    });
  }
}
