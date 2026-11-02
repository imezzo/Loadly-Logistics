import 'package:equatable/equatable.dart';

import '../../../../core/models/trip_schedule_update.dart';
import 'home_location_issue.dart';

/// Events for Home BLoC
abstract class HomeEvent extends Equatable {
  const HomeEvent();

  @override
  List<Object?> get props => [];
}

/// Event to toggle online/offline status
class HomeToggleOnlineStatus extends HomeEvent {
  const HomeToggleOnlineStatus();
}

/// Event to get initial location (one-time)
class HomeGetInitialLocation extends HomeEvent {
  const HomeGetInitialLocation();
}

/// Event to start location tracking (continuous)
class HomeStartLocationTracking extends HomeEvent {
  const HomeStartLocationTracking();
}

/// Event to stop location tracking
class HomeStopLocationTracking extends HomeEvent {
  const HomeStopLocationTracking();
}

/// Event when location is updated
class HomeLocationUpdated extends HomeEvent {
  final double latitude;
  final double longitude;

  const HomeLocationUpdated({required this.latitude, required this.longitude});

  @override
  List<Object?> get props => [latitude, longitude];
}

/// Event when location permission is denied
class HomeLocationPermissionDenied extends HomeEvent {
  const HomeLocationPermissionDenied();
}

/// Event when location stream reports an error
class HomeLocationError extends HomeEvent {
  const HomeLocationError(this.issue);

  final HomeLocationIssue issue;

  @override
  List<Object?> get props => [issue];
}

/// Event to set online status (e.g., from profile API)
class HomeSetOnlineStatus extends HomeEvent {
  final bool isOnline;

  const HomeSetOnlineStatus({required this.isOnline});

  @override
  List<Object?> get props => [isOnline];
}

/// Event to clear success message (used for delayed cleanup)
class HomeClearSuccessMessage extends HomeEvent {
  const HomeClearSuccessMessage();
}

/// Event when trip schedule update notification is received
class HomeTripScheduleUpdateReceived extends HomeEvent {
  const HomeTripScheduleUpdateReceived(this.update);

  final TripScheduleUpdate update;

  @override
  List<Object?> get props => [update];
}

/// Event to dismiss the trip schedule update card
class HomeDismissScheduleUpdate extends HomeEvent {
  const HomeDismissScheduleUpdate();
}

/// User read the in-app location disclosure and will see the system permission dialog.
class HomeContinueLocationPermissionAfterDisclosure extends HomeEvent {
  const HomeContinueLocationPermissionAfterDisclosure();
}

/// User declined the in-app location disclosure (treat as permission denied).
class HomeDismissLocationPermissionDisclosure extends HomeEvent {
  const HomeDismissLocationPermissionDisclosure();
}

/// User read the background-location disclosure; open the next system step.
class HomeContinueBackgroundLocationDisclosure extends HomeEvent {
  const HomeContinueBackgroundLocationDisclosure();
}

/// User skipped background location; continue with while-in-use only if granted.
class HomeDismissBackgroundLocationDisclosure extends HomeEvent {
  const HomeDismissBackgroundLocationDisclosure();
}
