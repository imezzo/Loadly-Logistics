import 'package:flutter_test/flutter_test.dart';
import 'package:loadly_driver/core/utils/notification_click_action.dart';

void main() {
  group('NotificationClickAction', () {
    group('tripIdFromData', () {
      test('returns resourceId when clickAction is open_trip', () {
        final tripId = NotificationClickAction.tripIdFromData(
          {
            'clickAction': 'open_trip',
            'resourceId': '42',
          },
          source: 'test',
        );

        expect(tripId, '42');
      });

      test('returns resourceId when resourceType is TRIP', () {
        final tripId = NotificationClickAction.tripIdFromData(
          {
            'resourceType': 'TRIP',
            'resourceId': '42',
          },
          source: 'test',
        );

        expect(tripId, '42');
      });

      test('supports snake_case keys', () {
        final tripId = NotificationClickAction.tripIdFromData(
          {
            'click_action': 'open_trip',
            'resource_id': '42',
            'resource_type': 'TRIP',
          },
          source: 'test',
        );

        expect(tripId, '42');
      });

      test('falls back to tripId when resourceId is missing', () {
        final tripId = NotificationClickAction.tripIdFromData(
          {
            'clickAction': 'open_trip',
            'tripId': '99',
          },
          source: 'test',
        );

        expect(tripId, '99');
      });

      test('returns null for unsupported click actions', () {
        final tripId = NotificationClickAction.tripIdFromData(
          {'clickAction': 'open_profile'},
          source: 'test',
        );

        expect(tripId, isNull);
      });

      test('returns null when open_trip has no trip identifier', () {
        final tripId = NotificationClickAction.tripIdFromData(
          {'clickAction': 'open_trip'},
          source: 'test',
        );

        expect(tripId, isNull);
      });
    });

    group('tripRouteFromData', () {
      test('builds a trip details route from FCM data', () {
        final route = NotificationClickAction.tripRouteFromData(
          {
            'clickAction': 'open_trip',
            'resourceId': '7',
          },
          source: 'test',
        );

        expect(route, '/trips/7');
      });
    });

    group('localNotificationPayloadFromData', () {
      test('returns trip payload when click action resolves a trip id', () {
        final payload = NotificationClickAction.localNotificationPayloadFromData(
          {
            'clickAction': 'open_trip',
            'resourceId': '15',
          },
          source: 'test',
        );

        expect(payload, 'trip:15');
      });

      test('returns null when no trip id can be resolved', () {
        final payload = NotificationClickAction.localNotificationPayloadFromData(
          {'clickAction': 'open_trip'},
          source: 'test',
        );

        expect(payload, isNull);
      });
    });

    group('tripRouteFromNotification', () {
      test('builds route for in-app notification taps', () {
        final route = NotificationClickAction.tripRouteFromNotification(
          clickAction: 'open_trip',
          resourceId: 123,
          source: 'test',
        );

        expect(route, '/trips/123');
      });

      test('builds route when only resourceType is TRIP', () {
        final route = NotificationClickAction.tripRouteFromNotification(
          resourceType: 'TRIP',
          resourceId: 123,
          source: 'test',
        );

        expect(route, '/trips/123');
      });

      test('returns null when click action or resource id is missing', () {
        expect(
          NotificationClickAction.tripRouteFromNotification(
            clickAction: 'open_trip',
            resourceId: null,
            source: 'test',
          ),
          isNull,
        );
        expect(
          NotificationClickAction.tripRouteFromNotification(
            clickAction: 'generic',
            resourceId: 123,
            source: 'test',
          ),
          isNull,
        );
      });
    });
  });
}
