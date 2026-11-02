import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:loadly_driver/core/models/trip_schedule_update.dart';
import 'package:loadly_driver/core/services/local_notification_service.dart';
import 'package:loadly_driver/core/services/notification_deep_link_store.dart';
import 'package:loadly_driver/core/services/notification_launch_handler.dart';

const _tripScheduleBody =
    'Title: Trip Schedule Updated\n\nBody: Your trip schedule has been updated: 14 May 2026 21:00 to 20 May 2026 21:00.';

RemoteMessage _message({
  Map<String, dynamic> data = const {},
  String? body,
}) {
  return RemoteMessage(
    data: data,
    notification: body == null ? null : RemoteNotification(body: body),
  );
}

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();
  final service = LocalNotificationService.instance;

  setUp(() async {
    SharedPreferences.setMockInitialValues({});
    await NotificationDeepLinkStore.init();
  });

  tearDown(() {
    service.clearPendingUpdate();
    NotificationLaunchHandler.resetForTesting();
  });

  group('LocalNotificationService.addTripScheduleUpdateFromMessage', () {
    test('stores and emits trip schedule updates from notification body', () async {
      final expectation = expectLater(
        service.scheduleUpdateStream,
        emits(
          isA<TripScheduleUpdate>().having(
            (update) => update.displayText,
            'displayText',
            '14 May 2026 21:00 → 20 May 2026 21:00',
          ),
        ),
      );

      service.addTripScheduleUpdateFromMessage(_message(body: _tripScheduleBody));

      expect(service.pendingUpdate, isNotNull);
      expect(
        service.pendingUpdate!.displayText,
        '14 May 2026 21:00 → 20 May 2026 21:00',
      );

      await expectation;
    });

    test('ignores messages that are not trip schedule updates', () {
      service.addTripScheduleUpdateFromMessage(
        _message(body: 'Your trip starts soon'),
      );

      expect(service.pendingUpdate, isNull);
    });
  });

  group('LocalNotificationService.handlePayloadForTesting', () {
    test('invokes schedule update tap callback on warm start', () {
      var tapped = false;
      service.setOnScheduleUpdateTap(() => tapped = true);

      service.handlePayloadForTesting(_tripScheduleBody);

      expect(tapped, isTrue);
      expect(service.pendingUpdate, isNotNull);
    });

    test('defers navigation to launch handler on cold start', () {
      service.handlePayloadForTesting('trip:21', fromColdStart: true);

      expect(NotificationLaunchHandler.pendingRoute, '/trips/21');
    });

    test('invokes generic notification tap callback for generic payloads', () {
      var tapped = false;
      service.setOnGenericNotificationTap(() => tapped = true);

      service.handlePayloadForTesting('generic');

      expect(tapped, isTrue);
    });

    test('ignores empty payloads', () {
      var tapped = false;
      service.setOnGenericNotificationTap(() => tapped = true);

      service.handlePayloadForTesting(null);
      service.handlePayloadForTesting('');

      expect(tapped, isFalse);
      expect(service.pendingUpdate, isNull);
    });
  });

  group('LocalNotificationService.clearPendingUpdate', () {
    test('clears stored pending update', () {
      service.addTripScheduleUpdateFromMessage(_message(body: _tripScheduleBody));
      expect(service.pendingUpdate, isNotNull);

      service.clearPendingUpdate();

      expect(service.pendingUpdate, isNull);
    });
  });
}
