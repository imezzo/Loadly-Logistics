import '../../../../l10n/gen/app_localizations.dart';

/// Maps API error keys (e.g. trips.trip_must_be_scheduled_to_accept) to localized messages.
String localizeTripError(String? apiMessage, AppLocalizations l10n) {
  if (apiMessage == null || apiMessage.isEmpty) {
    return l10n.tripErrorGeneric;
  }
  final key = apiMessage.trim().toLowerCase();
  switch (key) {
    case 'trips.trip_must_be_scheduled_to_accept':
      return l10n.tripErrorMustBeScheduledToAccept;
    case 'trips.delivery_note_image_required_at_dropoff':
      return l10n.tripErrorDeliveryNoteRequired;
    case 'trips.waypoint_must_be_pending':
      return l10n.tripErrorWaypointMustBePending;
    case 'trips.waypoint_must_be_arrived':
      return l10n.tripErrorWaypointMustBeArrived;
    case 'trips.waypoint_must_be_loading_started':
      return l10n.tripErrorWaypointMustBeLoadingStarted;
    case 'trips.waypoint_must_be_loading_completed':
      return l10n.tripErrorWaypointMustBeLoadingCompleted;
    case 'trips.all_waypoints_must_be_completed':
      return l10n.tripErrorAllWaypointsMustBeCompleted;
    case 'trips.first_waypoint_must_be_loading_completed':
      return l10n.tripErrorFirstWaypointMustBeLoadingCompleted;
    case 'trips.trip_must_be_in_progress':
      return l10n.tripErrorTripMustBeInProgress;
    case 'trips.trip_must_be_driver_accepted':
      return l10n.tripErrorTripMustBeDriverAccepted;
    case 'trips.trip_must_be_first_pickup_loading':
      return l10n.tripErrorTripMustBeFirstPickupLoading;
    case 'trips.invalid_image_format':
      return l10n.tripErrorInvalidImageFormat;
    case 'trips.image_too_large':
      return l10n.tripErrorImageTooLarge;
    case 'trips.image_missing_please_retake':
      return l10n.tripErrorImageMissingRetake;
    default:
      return apiMessage;
  }
}
