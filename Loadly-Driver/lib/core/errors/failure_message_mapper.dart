import '../../features/auth/domain/failures/auth_failure.dart';
import '../../features/home/domain/failures/home_failure.dart';
import '../../features/notifications/domain/failures/notifications_failure.dart';
import '../../features/trips/domain/failures/trips_failure.dart';

class FailureMessageMapper {
  static String forAuth(
    AuthFailure failure, {
    String invalidCredentialsMessage = 'auth.invalid_credentials',
    String networkMessage = 'auth.network_error',
    String unknownMessage = 'auth.unknown_error',
  }) {
    return switch (failure) {
      AuthFailureInvalidCredentials() => invalidCredentialsMessage,
      AuthFailureNetwork() => networkMessage,
      AuthFailureServer(message: final message, statusCode: final statusCode) =>
        message ?? defaultForStatusCode(statusCode),
      AuthFailureUnknown(message: final message) => message ?? unknownMessage,
    };
  }

  static String forHome(
    HomeFailure failure, {
    String serverFallback = 'Request failed',
    String networkFallback = 'Network error. Please try again.',
    String unknownFallback = 'An unexpected error occurred',
  }) {
    return switch (failure) {
      HomeFailureServer(message: final message) => message ?? serverFallback,
      HomeFailureNetwork(message: final message) => message ?? networkFallback,
      HomeFailureUnknown(message: final message) => message ?? unknownFallback,
      _ => unknownFallback,
    };
  }

  static String forTrips(
    TripsFailure failure, {
    String serverFallback = 'Request failed',
    String networkFallback = 'Network error. Please check your connection.',
    String missingFilesFallback =
        'One or more photos could not be found. Please retake them and try again.',
    String unknownFallback = 'An unexpected error occurred',
  }) {
    return switch (failure) {
      TripsFailureServer(message: final message) => message ?? serverFallback,
      TripsFailureNetwork(message: final message) => message ?? networkFallback,
      TripsFailureMissingFiles(message: final message) =>
        message ?? missingFilesFallback,
      TripsFailureUnknown(message: final message) => message ?? unknownFallback,
      _ => unknownFallback,
    };
  }

  static String forNotifications(
    NotificationsFailure failure, {
    String serverFallback = 'Request failed',
    String networkFallback = 'Network error. Please check your connection.',
    String unknownFallback = 'An unexpected error occurred',
  }) {
    return switch (failure) {
      NotificationsFailureServer(message: final message) =>
        message ?? serverFallback,
      NotificationsFailureNetwork(message: final message) =>
        message ?? networkFallback,
      _ => unknownFallback,
    };
  }

  static String defaultForStatusCode(int? code) {
    return switch (code) {
      400 => 'auth.error_400',
      401 => 'auth.error_401',
      404 => 'auth.error_404',
      405 => 'auth.error_405',
      422 => 'auth.error_422',
      500 => 'auth.error_500',
      _ => 'auth.error_generic',
    };
  }
}
