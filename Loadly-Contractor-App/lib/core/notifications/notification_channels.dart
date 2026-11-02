/// Android notification channel used by both FCM and local notifications.
///
/// The id must match `com.google.firebase.messaging.default_notification_channel_id`
/// in `AndroidManifest.xml`. Channel importance is fixed the first time Android
/// creates the channel, so it is created at startup before any message arrives.
abstract final class NotificationChannels {
  static const highImportanceId = 'high_importance_channel';
  static const highImportanceName = 'Trip updates';
  static const highImportanceDescription =
      'Notifications about trips, schedules, and important updates.';
}
