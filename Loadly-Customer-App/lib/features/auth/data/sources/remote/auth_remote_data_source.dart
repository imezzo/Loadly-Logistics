import 'package:dio/dio.dart';

import '../../../../../core/network/api_endpoints.dart';
import '../../models/login_request_dto.dart';
import '../../models/login_response_dto.dart';
import '../../models/send_login_otp_response_dto.dart';
import '../../models/login_with_otp_request_dto.dart';
import '../../models/send_mfa_code_request_dto.dart';
import '../../models/verify_mfa_request_dto.dart';
import '../../models/verify_email_request_dto.dart';
import '../../models/resend_verification_request_dto.dart';
import '../../models/enable_mfa_request_dto.dart';
import '../../models/update_profile_request_dto.dart';

abstract interface class AuthRemoteDataSource {
  /// Returns [LoginResponseDto] parsed from data envelope.
  /// Sends [lang] header: 'ar' when RTL (Arabic), else 'en'.
  Future<LoginResponseDto> login(LoginRequestDto request, {String? lang});
  Future<SendLoginOtpResponseDto> sendLoginOtp(LoginRequestDto request);
  Future<LoginResponseDto> loginWithOtp(LoginWithOtpRequestDto request);
  Future<void> logout();
  Future<CustomerProfileDto> getProfile();
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
    required String identifier,
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
  Future<void> verifyEmail({required String code});

  /// Requests a new verification code. [channel] 1 = Email, 2 = SMS. Omit or 1 for email.
  Future<void> resendVerification({int? channel});
  Future<void> enableMfa({required int channel});
  Future<void> disableMfa();
  Future<CustomerProfileDto> updateProfile(UpdateProfileRequestDto request);
  Future<bool> validatePassword({required String password});
}

class AuthRemoteDataSourceImpl implements AuthRemoteDataSource {
  AuthRemoteDataSourceImpl(this._dio);

  final Dio _dio;

  @override
  Future<LoginResponseDto> login(
    LoginRequestDto request, {
    String? lang,
  }) async {
    final headers = <String, dynamic>{'lang': ?lang};
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.login,
      data: request.toJson(),
      options: Options(headers: headers),
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
                'Request failed';
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
    final dataObj = data['data'] as Map<String, dynamic>?;
    if (dataObj == null) {
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: 'Missing data in login response',
      );
    }
    return LoginResponseDto.fromJson(dataObj);
  }

  @override
  Future<SendLoginOtpResponseDto> sendLoginOtp(LoginRequestDto request) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.sendLoginOtp,
      data: request.toJson(),
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
                'Request failed';
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
    final dataObj = data['data'] as Map<String, dynamic>?;
    if (dataObj == null) {
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: 'Missing data in send-login-otp response',
      );
    }
    return SendLoginOtpResponseDto.fromJson(dataObj);
  }

  @override
  Future<LoginResponseDto> loginWithOtp(LoginWithOtpRequestDto request) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.loginWithOtp,
      data: request.toJson(),
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
                'Request failed';
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
    final dataObj = data['data'] as Map<String, dynamic>?;
    if (dataObj == null) {
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: 'Missing data in login-with-otp response',
      );
    }
    return LoginResponseDto.fromJson(dataObj);
  }

  @override
  Future<void> logout() async {
    await _dio.post(ApiEndpoints.logout);
  }

  @override
  Future<CustomerProfileDto> getProfile() async {
    final response = await _dio.get<Map<String, dynamic>>(ApiEndpoints.profile);
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
                'Request failed';
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
    final dataObj = data['data'] as Map<String, dynamic>?;
    if (dataObj == null) {
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: 'Missing data in profile response',
      );
    }
    return CustomerProfileDto.fromJson(dataObj);
  }

  @override
  Future<void> forgetPassword({
    required String identifier,
    required int channel,
  }) async {
    await _dio.post(
      ApiEndpoints.forgetPassword,
      data: {'identifier': identifier, 'channel': channel},
    );
  }

  @override
  Future<void> resetPassword({
    required String token,
    required String newPassword,
    required String confirmPassword,
  }) async {
    await _dio.post(
      ApiEndpoints.resetPassword,
      data: {
        'token': token,
        'newPassword': newPassword,
        'confirmPassword': confirmPassword,
      },
    );
  }

  @override
  Future<void> resetPasswordWithCode({
    required String code,
    required String identifier,
    required String newPassword,
    required String confirmPassword,
  }) async {
    await _dio.post(
      ApiEndpoints.resetPasswordWithCode,
      data: {
        'code': code,
        'identifier': identifier,
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
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.changePassword,
      data: {
        'currentPassword': currentPassword,
        'newPassword': newPassword,
        'confirmPassword': confirmPassword,
      },
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
                'Request failed';
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
  }

  @override
  Future<void> sendMfaCode({required int channel}) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.sendMfaCode,
      data: SendMfaCodeRequestDto(channel: channel).toJson(),
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
                'Request failed';
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
  }

  @override
  Future<LoginResponseDto> verifyMfa({required String code}) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.verifyMfa,
      data: VerifyMfaRequestDto(code: code).toJson(),
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
                'Request failed';
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
    final dataObj = data['data'] as Map<String, dynamic>?;
    if (dataObj == null) {
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: 'Missing data in verify-mfa response',
      );
    }
    return LoginResponseDto.fromJson(dataObj);
  }

  @override
  Future<void> verifyEmail({required String code}) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.verifyEmail,
      data: VerifyEmailRequestDto(code: code).toJson(),
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
                'Request failed';
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
  }

  @override
  Future<void> resendVerification({int? channel}) async {
    final body = channel != null
        ? ResendVerificationRequestDto(channel: channel).toJson()
        : null;
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.resendVerification,
      data: body,
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
                'Request failed';
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
  }

  @override
  Future<void> enableMfa({required int channel}) async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.enableMfa,
      data: EnableMfaRequestDto(channel: channel).toJson(),
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
                'Request failed';
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
  }

  @override
  Future<void> disableMfa() async {
    final response = await _dio.post<Map<String, dynamic>>(
      ApiEndpoints.disableMfa,
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
                'Request failed';
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
  }

  @override
  Future<CustomerProfileDto> updateProfile(
    UpdateProfileRequestDto request,
  ) async {
    final formData = request.toFormData();
    final response = await _dio.put<Map<String, dynamic>>(
      ApiEndpoints.updateProfile,
      data: formData,
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
                'Request failed';
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
    final dataObj = data['data'] as Map<String, dynamic>?;
    if (dataObj == null) {
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: 'Missing data in update profile response',
      );
    }
    return CustomerProfileDto.fromJson(dataObj);
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
                'Request failed';
      throw DioException(
        requestOptions: response.requestOptions,
        response: response,
        type: DioExceptionType.badResponse,
        error: errorMessage,
      );
    }
    return success;
  }
}
