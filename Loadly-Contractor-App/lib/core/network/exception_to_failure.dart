import 'package:dio/dio.dart';

import '../errors/failure.dart';
import '../errors/api_fallback_keys.dart';
import 'api_failure_mapper.dart';

Failure mapExceptionToFailure(
  Object error, {
  String fallback = ApiFallbackKeys.requestFailed,
}) {
  if (error is DioException) {
    final message = userFacingDioMessage(error);
    if (message != null &&
        message.trim().isNotEmpty &&
        !_isGenericFallbackMessage(message)) {
      return Failure(message);
    }
    return Failure(fallback);
  }

  if (error is Exception) {
    final text = error.toString();
    const prefix = 'Exception: ';
    final message = text.startsWith(prefix) ? text.substring(prefix.length) : text;
    if (message.trim().isNotEmpty && isUserFacingApiMessage(message)) {
      return Failure(message);
    }
  }

  return Failure(fallback);
}

bool _isGenericFallbackMessage(String message) {
  final lower = message.trim().toLowerCase();
  return lower.startsWith('failed to ') ||
      lower == 'request failed' ||
      lower == 'empty response body';
}
