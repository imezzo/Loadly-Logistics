/// contractor API endpoint paths (no leading slash; base URL is set on Dio).
abstract final class ApiEndpoints {
  static const String login = '/auth/login';
  static const String sendLoginOtp = '/auth/send-login-otp';
  static const String loginWithOtp = '/auth/login-with-otp';
  static const String logout = '/auth/logout';
  static const String profile = '/auth/profile';
  static const String updateProfile = '/auth/profile';
  static const String forgetPassword = '/auth/forget-password';
  static const String resetPassword = '/auth/reset-password';
  static const String resetPasswordWithCode = '/auth/reset-password-with-code';
  static const String changePassword = '/auth/change-password';
  static const String sendMfaCode = '/auth/send-mfa-code';
  static const String verifyMfa = '/auth/verify-mfa';
  static const String enableMfa = '/auth/enable-mfa';
  static const String disableMfa = '/auth/disable-mfa';
  static const String refreshToken = '/auth/refresh-token';
  static const String validatePassword = '/auth/validate-password';
  static const register = '/auth/register';

  /// Shared API root (`kApiBaseUrl`), not under `/contractor`.
  static const vehicleTypesDefault = '/lookups/vehicletypes/default';

  // Trips endpoints
  static const documents = '/documents';
  static const driverDocumentsEntityId = 8;
  static const String trips = '/trips';
  static const String invitationStatistics = '/trips/invitation-statistics';
  static const String statusStatistics = '/trips/status-statistics';
  static const String payoutStatistics = '/trips/payout-statistics';
  static String tripById(int tripId) => '/trips/$tripId';
  static String bookTrip(int tripId) => '/trips/$tripId/apply';
  static String withdrawTrip(int tripId) => '/trips/$tripId/withdraw';
  static String updateTripDriver(int tripId) => '/trips/$tripId/driver';
  static const String bookingRequests = '/booking-requests';
  static String updateBookingRequest(int bookingRequestId) =>
      '/booking-requests/$bookingRequestId';
  static const String drivers = '/drivers';

  // Notifications endpoints
  static const String notifications = '/notifications';
  static String notificationRead(int id) => '/notifications/$id/read';

  // App version check (shared API root, not under /contractor)
  static const String appVersionCheck = '/app-version/check';
}
