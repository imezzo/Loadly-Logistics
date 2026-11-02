import '../router/route_constants.dart';
import 'notification_intent.dart';

/// Maps a parsed intent onto the app's existing GoRouter locations.
class NotificationRoutes {
  const NotificationRoutes._();

  static NotificationDestination destinationFor(NotificationIntent intent) {
    return switch (intent) {
      OpenTripNotificationIntent(:final tripId) => NotificationDestination(
        AppRoutePaths.tripById(tripId),
        extra: AppRoutePaths.home,
      ),
    };
  }
}
