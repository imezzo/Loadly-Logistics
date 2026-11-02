import 'package:dio/dio.dart';

import '../../../../../core/errors/user_message_codes.dart';
import '../../../../../core/network/api_endpoints.dart';
import '../../../../../core/network/api_response_parser.dart';
import '../../models/login_request_dto.dart';
import '../../models/login_response_dto.dart';
import '../../models/send_mfa_code_request_dto.dart';
import '../../models/verify_mfa_request_dto.dart';

abstract interface class AuthRemoteDataSource {
  /// Returns [LoginResponseDto] parsed from data envelope.
  /// Sends [lang] header: 'ar' when RTL (Arabic), else 'en'.
  Future<LoginResponseDto> login(LoginRequestDto request, {String? lang});
  Future<void> logout();
  Future<CollectorProfileDto> getProfile();
  Future<void> forgetPassword({
    required String identifier,
    required int channel,
  });
  Future<void> resetPassword({
    required String token,
    required String newPassword,
    required String confirmPassword,
  });
  Future<void> resetPasswordWithCode({
    required String code,
    required String login,
    required String newPassword,
    required String confirmPassword,
  });
  Future<void> sendMfaCode({required int channel});
  Future<LoginResponseDto> verifyMfa({required String code});
}

class AuthRemoteDataSourceImpl implements AuthRemoteDataSource {
  AuthRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  Future<Map<String, dynamic>> _postDataMap(
    String path, {
    Object? body,
    Options? options,
    String missingDataMessage = UserMessageCodes.missingData,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      path,
      data: body,
      options: options,
    );
    return requireEnvelopeDataMap(
      response,
      missingDataMessage: missingDataMessage,
    );
  }

  Future<void> _postSuccess(
    String path, {
    Object? body,
    Options? options,
    String fallbackMessage = UserMessageCodes.requestFailed,
  }) async {
    final response = await _dio.post<Map<String, dynamic>>(
      path,
      data: body,
      options: options,
    );
    throwIfEnvelopeFailed(response, fallbackMessage: fallbackMessage);
  }

  @override
  Future<LoginResponseDto> login(
    LoginRequestDto request, {
    String? lang,
  }) async {
    final headers = <String, dynamic>{'lang': ?lang};
    final dataObj = await _postDataMap(
      ApiEndpoints.login,
      body: request.toJson(),
      options: Options(headers: headers),
      missingDataMessage: 'Missing data in login response',
    );
    return LoginResponseDto.fromJson(dataObj);
  }

  @override
  Future<void> logout() async {
    await _dio.post(ApiEndpoints.logout);
  }

  @override
  Future<CollectorProfileDto> getProfile() async {
    final response = await _dio.get<Map<String, dynamic>>(ApiEndpoints.profile);
    final dataObj = requireEnvelopeDataMap(
      response,
      missingDataMessage: 'Missing data in profile response',
    );
    return CollectorProfileDto.fromJson(dataObj);
  }

  @override
  Future<void> forgetPassword({
    required String identifier,
    required int channel,
  }) async {
    await _postSuccess(
      ApiEndpoints.forgetPassword,
      body: {'identifier': identifier, 'channel': channel},
    );
  }

  @override
  Future<void> resetPassword({
    required String token,
    required String newPassword,
    required String confirmPassword,
  }) async {
    await _postSuccess(
      ApiEndpoints.resetPassword,
      body: {
        'token': token,
        'newPassword': newPassword,
        'confirmPassword': confirmPassword,
      },
    );
  }

  @override
  Future<void> resetPasswordWithCode({
    required String code,
    required String login,
    required String newPassword,
    required String confirmPassword,
  }) async {
    await _postSuccess(
      ApiEndpoints.resetPasswordWithCode,
      body: {
        'code': code,
        'identifier': login,
        'newPassword': newPassword,
        'confirmPassword': confirmPassword,
      },
    );
  }

  @override
  Future<void> sendMfaCode({required int channel}) async {
    await _postSuccess(
      ApiEndpoints.sendMfaCode,
      body: SendMfaCodeRequestDto(channel: channel).toJson(),
    );
  }

  @override
  Future<LoginResponseDto> verifyMfa({required String code}) async {
    final dataObj = await _postDataMap(
      ApiEndpoints.verifyMfa,
      body: VerifyMfaRequestDto(code: code).toJson(),
      missingDataMessage: 'Missing data in verify-mfa response',
    );
    return LoginResponseDto.fromJson(dataObj);
  }
}
