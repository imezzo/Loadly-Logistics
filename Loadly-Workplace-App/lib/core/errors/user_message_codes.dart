/// Stable identifiers for user-facing messages.
///
/// These values are stored in state and resolved with [AppLocalizations]
/// at the widget layer. They are never shown to the user as-is.
abstract final class UserMessageCodes {
  static const unexpected = 'error.unexpected';
  static const network = 'error.network';
  static const noInternet = 'error.no_internet';
  static const requestFailed = 'error.request_failed';
  static const loadTrips = 'error.load_trips';
  static const loadDocuments = 'error.load_documents';
  static const collectDocuments = 'error.collect_documents';
  static const confirmDocuments = 'error.confirm_documents';
  static const invalidLogin = 'error.invalid_login';
  static const invalidPhone = 'error.invalid_phone';
  static const invalidCode = 'error.invalid_code';
  static const sessionExpired = 'error.session_expired';
  static const invalidRequest = 'error.invalid_request';
  static const notFound = 'error.not_found';
  static const notAllowed = 'error.not_allowed';
  static const invalidData = 'error.invalid_data';
  static const server = 'error.server';
  static const loadProfile = 'error.load_profile';
  static const somethingWentWrong = 'error.something_went_wrong';
  static const emptyResponse = 'error.empty_response';
  static const missingData = 'error.missing_data';
  static const passwordReset = 'error.password_reset';
  static const forgetPassword = 'error.forget_password';
  static const tooManyAttempts = 'error.too_many_attempts';
}
