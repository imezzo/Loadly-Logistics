import 'package:intl/intl.dart';
import 'package:shared_preferences/shared_preferences.dart';

const _appLocaleKey = 'app_locale';

/// Matches embedded UTC timestamps like "12 Aug 2026 11:00 UTC".
final _utcDateInTextPattern = RegExp(
  r'\d{1,2} [A-Za-z]{3} \d{4} \d{1,2}:\d{2} UTC',
  caseSensitive: false,
);

final _utcDateParseFormat = DateFormat('d MMM yyyy HH:mm', 'en');

/// Replaces UTC date strings embedded in [text] with the equivalent local time.
///
/// Input format: `12 Aug 2026 11:00 UTC`
/// Output format: `12 Aug 2026 - 2:00 PM` (locale-aware month name and local time).
String convertUtcDatesInTextToLocal(String text, {String? locale}) {
  return text.replaceAllMapped(_utcDateInTextPattern, (match) {
    final utcText = match.group(0)!;
    final datePart = utcText.substring(0, utcText.length - 4).trim();
    try {
      final local = _utcDateParseFormat.parseUtc(datePart).toLocal();
      return DateFormat('d MMM yyyy - h:mm a', locale).format(local);
    } catch (_) {
      return utcText;
    }
  });
}

Future<String> _appLocale() async {
  final prefs = await SharedPreferences.getInstance();
  return prefs.getString(_appLocaleKey) ?? 'ar';
}

/// Converts UTC dates using the locale saved in app preferences.
Future<String> convertUtcDatesInTextToLocalForApp(String text) async {
  final locale = await _appLocale();
  return convertUtcDatesInTextToLocal(text, locale: locale);
}

/// Formats notification title and body with UTC-to-local date conversion.
Future<({String title, String body})> formatNotificationContent({
  required String title,
  required String body,
}) async {
  final locale = await _appLocale();
  return (
    title: convertUtcDatesInTextToLocal(title, locale: locale),
    body: convertUtcDatesInTextToLocal(body, locale: locale),
  );
}
