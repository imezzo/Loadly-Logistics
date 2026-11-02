/// Click-action names carried in FCM data and in-app notification records.
abstract final class NotificationClickActions {
  static const openTrip = 'open_trip';
}

/// A navigation target parsed from an FCM or local-notification payload.
///
/// Add a subtype here, a parser case, and a `NotificationRoutes` case to
/// support a new destination such as chat or orders.
sealed class NotificationIntent {
  const NotificationIntent();
}

final class OpenTripNotificationIntent extends NotificationIntent {
  const OpenTripNotificationIntent(this.tripId);

  final int tripId;
}

/// Where an intent should send the existing GoRouter.
class NotificationDestination {
  const NotificationDestination(this.location, {this.extra});

  final String location;
  final Object? extra;
}
