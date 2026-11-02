import 'package:flutter_test/flutter_test.dart';

import 'package:contractor_app/core/notifications/notification_intent.dart';
import 'package:contractor_app/core/notifications/notification_payload_parser.dart';
import 'package:contractor_app/core/notifications/notification_routes.dart';
import 'package:contractor_app/core/router/route_constants.dart';

void main() {
  group('NotificationPayloadParser', () {
    test('parses open_trip with a string resourceId', () {
      final intent = NotificationPayloadParser.parse(const {
        'clickAction': 'open_trip',
        'resourceId': '12345',
      });

      expect(intent, isA<OpenTripNotificationIntent>());
      expect((intent! as OpenTripNotificationIntent).tripId, 12345);
    });

    test('accepts snake_case keys and mixed-case click actions', () {
      final intent = NotificationPayloadParser.parse(const {
        'click_action': 'Open_Trip',
        'resource_id': 88,
      });

      expect((intent! as OpenTripNotificationIntent).tripId, 88);
    });

    test('accepts a legacy trip payload without a click action', () {
      final intent = NotificationPayloadParser.parse(const {
        'resourceType': 'trip',
        'tripId': '15',
      });

      expect((intent! as OpenTripNotificationIntent).tripId, 15);
    });

    test('ignores a missing, blank, zero, or non-numeric trip id', () {
      expect(
        NotificationPayloadParser.parse(const {'clickAction': 'open_trip'}),
        isNull,
      );
      expect(
        NotificationPayloadParser.parse(const {
          'clickAction': 'open_trip',
          'resourceId': '  ',
        }),
        isNull,
      );
      expect(
        NotificationPayloadParser.parse(const {
          'clickAction': 'open_trip',
          'resourceId': '0',
        }),
        isNull,
      );
      expect(
        NotificationPayloadParser.parse(const {
          'clickAction': 'open_trip',
          'resourceId': 'abc',
        }),
        isNull,
      );
      expect(
        NotificationPayloadParser.parse(const {
          'clickAction': 'open_trip',
          'resourceId': '-4',
        }),
        isNull,
      );
    });

    test('does not treat a different click action as a trip', () {
      expect(
        NotificationPayloadParser.parse(const {
          'clickAction': 'open_chat',
          'resourceType': 'TRIP',
          'resourceId': '9',
        }),
        isNull,
      );
    });

    test('round-trips a local notification payload', () {
      const data = {'clickAction': 'open_trip', 'resourceId': '42'};
      final payload = NotificationPayloadParser.encodeLocalPayload(
        data,
        messageId: 'msg-1',
      );

      final intent = NotificationPayloadParser.intentFromLocalPayload(payload);
      expect((intent! as OpenTripNotificationIntent).tripId, 42);
      expect(
        NotificationPayloadParser.messageIdFromLocalPayload(payload),
        'msg-1',
      );
    });

    test('rejects malformed local payloads', () {
      expect(NotificationPayloadParser.intentFromLocalPayload('{'), isNull);
      expect(
        NotificationPayloadParser.intentFromLocalPayload('trip:0'),
        isNull,
      );
      expect(
        NotificationPayloadParser.intentFromLocalPayload('trip:7'),
        isA<OpenTripNotificationIntent>(),
      );
      expect(NotificationPayloadParser.intentFromLocalPayload(null), isNull);
    });
  });

  group('NotificationRoutes', () {
    test('opens trip details and returns home afterwards', () {
      final destination = NotificationRoutes.destinationFor(
        const OpenTripNotificationIntent(12345),
      );

      expect(destination.location, AppRoutePaths.tripById(12345));
      expect(destination.extra, AppRoutePaths.home);
    });
  });
}
