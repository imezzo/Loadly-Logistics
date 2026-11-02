import 'dart:convert';

import 'notification_intent.dart';

/// Safely turns FCM data and local-notification payloads into intents.
class NotificationPayloadParser {
  const NotificationPayloadParser._();

  static const _tripResourceType = 'TRIP';

  static NotificationIntent? parse(Map<String, dynamic>? data) {
    if (data == null || data.isEmpty) return null;

    try {
      final clickAction = _read(
        data,
        'clickAction',
        'click_action',
      )?.toLowerCase();
      final resourceType = _read(
        data,
        'resourceType',
        'resource_type',
      )?.toUpperCase();

      // A missing click action with resourceType TRIP is the in-app payload
      // shape. An explicit action other than open_trip is left for a future
      // case instead of being treated as a trip.
      final action =
          clickAction ??
          (resourceType == _tripResourceType
              ? NotificationClickActions.openTrip
              : null);

      switch (action) {
        case NotificationClickActions.openTrip:
          final tripId = _tripId(data);
          if (tripId == null) return null;
          return OpenTripNotificationIntent(tripId);
        default:
          return null;
      }
    } catch (_) {
      return null;
    }
  }

  /// Local notifications store the original data map so a later tap can be
  /// routed without a second copy of the click-action rules.
  static String encodeLocalPayload(
    Map<String, dynamic> data, {
    String? messageId,
  }) {
    final normalized = <String, String>{};
    for (final entry in data.entries) {
      final value = entry.value;
      if (value == null) continue;
      final text = value.toString().trim();
      if (text.isEmpty) continue;
      normalized[entry.key.toString()] = text;
    }
    final id = messageId?.trim();
    if (id != null && id.isNotEmpty) {
      normalized['messageId'] = id;
    }
    return jsonEncode(normalized);
  }

  static NotificationIntent? intentFromLocalPayload(String? payload) {
    if (payload == null) return null;
    final trimmed = payload.trim();
    if (trimmed.isEmpty) return null;

    if (trimmed.startsWith('trip:')) {
      final tripId = int.tryParse(trimmed.substring(5).trim());
      if (tripId == null || tripId <= 0) return null;
      return OpenTripNotificationIntent(tripId);
    }

    try {
      final decoded = jsonDecode(trimmed);
      if (decoded is! Map) return null;
      return parse(Map<String, dynamic>.from(decoded));
    } catch (_) {
      return null;
    }
  }

  static String? messageIdFromLocalPayload(String? payload) {
    if (payload == null || payload.trim().isEmpty) return null;
    try {
      final decoded = jsonDecode(payload);
      if (decoded is! Map) return null;
      final messageId = decoded['messageId']?.toString().trim();
      if (messageId == null || messageId.isEmpty) return null;
      return messageId;
    } catch (_) {
      return null;
    }
  }

  static int? _tripId(Map<String, dynamic> data) {
    return _positiveInt(data, 'resourceId', 'resource_id') ??
        _positiveInt(data, 'tripId', 'trip_id');
  }

  static int? _positiveInt(
    Map<String, dynamic> data,
    String camel,
    String snake,
  ) {
    final text = _read(data, camel, snake);
    if (text == null) return null;
    final id = int.tryParse(text);
    if (id == null || id <= 0) return null;
    return id;
  }

  static String? _read(Map<String, dynamic> data, String camel, String snake) {
    final value = data.containsKey(camel) ? data[camel] : data[snake];
    if (value == null) return null;
    final text = value.toString().trim();
    return text.isEmpty ? null : text;
  }
}
