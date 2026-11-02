/// Stable tokens stored on API failures and resolved in [FailureMessageMapper].
abstract final class ApiFallbackKeys {
  static const loadDrivers = 'failedToLoadDrivers';
  static const loadDocumentTypes = 'failedToLoadDocumentTypes';
  static const createDriver = 'failedToCreateDriver';
  static const updateTripDriver = 'failedToUpdateTripDriver';
  static const loadTrip = 'failedToLoadTrip';
  static const loadTrips = 'failedToLoadTrips';
  static const loadBookingRequests = 'failedToLoadBookingRequests';
  static const bookTrip = 'failedToBookTrip';
  static const withdrawBooking = 'failedToWithdrawBooking';
  static const loadVehicleTypes = 'failedToLoadVehicleTypes';
  static const loadNotifications = 'failedToLoadNotifications';
  static const loadMore = 'failedToLoadMore';
  static const markAsRead = 'failedToMarkAsRead';
  static const requestFailed = 'requestFailed';
  static const registrationFailed = 'registrationFailed';
  static const invalidResponse = 'invalidResponse';
  static const invalidCredentials = 'invalidCredentials';
  static const networkError = 'network_error';
  static const unexpectedError = 'unknown_error';
}
