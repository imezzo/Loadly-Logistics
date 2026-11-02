import 'package:dio/dio.dart';

import '../errors/user_message_codes.dart';
import 'api_failure_mapper.dart';

Map<String, dynamic> requireEnvelope(Response<Map<String, dynamic>> response) {
  final data = response.data;
  if (data == null) {
    throw DioException(
      requestOptions: response.requestOptions,
      response: response,
      type: DioExceptionType.badResponse,
      error: UserMessageCodes.emptyResponse,
    );
  }
  return data;
}

void throwIfEnvelopeFailed(
  Response<Map<String, dynamic>> response, {
  String fallbackMessage = UserMessageCodes.requestFailed,
}) {
  final envelope = requireEnvelope(response);
  final success = envelope['success'] as bool? ?? false;
  if (success) return;

  final message = extractApiMessage(envelope) ?? fallbackMessage;
  throw DioException(
    requestOptions: response.requestOptions,
    response: response,
    type: DioExceptionType.badResponse,
    error: message,
  );
}

Map<String, dynamic> requireEnvelopeDataMap(
  Response<Map<String, dynamic>> response, {
  String missingDataMessage = UserMessageCodes.missingData,
}) {
  final envelope = requireEnvelope(response);
  final success = envelope['success'] as bool? ?? false;
  if (!success) {
    final message =
        extractApiMessage(envelope) ?? UserMessageCodes.requestFailed;
    throw DioException(
      requestOptions: response.requestOptions,
      response: response,
      type: DioExceptionType.badResponse,
      error: message,
    );
  }

  final data = envelope['data'];
  if (data is Map<String, dynamic>) return data;

  throw DioException(
    requestOptions: response.requestOptions,
    response: response,
    type: DioExceptionType.badResponse,
    error: missingDataMessage,
  );
}
