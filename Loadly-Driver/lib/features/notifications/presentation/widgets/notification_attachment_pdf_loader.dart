import 'dart:typed_data';

import 'package:dio/dio.dart';

/// In-memory session cache for notification PDF bytes (avoids repeat downloads).
abstract final class NotificationAttachmentPdfLoader {
  static final Map<String, Uint8List> _cache = {};

  static Future<Uint8List> loadBytes(String url, {Dio? dio}) async {
    final key = url.trim();
    final cached = _cache[key];
    if (cached != null) return cached;

    final client = dio ?? Dio();
    final response = await client.get<List<int>>(
      key,
      options: Options(responseType: ResponseType.bytes, followRedirects: true),
    );
    final data = response.data;
    if (data == null || data.isEmpty) {
      throw StateError('Empty PDF response');
    }
    final bytes = Uint8List.fromList(data);
    _cache[key] = bytes;
    return bytes;
  }

  static void clearCacheForTesting() => _cache.clear();
}
