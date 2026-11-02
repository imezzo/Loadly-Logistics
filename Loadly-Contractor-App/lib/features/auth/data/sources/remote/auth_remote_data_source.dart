import 'package:dio/dio.dart';

import '../../../../../core/errors/api_fallback_keys.dart';
import '../../../../../core/network/api_endpoints.dart';
import '../../../../../core/network/api_response_parser.dart';
import '../../models/login_request_dto.dart';
import '../../models/login_response_dto.dart';
import '../../models/send_login_otp_response_dto.dart';
import '../../models/login_with_otp_request_dto.dart';
import '../../models/send_mfa_code_request_dto.dart';
import '../../models/verify_mfa_request_dto.dart';
import '../../models/enable_mfa_request_dto.dart';
import '../../models/update_profile_request_dto.dart';
import '../../models/register_document_model.dart';
import '../../models/document_definition_dto.dart';

abstract interface class AuthRemoteDataSource {
  /// Returns [LoginResponseDto] parsed from data envelope.
  /// Sends [lang] header: 'ar' when RTL (Arabic), else 'en'.
  Future<List<DocumentDefinitionDto>> getDocuments({int? entityId});
  Future<LoginResponseDto> login(LoginRequestDto request, {String? lang});
  Future<SendLoginOtpResponseDto> sendLoginOtp(LoginRequestDto request);
  Future<LoginResponseDto> loginWithOtp(LoginWithOtpRequestDto request);
  Future<void> logout();
  Future<ContractorProfileDto> getProfile();
  Future<void> forgetPassword({
    required String identifier,
    required int channel,
  });
  Future<void> register({
    required String fullName,
    required String phone,
    required String address,
    required String nationalId,

    required List<RegisterDocumentModel> documents,
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
  Future<void> changePassword({
    required String currentPassword,
    required String newPassword,
    required String confirmPassword,
  });
  Future<void> sendMfaCode({required int channel});
  Future<LoginResponseDto> verifyMfa({required String code});
  Future<void> enableMfa({required int channel});
  Future<void> disableMfa();
  Future<ContractorProfileDto> updateProfile(UpdateProfileRequestDto request);
  Future<bool> validatePassword({required String password});
}

class AuthRemoteDataSourceImpl implements AuthRemoteDataSource {
  AuthRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  Future<Map<String, dynamic>> _postDataMap(
    String path, {
    Object? body,
    Options? options,
    String missingDataMessage = ApiFallbackKeys.invalidResponse,
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
    String fallbackMessage = ApiFallbackKeys.requestFailed,
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
      missingDataMessage: ApiFallbackKeys.invalidResponse,
    );
    return LoginResponseDto.fromJson(dataObj);
  }

  @override
  Future<SendLoginOtpResponseDto> sendLoginOtp(LoginRequestDto request) async {
    final dataObj = await _postDataMap(
      ApiEndpoints.sendLoginOtp,
      body: request.toJson(),
      missingDataMessage: ApiFallbackKeys.invalidResponse,
    );
    return SendLoginOtpResponseDto.fromJson(dataObj);
  }

  @override
  Future<LoginResponseDto> loginWithOtp(LoginWithOtpRequestDto request) async {
    final dataObj = await _postDataMap(
      ApiEndpoints.loginWithOtp,
      body: request.toJson(),
      missingDataMessage: ApiFallbackKeys.invalidResponse,
    );
    return LoginResponseDto.fromJson(dataObj);
  }

  @override
  Future<void> logout() async {
    await _dio.post(ApiEndpoints.logout);
  }

  @override
  Future<ContractorProfileDto> getProfile() async {
    final response = await _dio.get<Map<String, dynamic>>(ApiEndpoints.profile);
    final dataObj = requireEnvelopeDataMap(
      response,
      missingDataMessage: ApiFallbackKeys.invalidResponse,
    );
    return ContractorProfileDto.fromJson(dataObj);
  }

  @override
  Future<List<DocumentDefinitionDto>> getDocuments({int? entityId}) async {
    final response = await _dio.get<Map<String, dynamic>>(
      ApiEndpoints.documents,
      queryParameters: {
        'Page': 1,
        'PageSize': entityId == null ? 100 : 15,
        'Search': '',
        'Status': 1,
        if (entityId == null) 'Required': '',
        'entityId': ?entityId,
      },
    );

    final envelope = requireEnvelope(response);

    final data = envelope['data'];

    if (data is! List) {
      throw Exception(ApiFallbackKeys.invalidResponse);
    }

    return data.whereType<Map>().map((item) {
      return DocumentDefinitionDto.fromJson(Map<String, dynamic>.from(item));
    }).toList();
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
  Future<void> changePassword({
    required String currentPassword,
    required String newPassword,
    required String confirmPassword,
  }) async {
    await _postSuccess(
      ApiEndpoints.changePassword,
      body: {
        'currentPassword': currentPassword,
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
      missingDataMessage: ApiFallbackKeys.invalidResponse,
    );
    return LoginResponseDto.fromJson(dataObj);
  }

  @override
  Future<void> enableMfa({required int channel}) async {
    await _postSuccess(
      ApiEndpoints.enableMfa,
      body: EnableMfaRequestDto(channel: channel).toJson(),
    );
  }

  @override
  Future<void> disableMfa() async {
    await _postSuccess(ApiEndpoints.disableMfa);
  }

  @override
  Future<ContractorProfileDto> updateProfile(
    UpdateProfileRequestDto request,
  ) async {
    final formData = request.toFormData();
    final response = await _dio.put<Map<String, dynamic>>(
      ApiEndpoints.updateProfile,
      data: formData,
    );
    final envelope = requireEnvelope(response);
    if ((envelope['success'] as bool? ?? false) != true) {
      throwIfEnvelopeFailed(response);
    }
    final dataObj = envelope['data'] as Map<String, dynamic>?;
    if (dataObj != null) {
      return ContractorProfileDto.fromJson(dataObj);
    }
    // API returns success with data: null (e.g. "Authentication operation completed")
    // Fetch the profile to return updated data
    return getProfile();
  }

  @override
  Future<bool> validatePassword({required String password}) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.validatePassword,
      data: {'password': password},
    );
    final data = response.data;
    if (data == null) {
      throw DioException(requestOptions: response.requestOptions);
    }
    final success = data['success'] as bool? ?? false;
    final message = data['message'] as String? ?? '';
    if (!success) {
      final errors = data['errors'];
      final errorMessage = message.isNotEmpty
          ? message
          : (errors is Map
                        ? (errors['message'] ?? errors.toString())
                        : data['message'])
                    ?.toString() ??
                ApiFallbackKeys.requestFailed;
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
    return success;
  }

  @override
  Future<void> register({
    required String fullName,
    required String phone,
    required String address,
    required String nationalId,
    required List<RegisterDocumentModel> documents,
  }) async {
    final formData = FormData();

    formData.fields.add(MapEntry('FullName', fullName));

    formData.fields.add(MapEntry('Phone', phone));

    formData.fields.add(MapEntry('Address', address));

    formData.fields.add(MapEntry('NationalId', nationalId));

    for (var i = 0; i < documents.length; i++) {
      final document = documents[i];

      formData.fields.add(
        MapEntry('Documents[$i].DocumentId', document.documentId.toString()),
      );

      formData.fields.add(
        MapEntry(
          'Documents[$i].ExpiryDate',
          document.expiryDate.toUtc().toIso8601String(),
        ),
      );

      formData.files.add(
        MapEntry(
          'Documents[$i].File',
          await MultipartFile.fromFile(document.filePath),
        ),
      );
    }

    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.register,
      data: formData,
    );

    throwIfEnvelopeFailed(response, fallbackMessage: ApiFallbackKeys.registrationFailed);
  }
}
