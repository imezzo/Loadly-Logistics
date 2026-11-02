/// Workplace API endpoint paths (no leading slash; base URL is set on Dio).
abstract final class ApiEndpoints {
  static const String login = '/auth/login';
  static const String logout = '/auth/logout';
  static const String profile = '/auth/profile';
  static const String forgetPassword = '/auth/forget-password';
  static const String resetPassword = '/auth/reset-password';
  static const String resetPasswordWithCode = '/auth/reset-password-with-code';
  static const String sendMfaCode = '/auth/send-mfa-code';
  static const String verifyMfa = '/auth/verify-mfa';
  static const String refreshToken = '/auth/refresh-token';

  // Homes endpoints
  static const String collectorTrips = '/trips';
  static const String documents = '/documents';

  // Trips endpoints
  static String collectTripDocuments(int tripId) =>
      '/trips/$tripId/documents/collect';
  static String confirmTripDocuments(int tripId) =>
      '/trips/$tripId/documents/confirm';
  static const String bulkCollectTripDocuments =
      '/trips/documents/bulk-collect';
  static const String bulkConfirmTripDocuments =
      '/trips/documents/bulk-confirm';

  // App version check (shared API root, not under /driver)
  static const String appVersionCheck = '/app-version/check';
}
