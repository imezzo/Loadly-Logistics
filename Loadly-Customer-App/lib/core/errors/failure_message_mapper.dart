import '../../features/auth/domain/failures/auth_failure.dart';
import '../../features/notifications/domain/failures/notifications_failure.dart';
import '../../features/trips/domain/failures/trips_failure.dart';

class FailureMessageMapper {
  static String forAuth(
    AuthFailure failure, {
    String invalidCredentialsMessage = 'Invalid credentials.',
    String networkMessage = 'No internet connection.',
    String unknownMessage = 'Something went wrong.',
  }) {
    return switch (failure) {
      AuthFailureInvalidCredentials() => invalidCredentialsMessage,
      AuthFailureNetwork() => networkMessage,
      AuthFailureServer(message: final message, statusCode: final statusCode) =>
        message ?? defaultForStatusCode(statusCode),
      AuthFailureUnknown(message: final message) => message ?? unknownMessage,
    };
  }

  static String forTrips(
    TripsFailure failure, {
    String serverFallback = 'Request failed',
    String networkFallback = 'Network error. Please check your connection.',
    String unknownFallback = 'An unexpected error occurred',
  }) {
    return switch (failure) {
      TripsFailureServer(message: final message) => message ?? serverFallback,
      TripsFailureNetwork(message: final message) => message ?? networkFallback,
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
      400 => 'Invalid request. Please check your input.',
      401 => 'Invalid or expired session. Please sign in again.',
      404 => 'Resource not found.',
      405 => 'This action is not allowed.',
      422 => 'The given data was invalid.',
      500 => 'Server error. Please try again later.',
      _ => 'Something went wrong. Please try again.',
    };
  }
}
