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

String? _extractMessage(DioException error) {
  if (error.error is String && (error.error as String).isNotEmpty) {
    return error.error as String;
  }
  return extractApiMessage(error.response?.data);
}

String? extractApiMessage(dynamic data) {
  if (data is! Map) return null;

  final message = data['message'];
  if (message is String &&
      message.isNotEmpty &&
      !message.startsWith('One or more')) {
    return message;
  }

  final errors = data['errors'];
  if (errors is Map) {
    final directMessage = errors['message'];
    if (directMessage != null && directMessage.toString().isNotEmpty) {
      return directMessage.toString();
    }

    if (errors.containsKey('0') && errors['0'] != null) {
      return errors['0'].toString();
    }

    for (final value in errors.values) {
      if (value == null) continue;
      if (value is List && value.isNotEmpty) return value.first.toString();
      final text = value.toString();
      if (text.isNotEmpty) return text;
    }
  }

  if (errors is List && errors.isNotEmpty) {
    return errors.first.toString();
  }

  return null;
}
