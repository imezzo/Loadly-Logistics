import '../../../../core/locale/user_facing_message.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../bloc/auth_state.dart';

extension AuthErrorExtensions on AuthError {
  /// Localized message for known error codes, otherwise a safe user-facing message.
  String getDisplayMessage(AppLocalizations l10n) {
    switch (errorCode) {
      case 'biometric_failed':
        return l10n.biometricErrorFailedOrCancelled;
      case 'biometric_enable_failed':
        return l10n.biometricErrorEnableFailed;
      case 'biometric_disable_failed':
        return l10n.biometricErrorDisableFailed;
      default:
        return localizeUserMessage(l10n, message);
    }
  }
}
