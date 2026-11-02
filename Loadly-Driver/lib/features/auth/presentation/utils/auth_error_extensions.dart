import '../../../../l10n/gen/app_localizations.dart';
import '../bloc/auth_state.dart';

extension AuthErrorExtensions on AuthError {
  /// Returns localized message for known error codes (e.g. biometric), otherwise the raw message.
  String getDisplayMessage(AppLocalizations l10n) {
    if (errorCode == null) {
      switch (message) {
        case 'biometric_failed':
          return l10n.biometricErrorFailedOrCancelled;
        case 'biometric_enable_failed':
          return l10n.biometricErrorEnableFailed;
        case 'biometric_disable_failed':
          return l10n.biometricErrorDisableFailed;
        case 'auth.invalid_phone_number':
          return l10n.invalidPhoneNumber;
        case 'auth.invalid_credentials':
          return l10n.invalidCredentials;
        case 'auth.network_error':
          return l10n.networkError;
        case 'auth.unknown_error':
          return l10n.unknownError;
        default:
          return message;
      }
    } else {
      switch (errorCode) {
        case 'biometric_failed':
          return l10n.biometricErrorFailedOrCancelled;
        case 'biometric_enable_failed':
          return l10n.biometricErrorEnableFailed;
        case 'biometric_disable_failed':
          return l10n.biometricErrorDisableFailed;
        case 'auth.invalid_phone_number':
          return l10n.invalidPhoneNumber;
        case 'auth.invalid_credentials':
          return l10n.invalidCredentials;
        case 'auth.network_error':
          return l10n.networkError;
        case 'auth.unknown_error':
          return l10n.unknownError;
        default:
          return message;
      }
    }
  }
}
