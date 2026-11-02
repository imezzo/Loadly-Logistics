import 'package:equatable/equatable.dart';

import '../../../../core/models/trip_schedule_update.dart';
import 'home_location_issue.dart';
import 'home_location_permission_disclosure_context.dart';

/// States for Home BLoC
abstract class HomeState extends Equatable {
  const HomeState();

  @override
  List<Object?> get props => [];
}

/// Loaded state with location and online status
class HomeLoaded extends HomeState {
  final bool isOnline;
  final double? latitude;
  final double? longitude;
  final bool isTrackingLocation;

  /// Go-online / go-offline API or network message (often English from server).
  final String? locationError;

  /// Geolocator / permission / GPS (localized in the UI).
  final HomeLocationIssue? locationIssue;
  final String? successMessage;
  final TripScheduleUpdate? pendingScheduleUpdate;

  /// When non-null, show prominent disclosure in the UI, then continue this flow.
  final HomeLocationPermissionDisclosureContext?
  pendingLocationPermissionDisclosure;

  /// Android/iOS: after "while in use" is granted, second disclosure before
  /// requesting background / "always" location (Play policy for ACCESS_BACKGROUND_LOCATION).
  final HomeLocationPermissionDisclosureContext?
  pendingBackgroundLocationDisclosure;

  const HomeLoaded({
    required this.isOnline,
    this.latitude,
    this.longitude,
    this.isTrackingLocation = false,
    this.locationError,
    this.locationIssue,
    this.successMessage,
    this.pendingScheduleUpdate,
    this.pendingLocationPermissionDisclosure,
    this.pendingBackgroundLocationDisclosure,
  });

  HomeLoaded copyWith({
    bool? isOnline,
    double? latitude,
    double? longitude,
    bool? isTrackingLocation,
    String? locationError,
    bool clearLocationError = false,
    HomeLocationIssue? locationIssue,
    bool clearLocationIssue = false,
    String? successMessage,
    TripScheduleUpdate? pendingScheduleUpdate,
    bool clearPendingScheduleUpdate = false,
    bool clearSuccessMessage = false,
    HomeLocationPermissionDisclosureContext?
    pendingLocationPermissionDisclosure,
    bool clearPendingLocationPermissionDisclosure = false,
    HomeLocationPermissionDisclosureContext?
    pendingBackgroundLocationDisclosure,
    bool clearPendingBackgroundLocationDisclosure = false,
  }) {
    return HomeLoaded(
      isOnline: isOnline ?? this.isOnline,
      latitude: latitude ?? this.latitude,
      longitude: longitude ?? this.longitude,
      isTrackingLocation: isTrackingLocation ?? this.isTrackingLocation,
      locationError: clearLocationError
          ? null
          : (locationError ?? this.locationError),
      locationIssue: clearLocationIssue
          ? null
          : (locationIssue ?? this.locationIssue),
      successMessage: clearSuccessMessage
          ? null
          : (successMessage ?? this.successMessage),
      pendingScheduleUpdate: clearPendingScheduleUpdate
          ? null
          : (pendingScheduleUpdate ?? this.pendingScheduleUpdate),
      pendingLocationPermissionDisclosure:
          clearPendingLocationPermissionDisclosure
          ? null
          : (pendingLocationPermissionDisclosure ??
                this.pendingLocationPermissionDisclosure),
      pendingBackgroundLocationDisclosure:
          clearPendingBackgroundLocationDisclosure
          ? null
          : (pendingBackgroundLocationDisclosure ??
                this.pendingBackgroundLocationDisclosure),
    );
  }

  /// Retry after the user returns from system settings (GPS or app permissions).
  /// Do not retry while waiting for the user to read the permission disclosure.
  bool get shouldRetryLocationOnResume =>
      locationIssue != null &&
      pendingLocationPermissionDisclosure == null &&
      pendingBackgroundLocationDisclosure == null;

  @override
  List<Object?> get props => [
    isOnline,
    latitude,
    longitude,
    isTrackingLocation,
    locationError,
    locationIssue,
    successMessage,
    pendingScheduleUpdate,
    pendingLocationPermissionDisclosure,
    pendingBackgroundLocationDisclosure,
  ];
}
