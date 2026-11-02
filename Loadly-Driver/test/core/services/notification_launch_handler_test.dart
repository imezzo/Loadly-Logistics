import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:go_router/go_router.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:loadly_driver/core/router/app_router.dart';
import 'package:loadly_driver/core/services/local_notification_service.dart';
import 'package:loadly_driver/core/services/notification_deep_link_store.dart';
import 'package:loadly_driver/core/services/notification_launch_handler.dart';

const _tripScheduleBody =
    'Title: Trip Schedule Updated\n\nBody: Your trip schedule has been updated: 14 May 2026 21:00 to 20 May 2026 21:00.';

class _RecordingRouter extends Fake implements GoRouter {
  String? lastLocation;

  @override
  void go(String location, {Object? extra}) {
    lastLocation = location;
  }
}

RemoteMessage _message({
  Map<String, dynamic> data = const {},
  String? body,
  String? messageId,
}) {
  return RemoteMessage(
    messageId: messageId,
    data: data,
    notification: body == null ? null : RemoteNotification(body: body),
  );
}

void main() {
  TestWidgetsFlutterBinding.ensureInitialized();

  setUp(() async {
    SharedPreferences.setMockInitialValues({});
    await NotificationDeepLinkStore.init();
    NotificationLaunchHandler.resetForTesting();
    LocalNotificationService.instance.clearPendingUpdate();
    AppRouterHolder.instance.clearRouter();
  });

  tearDown(() {
    NotificationLaunchHandler.resetForTesting();
    LocalNotificationService.instance.clearPendingUpdate();
    AppRouterHolder.instance.clearRouter();
  });

  group('NotificationLaunchHandler.setPendingFromPayload', () {
    test('stores trip route for trip payloads', () {
      NotificationLaunchHandler.setPendingFromPayload('trip:42');

      expect(NotificationLaunchHandler.pendingRoute, '/trips/42');
    });

    test('stores home route for trip schedule update payloads', () {
      NotificationLaunchHandler.setPendingFromPayload(_tripScheduleBody);

      expect(NotificationLaunchHandler.pendingRoute, '/home');
    });

    test('stores home route for generic payloads', () {
      NotificationLaunchHandler.setPendingFromPayload('generic');

      expect(NotificationLaunchHandler.pendingRoute, '/home');
    });

    test('ignores trip payloads with an empty trip id', () {
      NotificationLaunchHandler.setPendingFromPayload('trip:');

      expect(NotificationLaunchHandler.pendingRoute, isNull);
    });
  });

  group('NotificationLaunchHandler.setPendingFromMessage', () {
    test('stores trip route when click action opens a trip', () {
      NotificationLaunchHandler.setPendingFromMessage(
        _message(data: {'clickAction': 'open_trip', 'resourceId': '55'}),
      );

      expect(NotificationLaunchHandler.pendingRoute, '/trips/55');
    });

    test('stores trip route when resourceType is TRIP', () {
      NotificationLaunchHandler.setPendingFromMessage(
        _message(data: {'resourceType': 'TRIP', 'resourceId': '55'}),
      );

      expect(NotificationLaunchHandler.pendingRoute, '/trips/55');
    });

    test('restores trip route from deep link store when tap data is empty', () {
      NotificationLaunchHandler.cacheFromMessage(
        _message(
          messageId: 'msg-1',
          data: {'clickAction': 'open_trip', 'resourceId': '77'},
        ),
        source: 'test',
      );

      NotificationLaunchHandler.setPendingFromMessage(
        _message(messageId: 'msg-1'),
      );

      expect(NotificationLaunchHandler.pendingRoute, '/trips/77');
    });

    test('falls back to home when no trip click action is present', () {
      NotificationLaunchHandler.setPendingFromMessage(
        _message(data: {'clickAction': 'generic'}),
      );

      expect(NotificationLaunchHandler.pendingRoute, '/home');
    });
  });

  group('NotificationLaunchHandler.navigateIfPending', () {
    testWidgets('does not navigate when the user is logged out', (
      tester,
    ) async {
      NotificationLaunchHandler.setPendingFromPayload('trip:10');

      NotificationLaunchHandler.navigateIfPending(isLoggedIn: false);
      await tester.pump();

      expect(NotificationLaunchHandler.pendingRoute, '/trips/10');
    });

    testWidgets('does not navigate when the router is not ready', (
      tester,
    ) async {
      NotificationLaunchHandler.setPendingFromPayload('trip:10');

      NotificationLaunchHandler.navigateIfPending(isLoggedIn: true);
      await tester.pump();

      expect(NotificationLaunchHandler.pendingRoute, '/trips/10');
    });

    test(
      'navigates and clears pending route when logged in and router is ready',
      () async {
        final router = _RecordingRouter();
        AppRouterHolder.instance.setRouter(router);
        NotificationLaunchHandler.setPendingFromPayload('trip:88');

        NotificationLaunchHandler.navigateIfPending(isLoggedIn: true);
        await Future<void>.delayed(Duration.zero);

        expect(router.lastLocation, '/trips/88');
        expect(NotificationLaunchHandler.pendingRoute, isNull);
      },
    );

    test(
      'applies trip schedule side effects from pending FCM message',
      () async {
        final router = _RecordingRouter();
        AppRouterHolder.instance.setRouter(router);
        NotificationLaunchHandler.setPendingFromMessage(
          _message(body: _tripScheduleBody),
        );

        NotificationLaunchHandler.navigateIfPending(isLoggedIn: true);
        await Future<void>.delayed(Duration.zero);

        expect(router.lastLocation, '/home');
        expect(LocalNotificationService.instance.pendingUpdate, isNotNull);
      },
    );
  });

  group('NotificationLaunchHandler.handleTap', () {
    test(
      'refreshes trips and navigates when the app is already running',
      () async {
        final router = _RecordingRouter();
        AppRouterHolder.instance.setRouter(router);
        var refreshCount = 0;

        NotificationLaunchHandler.handleTap(
          _message(data: {'clickAction': 'open_trip', 'resourceId': '3'}),
          onRefreshTrips: () => refreshCount++,
        );
        await Future<void>.delayed(Duration.zero);

        expect(refreshCount, 1);
        expect(router.lastLocation, '/trips/3');
        expect(NotificationLaunchHandler.pendingRoute, isNull);
      },
    );

    testWidgets('does not reload the trip when that trip is already open', (
      tester,
    ) async {
      final router = GoRouter(
        initialLocation: '/trips/3',
        routes: [
          GoRoute(
            path: '/trips/:tripId',
            builder: (context, state) => const SizedBox.shrink(),
          ),
          GoRoute(
            path: '/home',
            builder: (context, state) => const SizedBox.shrink(),
          ),
        ],
      );
      addTearDown(router.dispose);
      AppRouterHolder.instance.setRouter(router);
      await tester.pumpWidget(CupertinoApp.router(routerConfig: router));

      var refreshCount = 0;
      NotificationLaunchHandler.handleTap(
        _message(data: {'clickAction': 'open_trip', 'resourceId': '3'}),
        onRefreshTrips: () => refreshCount++,
      );
      await tester.pump();

      expect(refreshCount, 0);
      expect(router.state.uri.path, '/trips/3');
      expect(NotificationLaunchHandler.pendingRoute, isNull);
    });
  });
}
