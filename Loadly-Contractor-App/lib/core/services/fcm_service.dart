import 'dart:async';
import 'dart:io';

import 'package:firebase_core/firebase_core.dart';
import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/widgets.dart';
import 'package:shared_preferences/shared_preferences.dart';

import '../notifications/local_notification_service.dart';
import '../notifications/notification_intent.dart';
import '../notifications/notification_payload_parser.dart';
import '../notifications/notification_routes.dart';
import '../notifications/pending_notification_navigator.dart';
import '../router/app_router_holder.dart';
import '../services/app_update_notifier.dart';
import '../utils/fcm_notification_logger.dart';

/// Push notifications: permissions, token persistence, display, and deferred
/// navigation. In-app notification history stays in the notifications feature.
class FCMService {
  FCMService._internal();

  static final FCMService instance = FCMService._internal();

  factory FCMService() => instance;

  static const _tokenKey = 'fcm_token';
  static const _disclosureDecidedKey = 'fcm_notification_disclosure_decided';
  static const _disclosureAcceptedKey = 'fcm_notification_disclosure_accepted';

  static Future<void>? _firebaseInit;

  final LocalNotificationService _localNotifications =
      LocalNotificationService();
  late final PendingNotificationNavigator _navigator =
      PendingNotificationNavigator(
        destinationFor: NotificationRoutes.destinationFor,
        navigate: _navigate,
      );

  final Set<String> _displayedMessageIds = <String>{};

  String? _currentToken;
  Future<void>? _initializing;
  bool _initialized = false;
  bool _listenersAttached = false;

  /// Cached FCM token from this process or the last successful registration.
  String? get currentToken => _currentToken;

  /// Overrides the cached token. Does not contact Firebase.
  void setToken(String? token) {
    _currentToken = token;
  }

  bool get isSupportedPlatform {
    if (kIsWeb) return false;
    return Platform.isAndroid || Platform.isIOS;
  }

  /// Safe to call more than once. A failed attempt can be retried.
  Future<void> initialize() {
    if (_initialized) return Future<void>.value();
    final inFlight = _initializing;
    if (inFlight != null) return inFlight;

    final future = _initialize();
    _initializing = future;
    return future;
  }

  void markNavigationReady() => _navigator.markNavigationReady();

  void setAuthenticated(bool isAuthenticated) {
    _navigator.setAuthenticated(isAuthenticated);
  }

  /// Shows the system permission dialog after the caller has already obtained
  /// prominent-disclosure consent. A declined disclosure is remembered so the
  /// prompt is not shown on every launch.
  Future<void> resolvePermission({
    required bool Function() canPrompt,
    required Future<bool> Function() showDisclosure,
  }) async {
    if (!isSupportedPlatform || !_initialized) {
      FcmNotificationLogger.logStep(
        step: 'permission_skipped',
        source: 'permission',
        detail: 'supported=$isSupportedPlatform initialized=$_initialized',
      );
      return;
    }

    try {
      final prefs = await SharedPreferences.getInstance();
      if (prefs.getBool(_disclosureDecidedKey) == true) {
        if (prefs.getBool(_disclosureAcceptedKey) == true) {
          await requestSystemPermission();
        }
        return;
      }

      if (!canPrompt()) return;
      final accepted = await showDisclosure();
      if (!canPrompt()) return;

      await prefs.setBool(_disclosureDecidedKey, true);
      await prefs.setBool(_disclosureAcceptedKey, accepted);
      if (!accepted) {
        FcmNotificationLogger.logStep(
          step: 'permission_declined',
          source: 'permission',
          detail: 'disclosure dismissed',
        );
        return;
      }
      await requestSystemPermission();
    } catch (error, stackTrace) {
      FcmNotificationLogger.logError(
        step: 'permission_failed',
        error: error,
        stackTrace: stackTrace,
      );
    }
  }

  Future<bool> requestSystemPermission() async {
    try {
      final settings = await FirebaseMessaging.instance.requestPermission(
        alert: true,
        announcement: false,
        badge: true,
        carPlay: false,
        criticalAlert: false,
        provisional: false,
        sound: true,
      );
      final status = settings.authorizationStatus;
      final granted =
          status == AuthorizationStatus.authorized ||
          status == AuthorizationStatus.provisional;
      FcmNotificationLogger.logStep(
        step: 'permission_result',
        source: 'permission',
        detail: 'status=$status granted=$granted',
      );
      if (granted) {
        unawaited(_syncToken());
      }
      return granted;
    } catch (error, stackTrace) {
      FcmNotificationLogger.logError(
        step: 'permission_request_failed',
        error: error,
        stackTrace: stackTrace,
      );
      return false;
    }
  }

  /// Background isolate entry. Displays data-only messages and never navigates.
  static Future<void> handleBackgroundMessage(RemoteMessage message) async {
    try {
      WidgetsFlutterBinding.ensureInitialized();
      await ensureFirebaseInitialized();
      FcmNotificationLogger.logReceived(source: 'background', message: message);
      if (message.notification != null) {
        FcmNotificationLogger.logStep(
          step: 'background_skip_local_display',
          source: 'background',
          message: message,
          detail: 'the OS already displayed the notification payload',
        );
        return;
      }

      final localNotifications = LocalNotificationService();
      await localNotifications.initialize();
      await localNotifications.showRemoteMessage(message, source: 'background');
    } catch (error, stackTrace) {
      FcmNotificationLogger.logError(
        step: 'background_handler_failed',
        error: error,
        stackTrace: stackTrace,
      );
    }
  }

  static Future<void> ensureFirebaseInitialized() async {
    if (kIsWeb || (!Platform.isAndroid && !Platform.isIOS)) return;
    if (Firebase.apps.isNotEmpty) return;

    final inFlight = _firebaseInit;
    if (inFlight != null) {
      await inFlight;
      return;
    }

    // ponytail: no firebase_options until new Loadly Firebase project wired
    final future = Firebase.initializeApp();
    _firebaseInit = future;
    try {
      await future;
    } catch (error) {
      if (identical(_firebaseInit, future)) {
        _firebaseInit = null;
      }
      rethrow;
    }
  }

  Future<void> _initialize() async {
    try {
      if (!isSupportedPlatform) {
        FcmNotificationLogger.logStep(
          step: 'init_skipped',
          source: 'startup',
          detail: 'platform not supported',
        );
        _initialized = true;
        return;
      }

      await ensureFirebaseInitialized();
      await _localNotifications.initialize(onTap: _onLocalNotificationTap);

      // iOS presents notification payloads in the foreground unless this is
      // disabled. The local notification below is the single visible copy.
      await FirebaseMessaging.instance
          .setForegroundNotificationPresentationOptions(
            alert: false,
            badge: false,
            sound: false,
          );

      await _restoreToken();
      _attachListeners();
      await _captureInitialMessage();
      unawaited(_syncToken());
      _initialized = true;
      FcmNotificationLogger.logStep(step: 'init_completed', source: 'startup');
    } catch (error, stackTrace) {
      _initializing = null;
      FcmNotificationLogger.logError(
        step: 'init_failed',
        error: error,
        stackTrace: stackTrace,
      );
      rethrow;
    }
  }

  void _attachListeners() {
    if (_listenersAttached) return;
    _listenersAttached = true;

    FirebaseMessaging.onMessage.listen(
      _onForegroundMessage,
      onError: (Object error, StackTrace stackTrace) {
        FcmNotificationLogger.logError(
          step: 'on_message_failed',
          error: error,
          stackTrace: stackTrace,
        );
      },
    );

    FirebaseMessaging.onMessageOpenedApp.listen(
      (message) => _onNotificationOpened(message, source: 'openedApp'),
      onError: (Object error, StackTrace stackTrace) {
        FcmNotificationLogger.logError(
          step: 'on_message_opened_failed',
          error: error,
          stackTrace: stackTrace,
        );
      },
    );

    FirebaseMessaging.instance.onTokenRefresh.listen(
      (token) {
        unawaited(_persistToken(token, source: 'refresh'));
      },
      onError: (Object error, StackTrace stackTrace) {
        FcmNotificationLogger.logError(
          step: 'token_refresh_failed',
          error: error,
          stackTrace: stackTrace,
        );
      },
    );
  }

  Future<void> _captureInitialMessage() async {
    try {
      final message = await FirebaseMessaging.instance.getInitialMessage();
      if (message == null) return;
      _onNotificationOpened(message, source: 'initialMessage');
    } catch (error, stackTrace) {
      FcmNotificationLogger.logError(
        step: 'initial_message_failed',
        error: error,
        stackTrace: stackTrace,
      );
    }
  }

  void _onForegroundMessage(RemoteMessage message) {
    FcmNotificationLogger.logReceived(source: 'foreground', message: message);
    final messageId = message.messageId;
    if (messageId != null &&
        messageId.isNotEmpty &&
        !_displayedMessageIds.add(messageId)) {
      FcmNotificationLogger.logStep(
        step: 'foreground_duplicate_ignored',
        source: 'foreground',
        message: message,
      );
      return;
    }
    unawaited(
      _localNotifications.showRemoteMessage(message, source: 'foreground'),
    );
  }

  void _onNotificationOpened(RemoteMessage message, {required String source}) {
    FcmNotificationLogger.logClicked(source: source, message: message);
    _enqueueFromData(
      message.data,
      messageId: message.messageId,
      source: source,
    );
  }

  void _onLocalNotificationTap(String? payload) {
    final intent = NotificationPayloadParser.intentFromLocalPayload(payload);
    if (intent == null) {
      FcmNotificationLogger.logStep(
        step: 'payload_ignored',
        source: 'localTap',
        payload: payload,
        detail: 'no routable click action',
      );
      return;
    }
    _navigator.enqueue(
      intent,
      dedupeKey: NotificationPayloadParser.messageIdFromLocalPayload(payload),
    );
  }

  void _enqueueFromData(
    Map<String, dynamic> data, {
    required String? messageId,
    required String source,
  }) {
    final intent = NotificationPayloadParser.parse(data);
    if (intent == null) {
      FcmNotificationLogger.logStep(
        step: 'payload_ignored',
        source: source,
        data: data,
        detail: 'missing open_trip action or valid trip id',
      );
      return;
    }
    _navigator.enqueue(intent, dedupeKey: messageId);
  }

  Future<void> _restoreToken() async {
    try {
      final prefs = await SharedPreferences.getInstance();
      final stored = prefs.getString(_tokenKey);
      if (stored == null || stored.isEmpty) return;
      _currentToken = stored;
      FcmNotificationLogger.logStep(
        step: 'token_restored',
        source: 'token',
        detail: 'length=${stored.length}',
      );
    } catch (error, stackTrace) {
      FcmNotificationLogger.logError(
        step: 'token_restore_failed',
        error: error,
        stackTrace: stackTrace,
      );
    }
  }

  Future<void> _syncToken() async {
    try {
      if (Platform.isIOS) {
        final apnsToken = await FirebaseMessaging.instance.getAPNSToken();
        if (apnsToken == null) {
          FcmNotificationLogger.logStep(
            step: 'token_waiting_for_apns',
            source: 'token',
            detail: 'onTokenRefresh will persist the token',
          );
          return;
        }
      }

      final token = await FirebaseMessaging.instance.getToken().timeout(
        const Duration(seconds: 10),
      );
      if (token == null || token.isEmpty) {
        FcmNotificationLogger.logStep(step: 'token_empty', source: 'token');
        return;
      }
      await _persistToken(token, source: 'getToken');
    } catch (error, stackTrace) {
      FcmNotificationLogger.logError(
        step: 'token_sync_failed',
        error: error,
        stackTrace: stackTrace,
      );
    }
  }

  Future<void> _persistToken(String token, {required String source}) async {
    final trimmed = token.trim();
    if (trimmed.isEmpty) return;
    _currentToken = trimmed;
    try {
      final prefs = await SharedPreferences.getInstance();
      await prefs.setString(_tokenKey, trimmed);
      FcmNotificationLogger.logStep(
        step: 'token_persisted',
        source: source,
        detail: 'length=${trimmed.length}',
      );
      if (kDebugMode) {
        debugPrint('[FCM] debug token=$trimmed');
      }
    } catch (error, stackTrace) {
      FcmNotificationLogger.logError(
        step: 'token_persist_failed',
        error: error,
        stackTrace: stackTrace,
      );
    }
  }

  static bool _navigate(NotificationDestination destination) {
    if (AppUpdateNotifier.instance.pending?.isForced == true) {
      FcmNotificationLogger.logStep(
        step: 'navigation_deferred',
        source: 'navigator',
        route: destination.location,
        detail: 'forced app update is blocking the app',
      );
      return false;
    }

    final router = AppRouterHolder.instance.router;
    if (router == null) return false;
    router.go(destination.location, extra: destination.extra);
    return true;
  }
}
