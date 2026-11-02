/// Customer API endpoint paths (no leading slash; base URL is set on Dio).
abstract final class ApiEndpoints {
  // Auth endpoints
  static const String login = '/company/auth/login';
  static const String sendLoginOtp = '/company/auth/send-login-otp';
  static const String loginWithOtp = '/company/auth/login-with-otp';
  static const String logout = '/company/auth/logout';
  static const String profile = '/company/auth/profile';
  static const String updateProfile = '/company/auth/profile';
  static const String forgetPassword = '/company/auth/forget-password';
  static const String resetPassword = '/company/auth/reset-password';
  static const String resetPasswordWithCode =
      '/company/auth/reset-password-with-code';
  static const String changePassword = '/company/auth/change-password';
  static const String sendMfaCode = '/company/auth/send-mfa-code';
  static const String verifyMfa = '/company/auth/verify-mfa';
  static const String enableMfa = '/company/auth/enable-mfa';
  static const String disableMfa = '/company/auth/disable-mfa';
  static const String verifyEmail = '/company/auth/verify-email';
  static const String resendVerification = '/company/auth/resend-verification';
  static const String refreshToken = '/company/auth/refresh-token';
  static const String validatePassword = '/company/auth/validate-password';

  // Home endpoints
  static const String tripsStatistics = '/company/dashboard/trips';
  static const String monthlyTripsKilometers =
      '/company/dashboard/monthly-trips-kilometers';

  // Trips endpoints
  static const String trips = '/company/trips';
  static const String contractReferenceNumbers =
      '/lookups/contractreferencenumbers/default';
  static const String vehicleTypes = '/lookups/vehicletypes/default';
  static const String contractDestinations =
      '/lookups/contractdestinations/default';
  static String contractDestination(int id) =>
      '/company/contracts/destinations/$id';
  static String consumptionType(int contractId) =>
      '/company/contracts/$contractId/consumption-type';
  static const String extractPlace = '/company/maps/extract-place';
  static const String trackTrips = '/company/trips/track';

  // Reports endpoints
  static const String tripCostPeriod =
      '/company/dashboard/trip-cost-period-totals';
  static const String invoiceStatistics =
      '/company/dashboard/invoice-statistics';
  static const String invoices = '/company/invoices';
  static const String invoicesPaymentRequest =
      '/company/invoice-payment-requests';

  // notifications endpoints
  static const String notifications = '/company/notifications';
  static String notificationRead(int id) => '/company/notifications/$id/read';

  // App version check (shared API root)
  static const String appVersionCheck = '/app-version/check';
}
