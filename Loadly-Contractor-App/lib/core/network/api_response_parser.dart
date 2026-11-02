import 'package:dio/dio.dart';

import 'api_failure_mapper.dart';
import '../errors/api_fallback_keys.dart';

Map<String, dynamic> requireEnvelope(Response<Map<String, dynamic>> response) {
  final data = response.data;
  if (data == null) {
    throw DioException(
      requestOptions: response.requestOptions,
      response: response,
      type: DioExceptionType.badResponse,
      error: 'Empty response body',
    );
  }
  return data;
}

int? _parseCountValue(dynamic value) {
  if (value is num) return value.toInt();
  if (value is String) return int.tryParse(value.trim());
  return null;
}

int? _totalCountFromMap(Map map) {
  for (final key in [
    'totalCount',
    'TotalCount',
    'total',
    'Total',
    'count',
    'Count',
    'recordsTotal',
  ]) {
    final parsed = _parseCountValue(map[key]);
    if (parsed != null) return parsed;
  }

  for (final paginationKey in ['pagination', 'Pagination']) {
    final pagination = map[paginationKey];
    if (pagination is Map) {
      for (final key in ['totalCount', 'TotalCount', 'total', 'Total']) {
        final parsed = _parseCountValue(pagination[key]);
        if (parsed != null) return parsed;
      }
    }
  }

  for (final metaKey in ['meta', 'Meta']) {
    final meta = map[metaKey];
    if (meta is Map) {
      for (final key in ['totalCount', 'TotalCount', 'total', 'Total']) {
        final parsed = _parseCountValue(meta[key]);
        if (parsed != null) return parsed;
      }
    }
  }

  return null;
}

int totalCountFromEnvelope(Map<String, dynamic> envelope, int fallback) {
  final envelopeCount = _totalCountFromMap(envelope);
  if (envelopeCount != null) return envelopeCount;

  final data = envelope['data'] ?? envelope['Data'];
  if (data is Map) {
    final dataCount = _totalCountFromMap(Map<String, dynamic>.from(data));
    if (dataCount != null) return dataCount;
  }

  return fallback;
}

void throwIfEnvelopeFailed(
  Response<Map<String, dynamic>> response, {
  String fallbackMessage = ApiFallbackKeys.requestFailed,
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

/// Throws only when the API explicitly returns `success: false`.
/// Use for list/read endpoints that may omit the success field on success.
void throwIfEnvelopeSuccessFalse(
  Response<Map<String, dynamic>> response, {
  String fallbackMessage = ApiFallbackKeys.requestFailed,
}) {
  final envelope = requireEnvelope(response);
  final success = envelope['success'] ?? envelope['Success'];
  if (success != false) return;

  final message = extractApiMessage(envelope) ?? fallbackMessage;
  throw DioException(
    requestOptions: response.requestOptions,
    response: response,
    type: DioExceptionType.badResponse,
    error: message,
  );
}

List<dynamic> requireEnvelopeList(
  Response<Map<String, dynamic>> response, {
  String fallbackMessage = ApiFallbackKeys.requestFailed,
  String invalidListMessage = ApiFallbackKeys.invalidResponse,
}) {
  throwIfEnvelopeSuccessFalse(response, fallbackMessage: fallbackMessage);
  final envelope = requireEnvelope(response);
  var data = envelope['data'] ?? envelope['Data'];

  if (data is Map) {
    data = data['items'] ?? data['Items'] ?? data['data'] ?? data['Data'];
  }

  if (data is! List) {
    throw DioException(
      requestOptions: response.requestOptions,
      response: response,
      type: DioExceptionType.badResponse,
      error: invalidListMessage,
    );
  }

  return data;
}

Map<String, dynamic> requireEnvelopeDataMap(
  Response<Map<String, dynamic>> response, {
  String missingDataMessage = ApiFallbackKeys.invalidResponse,
}) {
  final envelope = requireEnvelope(response);
  final success = envelope['success'] as bool? ?? false;
  if (!success) {
    final message = extractApiMessage(envelope) ?? ApiFallbackKeys.requestFailed;
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
