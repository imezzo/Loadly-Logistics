import '../../l10n/gen/app_localizations.dart';
import '../errors/user_message_codes.dart';

final _arabicPattern = RegExp(r'[\u0600-\u06FF]');

const _technicalMarkers = [
  'exception',
  'stack trace',
  'stacktrace',
  'dioexception',
  'socketexception',
  'formatexception',
  'timeoutexception',
  'httpexception',
  'handshake',
  'failed host lookup',
  'connection refused',
  'connection reset',
  'null check',
  'bad state',
  'instance of',
  "type '",
  'package:',
  'dart:',
  '#0',
  'xmlhttprequest',
  'sqlstate',
  'syntax error',
  'nullpointer',
  'at line',
  '<html',
  'status code',
  'errno',
  'os error',
  'internal server error',
];

const _knownEnglish = <String, String>{
  'network error': UserMessageCodes.network,
  'network error. please try again.': UserMessageCodes.network,
  'network error. please check your connection.': UserMessageCodes.network,
  'no internet connection': UserMessageCodes.noInternet,
  'no internet connection.': UserMessageCodes.noInternet,
  'request failed': UserMessageCodes.requestFailed,
  'an unexpected error occurred': UserMessageCodes.unexpected,
  'an unexpected error occurred. please try again.':
      UserMessageCodes.unexpected,
  'something went wrong': UserMessageCodes.somethingWentWrong,
  'something went wrong.': UserMessageCodes.somethingWentWrong,
  'something went wrong. please try again.': UserMessageCodes.somethingWentWrong,
  'invalid credentials': UserMessageCodes.invalidLogin,
  'invalid credentials.': UserMessageCodes.invalidLogin,
  'invalid credentials. please try again.': UserMessageCodes.invalidLogin,
  'invalid email or password': UserMessageCodes.invalidLogin,
  'invalid email or password.': UserMessageCodes.invalidLogin,
  'these credentials do not match our records.': UserMessageCodes.invalidLogin,
  'the provided credentials are incorrect.': UserMessageCodes.invalidLogin,
  'invalid phone number': UserMessageCodes.invalidPhone,
  'invalid phone number.': UserMessageCodes.invalidPhone,
  'unauthenticated': UserMessageCodes.sessionExpired,
  'unauthenticated.': UserMessageCodes.sessionExpired,
  'unauthorized': UserMessageCodes.sessionExpired,
  'unauthorized.': UserMessageCodes.sessionExpired,
  'invalid or expired session. please sign in again.':
      UserMessageCodes.sessionExpired,
  'invalid request. please check your input.': UserMessageCodes.invalidRequest,
  'bad request': UserMessageCodes.invalidRequest,
  'the given data was invalid': UserMessageCodes.invalidData,
  'the given data was invalid.': UserMessageCodes.invalidData,
  'resource not found': UserMessageCodes.notFound,
  'resource not found.': UserMessageCodes.notFound,
  'not found': UserMessageCodes.notFound,
  'not found.': UserMessageCodes.notFound,
  'this action is not allowed': UserMessageCodes.notAllowed,
  'this action is not allowed.': UserMessageCodes.notAllowed,
  'method not allowed': UserMessageCodes.notAllowed,
  'forbidden': UserMessageCodes.notAllowed,
  'server error': UserMessageCodes.server,
  'server error.': UserMessageCodes.server,
  'server error. please try again later.': UserMessageCodes.server,
  'internal server error': UserMessageCodes.server,
  'failed to load trips': UserMessageCodes.loadTrips,
  'failed to load documents': UserMessageCodes.loadDocuments,
  'failed to collect documents': UserMessageCodes.collectDocuments,
  'failed to confirm documents': UserMessageCodes.confirmDocuments,
  'empty response body': UserMessageCodes.emptyResponse,
  'missing data in response': UserMessageCodes.missingData,
  'too many attempts': UserMessageCodes.tooManyAttempts,
  'too many attempts.': UserMessageCodes.tooManyAttempts,
  'too many requests': UserMessageCodes.tooManyAttempts,
  'invalid code': UserMessageCodes.invalidCode,
  'invalid verification code': UserMessageCodes.invalidCode,
  'the verification code is invalid.': UserMessageCodes.invalidCode,
  'incorrect password': UserMessageCodes.invalidLogin,
  'incorrect password.': UserMessageCodes.invalidLogin,
};

bool containsArabic(String text) => _arabicPattern.hasMatch(text);

bool isTechnicalUserMessage(String text) {
  final lower = text.toLowerCase();
  if (text.length > 180 && !containsArabic(text)) return true;
  for (final marker in _technicalMarkers) {
    if (lower.contains(marker)) return true;
  }
  return false;
}

String codeForStatus(int? code) {
  return switch (code) {
    400 => UserMessageCodes.invalidRequest,
    401 => UserMessageCodes.sessionExpired,
    403 || 405 => UserMessageCodes.notAllowed,
    404 => UserMessageCodes.notFound,
    422 => UserMessageCodes.invalidData,
    429 => UserMessageCodes.tooManyAttempts,
    500 || 502 || 503 || 504 => UserMessageCodes.server,
    _ => UserMessageCodes.unexpected,
  };
}

/// Turns a raw failure or API message into a localization code, or keeps a
/// non-technical Arabic server message that is already safe to display.
String resolveUserMessageCode(
  String? raw, {
  int? statusCode,
  String fallback = UserMessageCodes.unexpected,
}) {
  final trimmed = raw?.trim() ?? '';
  if (trimmed.isEmpty) {
    if (statusCode != null) return codeForStatus(statusCode);
    return fallback;
  }
  if (trimmed.startsWith('error.')) return trimmed;

  final known = _knownEnglish[trimmed.toLowerCase()];
  if (known != null) return known;

  if (isTechnicalUserMessage(trimmed)) {
    if (statusCode != null) return codeForStatus(statusCode);
    return fallback;
  }

  if (containsArabic(trimmed)) return trimmed;

  return UserMessageCodes.unexpected;
}

/// Localized text for a stored message code or a safe Arabic server message.
String localizeUserMessage(AppLocalizations l10n, String? raw) {
  final resolved = resolveUserMessageCode(raw);
  if (!resolved.startsWith('error.')) return resolved;
  return localizedMessageForCode(l10n, resolved);
}

/// Replaces English server copy with [fallback] while keeping Arabic copy.
String localizeServerCopy(
  AppLocalizations l10n,
  String? raw,
  String fallback,
) {
  final trimmed = raw?.trim() ?? '';
  if (trimmed.isEmpty || isTechnicalUserMessage(trimmed)) return fallback;
  if (containsArabic(trimmed)) return trimmed;
  return fallback;
}

String localizedMessageForCode(AppLocalizations l10n, String code) {
  return switch (code) {
    UserMessageCodes.network => l10n.networkErrorPleaseRetry,
    UserMessageCodes.noInternet => l10n.noInternetConnection,
    UserMessageCodes.requestFailed => l10n.requestFailedTryAgain,
    UserMessageCodes.loadTrips => l10n.failedToLoadTrips,
    UserMessageCodes.loadDocuments => l10n.failedToLoadDocuments,
    UserMessageCodes.collectDocuments => l10n.failedToCollectDocuments,
    UserMessageCodes.confirmDocuments => l10n.failedToConfirmDocuments,
    UserMessageCodes.invalidLogin => l10n.invalidLoginCredentials,
    UserMessageCodes.invalidPhone => l10n.invalidPhoneNumber,
    UserMessageCodes.invalidCode => l10n.invalidVerificationCode,
    UserMessageCodes.sessionExpired => l10n.sessionExpired,
    UserMessageCodes.invalidRequest => l10n.invalidRequest,
    UserMessageCodes.notFound => l10n.resourceNotFound,
    UserMessageCodes.notAllowed => l10n.actionNotAllowed,
    UserMessageCodes.invalidData => l10n.invalidSubmittedData,
    UserMessageCodes.server => l10n.serverErrorTryLater,
    UserMessageCodes.loadProfile => l10n.failedToLoadProfile,
    UserMessageCodes.somethingWentWrong => l10n.somethingWentWrongTryAgain,
    UserMessageCodes.emptyResponse => l10n.emptyServerResponse,
    UserMessageCodes.missingData => l10n.missingServerData,
    UserMessageCodes.passwordReset => l10n.passwordResetFailed,
    UserMessageCodes.forgetPassword => l10n.couldNotSendResetCode,
    UserMessageCodes.tooManyAttempts => l10n.tooManyAttempts,
    _ => l10n.unexpectedError,
  };
}
