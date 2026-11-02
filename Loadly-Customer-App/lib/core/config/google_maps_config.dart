import 'package:flutter/foundation.dart';

/// Google Maps / Web Service API key used by Dart (Directions, Geocoding REST).
///
/// **Build:** pass at compile time so CI/release do not rely on a key in source:
/// `flutter run --dart-define=GOOGLE_MAPS_API_KEY=your_key`
/// `flutter build apk --dart-define=GOOGLE_MAPS_API_KEY=your_key`
/// The value must match the native SDK key:
/// - Android: `AndroidManifest.xml` → `com.google.android.geo.API_KEY`
/// - iOS: `AppDelegate.swift` → `GMSServices.provideAPIKey(...)`
///
/// **GCP guardrails (manual in Cloud Console):**
/// - Billing → Budgets & alerts on the Maps billing account.
/// - APIs & Services → Directions API → Quotas → set sensible daily caps if needed.
/// - Billing reports: group by SKU to see multi-waypoint vs basic Directions usage.
/// - If Directions error rate is high: check API key restrictions — Android/iOS app
///   restrictions often do **not** apply to Web Service REST calls the same way as
///   the Maps SDK; you may need a separate key or IP-less Web Service restrictions.
const String kGoogleMapsApiKey = String.fromEnvironment(
  'GOOGLE_MAPS_API_KEY',
  defaultValue: 'AIzaSyAW0huedjS-G63-Ob_VkTSCcBlKe-BXi4Q',
);

/// Debug-only: log non-OK [status] / [error_message] for Geocoding, etc.
void logGoogleMapsWebServiceStatus(String context, Map<String, dynamic>? data) {
  if (!kDebugMode || data == null) return;
  final status = data['status'];
  if (status == 'OK') return;
  debugPrint(
    'Google Maps Web Service [$context]: status=$status '
    'error_message=${data['error_message']}',
  );
}
