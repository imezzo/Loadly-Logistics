import '../../features/auth/domain/failures/auth_failure.dart';
import '../../features/notifications/domain/failures/notifications_failure.dart';
import '../../l10n/gen/app_localizations.dart';
import '../network/api_failure_mapper.dart';
import 'api_fallback_keys.dart';
import 'failure.dart';

/// Maps auth failures to user-facing display strings.
/// This is a lightweight replacement for the Loadly Contractor shared mapper.
class FailureMessageMapper {
  static String forMessage(String message, AppLocalizations l10n) {
    switch (message.trim()) {
      case 'network_error':
        return l10n.networkError;
      case 'unauthorized':
        return l10n.unknownError;
      case 'server_error':
        return l10n.unknownError;
      case 'unknown_error':
        return l10n.unknownError;
      case ApiFallbackKeys.loadDrivers:
        return l10n.failedToLoadDrivers;
      case ApiFallbackKeys.loadDocumentTypes:
        return l10n.failedToLoadDocumentTypes;
      case ApiFallbackKeys.createDriver:
        return l10n.failedToCreateDriver;
      case ApiFallbackKeys.updateTripDriver:
        return l10n.failedToUpdateTripDriver;
      case ApiFallbackKeys.loadTrip:
        return l10n.failedToLoadTrip;
      case ApiFallbackKeys.loadTrips:
        return l10n.failedToLoadTrips;
      case ApiFallbackKeys.loadBookingRequests:
        return l10n.failedToLoadBookingRequests;
      case ApiFallbackKeys.bookTrip:
        return l10n.failedToBookTrip;
      case ApiFallbackKeys.withdrawBooking:
        return l10n.failedToWithdrawBooking;
      case ApiFallbackKeys.loadVehicleTypes:
        return l10n.failedToLoadVehicleTypes;
      case ApiFallbackKeys.loadNotifications:
        return l10n.failedToLoadNotifications;
      case ApiFallbackKeys.loadMore:
        return l10n.failedToLoadMore;
      case ApiFallbackKeys.markAsRead:
        return l10n.failedToMarkAsRead;
      case ApiFallbackKeys.requestFailed:
        return l10n.requestFailed;
      case ApiFallbackKeys.registrationFailed:
        return l10n.registrationFailed;
      case ApiFallbackKeys.invalidResponse:
        return l10n.invalidResponse;
      case ApiFallbackKeys.invalidCredentials:
      case 'auth.invalid_credentials':
        return l10n.invalidCredentials;
    }

    final text = message.trim();
    if (text.isEmpty || !isUserFacingApiMessage(text)) {
      return l10n.unknownError;
    }
    return text;
  }

  static String forNotifications(
    NotificationsFailure failure, {
    String serverFallback = ApiFallbackKeys.requestFailed,
    String networkFallback = ApiFallbackKeys.networkError,
    String unknownFallback = ApiFallbackKeys.unexpectedError,
  }) {
    return switch (failure) {
      NotificationsFailureServer(message: final message) =>
        message ?? serverFallback,
      NotificationsFailureNetwork(message: final message) =>
        message ?? networkFallback,
      _ => unknownFallback,
    };
  }

  static String forGeneric(Failure failure, AppLocalizations l10n) {
    return forMessage(failure.message, l10n);
  }

  static String forAuth(
    AuthFailure failure, {
    String invalidCredentialsMessage = 'auth.invalid_credentials',
  }) {
    return switch (failure) {
      AuthFailureInvalidCredentials(:final message) =>
        message ?? invalidCredentialsMessage,
      AuthFailureServer(:final message) => message ?? 'server_error',
      AuthFailureNetwork() => 'network_error',
      AuthFailureUnknown(:final message) => message ?? 'unknown_error',
    };
  }

  /// Resolves profile error [message] tokens (and API text) to localized strings.
  static String localizeProfileErrorMessage(
    String message,
    AppLocalizations l10n,
  ) {
    final text = message.trim();
    switch (text) {
      case 'invalidCredentials':
        return l10n.invalidCredentials;
      case 'networkError':
        return l10n.networkError;
      case 'unknownError':
        return l10n.unknownError;
      case 'error400':
        return l10n.error400;
      case 'error401':
        return l10n.error401;
      case 'error404':
        return l10n.error404;
      case 'error405':
        return l10n.error405;
      case 'error422':
        return l10n.error422;
      case 'error500':
        return l10n.error500;
      case 'errorDefault':
        return l10n.errorDefault;
    }
    return forMessage(text, l10n);
  }
}
