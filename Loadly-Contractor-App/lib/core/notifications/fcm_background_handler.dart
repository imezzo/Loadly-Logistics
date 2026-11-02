import 'package:firebase_messaging/firebase_messaging.dart';

import '../services/fcm_service.dart';

/// Entry point for FCM messages delivered while the app is backgrounded or
/// terminated. This must stay a top-level function.
///
/// Navigation is intentionally not performed here. This isolate cannot safely
/// use GoRouter; the tap is handled later on the main isolate.
@pragma('vm:entry-point')
Future<void> firebaseMessagingBackgroundHandler(RemoteMessage message) {
  return FCMService.handleBackgroundMessage(message);
}
