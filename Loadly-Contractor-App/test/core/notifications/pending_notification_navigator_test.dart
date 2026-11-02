import 'package:flutter_test/flutter_test.dart';

import 'package:contractor_app/core/notifications/notification_intent.dart';
import 'package:contractor_app/core/notifications/pending_notification_navigator.dart';

void main() {
  NotificationDestination destinationFor(NotificationIntent intent) {
    final tripId = (intent as OpenTripNotificationIntent).tripId;
    return NotificationDestination('/trips/$tripId');
  }

  test('defers navigation until the router and session are ready', () {
    final visits = <String>[];
    final navigator = PendingNotificationNavigator(
      destinationFor: destinationFor,
      navigate: (destination) {
        visits.add(destination.location);
        return true;
      },
      schedule: (action) => action(),
    );

    navigator.enqueue(const OpenTripNotificationIntent(12345), dedupeKey: 'm1');
    expect(visits, isEmpty);

    navigator.markNavigationReady();
    expect(visits, isEmpty);

    navigator.setAuthenticated(false);
    expect(visits, isEmpty);

    navigator.setAuthenticated(true);
    expect(visits, ['/trips/12345']);
  });

  test('ignores a repeated tap for the same message', () {
    final visits = <String>[];
    final navigator = PendingNotificationNavigator(
      destinationFor: destinationFor,
      navigate: (destination) {
        visits.add(destination.location);
        return true;
      },
      schedule: (action) => action(),
    );

    navigator.markNavigationReady();
    navigator.setAuthenticated(true);
    navigator.enqueue(const OpenTripNotificationIntent(5), dedupeKey: 'm1');
    navigator.enqueue(const OpenTripNotificationIntent(5), dedupeKey: 'm1');

    expect(visits, ['/trips/5']);
  });

  test('keeps the intent when navigation cannot run yet', () {
    var allowNavigation = false;
    final navigator = PendingNotificationNavigator(
      destinationFor: destinationFor,
      navigate: (destination) => allowNavigation,
      schedule: (action) => action(),
    );

    navigator.markNavigationReady();
    navigator.setAuthenticated(true);
    navigator.enqueue(const OpenTripNotificationIntent(9), dedupeKey: 'm2');
    expect(navigator.hasPendingIntent, isTrue);

    allowNavigation = true;
    navigator.markNavigationReady();
    expect(navigator.hasPendingIntent, isFalse);
  });

  test('drops a pending tap when the session ends', () {
    final visits = <String>[];
    final navigator = PendingNotificationNavigator(
      destinationFor: destinationFor,
      navigate: (destination) {
        visits.add(destination.location);
        return true;
      },
      schedule: (action) => action(),
    );

    navigator.setAuthenticated(true);
    navigator.enqueue(const OpenTripNotificationIntent(3), dedupeKey: 'm3');
    navigator.setAuthenticated(false);
    navigator.markNavigationReady();

    expect(visits, isEmpty);
    expect(navigator.hasPendingIntent, isFalse);
  });
}
