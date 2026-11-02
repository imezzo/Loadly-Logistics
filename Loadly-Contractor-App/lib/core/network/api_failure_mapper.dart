import 'dart:convert';

import 'package:dio/dio.dart';

enum ApiFailureKind { network, unauthorized, server, unknown }

class ApiFailureDetails {
  const ApiFailureDetails({required this.kind, this.message, this.statusCode});

  final ApiFailureKind kind;
  final String? message;
  final int? statusCode;
}

ApiFailureDetails mapDioExceptionToFailure(DioException error) {
  final statusCode = error.response?.statusCode;
  if (_isGatewayOrServerError(statusCode)) {
    return ApiFailureDetails(
      kind: ApiFailureKind.server,
      message: null,
      statusCode: statusCode,
    );
  }

  final message = _extractMessage(error);

  if (statusCode == 401) {
    return ApiFailureDetails(
      kind: ApiFailureKind.unauthorized,
      message: message,
      statusCode: statusCode,
    );
  }

  if (_isNetworkError(error.type)) {
    return const ApiFailureDetails(kind: ApiFailureKind.network);
  }

  return ApiFailureDetails(
    kind: ApiFailureKind.server,
    message: message,
    statusCode: statusCode,
  );
}

bool _isNetworkError(DioExceptionType type) {
  return switch (type) {
    DioExceptionType.connectionTimeout ||
    DioExceptionType.sendTimeout ||
    DioExceptionType.receiveTimeout ||
    DioExceptionType.connectionError => true,
    _ => false,
  };
}

String? userFacingDioMessage(DioException error) {
  final mapped = mapDioExceptionToFailure(error).message;
  return isUserFacingApiMessage(mapped) ? mapped : null;
}

bool isHtmlContent(String text) {
  final lower = text.trim().toLowerCase();
  if (lower.isEmpty) return false;
  return lower.startsWith('<!doctype') ||
      lower.startsWith('<html') ||
      lower.contains('<head') ||
      lower.contains('<body') ||
      lower.contains('</html>') ||
      lower.contains('<center>') ||
      RegExp(r'<[a-z][\s\S]*>', caseSensitive: false).hasMatch(lower);
}

bool isUserFacingApiMessage(String? message) {
  if (message == null) return false;
  final text = message.trim();
  if (text.isEmpty) return false;
  if (text.length > 180) return false;
  if (isHtmlContent(text)) return false;
  final lower = text.toLowerCase();
  return !lower.contains('this exception was thrown') &&
      !lower.contains('validatestatus') &&
      !lower.contains('dioexception') &&
      !lower.contains('requestoptions') &&
      !lower.contains('status code of') &&
      !lower.contains('xmlhttprequest');
}

bool _isGatewayOrServerError(int? statusCode) {
  if (statusCode == null) return false;
  return statusCode == 502 || statusCode == 503 || statusCode == 504;
}

bool _isGenericFallbackMessage(String message) {
  final lower = message.trim().toLowerCase();
  return lower.startsWith('failed to ') ||
      lower == 'request failed' ||
      lower == 'empty response body';
}

String? _extractMessage(DioException error) {
  if (_isGatewayOrServerError(error.response?.statusCode)) {
    return null;
  }

  final fromResponse = extractApiMessage(error.response?.data);
  if (fromResponse != null &&
      fromResponse.trim().isNotEmpty &&
      isUserFacingApiMessage(fromResponse)) {
    return fromResponse;
  }

  if (error.error is String && (error.error as String).isNotEmpty) {
    final text = (error.error as String).trim();
    if (isUserFacingApiMessage(text) && !_isGenericFallbackMessage(text)) {
      return text;
    }
  }

  return null;
}

String? extractApiMessage(dynamic data) {
  if (data == null) return null;

  if (data is String) {
    final trimmed = data.trim();
    if (trimmed.isEmpty) return null;
    if (isHtmlContent(trimmed)) return null;
    if (trimmed.startsWith('{') || trimmed.startsWith('[')) {
      try {
        return extractApiMessage(jsonDecode(trimmed));
      } catch (_) {
        return isUserFacingApiMessage(trimmed) ? trimmed : null;
      }
    }
    return isUserFacingApiMessage(trimmed) ? trimmed : null;
  }

  if (data is! Map) return null;

  final map = data is Map<String, dynamic>
      ? data
      : Map<String, dynamic>.from(data);

  final fromErrors = _messageFromErrors(map['errors'] ?? map['Errors']);
  if (fromErrors != null) return fromErrors;

  for (final key in [
    'message',
    'Message',
    'errorMessage',
    'ErrorMessage',
    'detail',
    'Detail',
  ]) {
    final message = _messageFromValue(map[key], allowCommonCode: true);
    if (message != null) return message;
  }

  final title = _messageFromValue(
    map['title'] ?? map['Title'],
    allowCommonCode: true,
  );
  if (title != null) return title;

  final errorField = map['error'] ?? map['Error'];
  if (errorField is Map) {
    final nested = extractApiMessage(errorField);
    if (nested != null) return nested;
  } else {
    final errorMessage = _messageFromValue(errorField, allowCommonCode: true);
    if (errorMessage != null) return errorMessage;
  }

  final nestedData = map['data'] ?? map['Data'];
  if (nestedData is Map) {
    final nested = extractApiMessage(nestedData);
    if (nested != null) return nested;
  }

  return null;
}

String? _messageFromErrors(dynamic errors) {
  if (errors == null) return null;

  if (errors is String) {
    return _messageFromValue(errors, allowCommonCode: true);
  }

  if (errors is List && errors.isNotEmpty) {
    return _messageFromValue(errors.first, allowCommonCode: true);
  }

  if (errors is! Map) return null;

  final directMessage = errors['message'] ?? errors['Message'];
  final direct = _messageFromValue(directMessage, allowCommonCode: true);
  if (direct != null) return direct;

  for (final key in ['0', r'$', 'general', 'General', 'error', 'Error']) {
    if (!errors.containsKey(key) || errors[key] == null) continue;
    final value = _messageFromValue(errors[key], allowCommonCode: true);
    if (value != null) return value;
  }

  for (final value in errors.values) {
    final message = _messageFromValue(value, allowCommonCode: true);
    if (message != null) return message;
  }

  return null;
}

String? _messageFromValue(dynamic value, {required bool allowCommonCode}) {
  if (value == null) return null;

  if (value is List && value.isNotEmpty) {
    return _messageFromValue(value.first, allowCommonCode: allowCommonCode);
  }

  final text = value.toString().trim();
  if (text.isEmpty) return null;
  if (text.startsWith('One or more') || text.startsWith('حدث خطأ أو أكثر')) {
    return null;
  }
  if (!allowCommonCode && text.startsWith('common.')) return null;
  if (!isUserFacingApiMessage(text)) return null;
  return text;
}
