/// Driver API endpoint paths (no leading slash; base URL is set on Dio).
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
  static const String verifyEmail = '/auth/verify-email';
  static const String resendVerification = '/auth/resend-verification';
  static const String driverDocuments = '/driver/documents';
  static String driverDocumentById(int id) => '/driver/documents/$id';
  static const String refreshToken = '/auth/refresh-token';
  static const String validatePassword = '/auth/validate-password';

  // Home/Driver status endpoints
  static const String goOnline = '/go-online';
  static const String goOffline = '/go-offline';
  static const String locationPing = '/location/ping';

  // Trips endpoints
  static const String trips = '/trips';

  static String trip(int id) => '/trips/$id';
  static String tripStart(int tripId) => '/trips/$tripId/start';
  static String tripComplete(int tripId) => '/trips/$tripId/complete';
  static String tripDocumentDownload(int tripId, String path) =>
      '/trips/$tripId/documents/download?path=${Uri.encodeComponent(path)}';
  static String tripWaypointLoadingStart(int tripId, int waypointId) =>
      '/trips/$tripId/waypoints/$waypointId/loading/start';
  static String tripWaypointLoadingComplete(int tripId, int waypointId) =>
      '/trips/$tripId/waypoints/$waypointId/loading/complete';
  static String tripWaypointUnloadingStart(int tripId, int waypointId) =>
      '/trips/$tripId/waypoints/$waypointId/unloading/start';
  static String tripWaypointUnloadingComplete(int tripId, int waypointId) =>
      '/trips/$tripId/waypoints/$waypointId/unloading/complete';
  static String tripWaypointArrive(int tripId, int waypointId) =>
      '/trips/$tripId/waypoints/$waypointId/arrive';
  static String tripWaypointDepart(int tripId, int waypointId) =>
      '/trips/$tripId/waypoints/$waypointId/depart';
  static String tripEnterCustoms(int tripId) => '/trips/$tripId/customs/enter';
  static String tripExitCustoms(int tripId) => '/trips/$tripId/customs/exit';
  static String validateOdometerImage(int tripId) =>
      '/trips/$tripId/odometer/suggest';
  static String tripRouteSync(int tripId) => '/trips/$tripId/route-sync';

  // Notifications endpoints
  static const String notifications = '/notifications';
  static String notificationRead(int id) => '/notifications/$id/read';

  // App version check (shared API root, not under /driver)
  static const String appVersionCheck = '/app-version/check';
}
