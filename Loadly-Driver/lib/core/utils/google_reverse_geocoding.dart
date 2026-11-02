import 'package:dio/dio.dart';

import '../config/google_maps_config.dart';

/// Region bias for Google Geocoding (ISO 3166-1 alpha-2).
const kGoogleGeocodingRegionCode = 'eg';

/// Reverse-geocodes [latitude]/[longitude] via Google Geocoding API v3.
///
/// [languageCode] should match the app locale (e.g. `ar`, `en`).
Future<String?> reverseGeocodeLatLng({
  required double latitude,
  required double longitude,
  required String languageCode,
  Dio? dio,
}) async {
  final client = dio ?? Dio();
  try {
    final response = await client.get<Map<String, dynamic>>(
      'https://maps.googleapis.com/maps/api/geocode/json',
      queryParameters: {
        'latlng': '$latitude,$longitude',
        'language': languageCode,
        'region': kGoogleGeocodingRegionCode,
        'key': kGoogleMapsApiKey,
      },
    );
    logGoogleMapsWebServiceStatus('Geocoding reverse', response.data);
    return formattedAddressFromReverseGeocodeResponse(response.data);
  } catch (_) {
    return null;
  }
}

/// Builds a human-readable address from Google reverse-geocoding JSON,
/// without Plus Codes, postal codes, or country name.
String? formattedAddressFromReverseGeocodeResponse(
  Map<String, dynamic>? response,
) {
  final results = response?['results'] as List?;
  if (results == null || results.isEmpty) return null;

  for (final entry in results) {
    if (entry is! Map<String, dynamic>) continue;
    final types = (entry['types'] as List?)?.cast<String>() ?? const [];
    if (types.length == 1 && types.single == 'plus_code') continue;

    final fromComponents = _addressFromComponents(entry);
    if (fromComponents != null && fromComponents.isNotEmpty) {
      return fromComponents;
    }
  }

  final first = results.first;
  if (first is Map<String, dynamic>) {
    return _sanitizeFormattedAddress(first['formatted_address'] as String?);
  }
  return null;
}

const _excludedComponentTypes = {'plus_code', 'postal_code', 'country'};

String? _addressFromComponents(Map<String, dynamic> result) {
  final components = result['address_components'] as List?;
  if (components == null || components.isEmpty) {
    return _sanitizeFormattedAddress(result['formatted_address'] as String?);
  }

  final parts = <String>[];
  for (final component in components) {
    if (component is! Map<String, dynamic>) continue;
    final types = (component['types'] as List?)?.cast<String>() ?? const [];
    if (types.any(_excludedComponentTypes.contains)) continue;

    final name = component['long_name'] as String?;
    if (name == null || name.isEmpty) continue;
    if (parts.contains(name)) continue;
    parts.add(name);
  }

  if (parts.isEmpty) {
    return _sanitizeFormattedAddress(result['formatted_address'] as String?);
  }
  return parts.join(', ');
}

/// Fallback when [address_components] are missing or unusable.
String? _sanitizeFormattedAddress(String? address) {
  if (address == null || address.isEmpty) return address;

  var cleaned = address.replaceAll(
    RegExp(r'\b[A-Z0-9]{4,8}\+[A-Z0-9]{2,4}\b[,،]?\s*'),
    '',
  );
  cleaned = cleaned.replaceAll(
    RegExp(r'[,،]\s*\d{5,7}\s*(?=[,،]|$)'),
    '',
  );
  cleaned = cleaned.replaceAll(RegExp(r'\s{2,}'), ' ').trim();
  cleaned = cleaned.replaceAll(RegExp(r'^[,،]\s*'), '');
  cleaned = cleaned.replaceAll(RegExp(r'[,،]\s*$'), '');
  cleaned = cleaned.replaceAll(
    RegExp(r'[,،]\s*Egypt\s*$', caseSensitive: false),
    '',
  );
  cleaned = cleaned.replaceAll(RegExp(r'[,،]\s*$'), '');
  return cleaned.isEmpty ? address : cleaned;
}
