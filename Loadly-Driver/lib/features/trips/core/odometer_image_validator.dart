import 'dart:io';

import 'package:google_mlkit_text_recognition/google_mlkit_text_recognition.dart';
import 'package:flutter/services.dart';

/// Returns null when the image looks like an odometer photo, otherwise an error key.
Future<String?> validateOdometerImage(File file) async {
  final textRecognizer = TextRecognizer(script: TextRecognitionScript.latin);
  try {
    final inputImage = InputImage.fromFile(file);
    final recognizedText = await textRecognizer.processImage(inputImage);
    final flattened = recognizedText.text
        .replaceAll('\n', ' ')
        .toUpperCase()
        .trim();

    if (flattened.isEmpty) {
      return 'trips.odometer_image_unreadable';
    }

    // Reject images that look like screens (monitors, browsers, desktops).
    if (_screenIndicators.any(flattened.contains)) {
      return 'trips.odometer_image_not_detected';
    }

    final hasMileageKeyword = _odometerKeywords.any(flattened.contains);
    final tokens = flattened
        .split(' ')
        .map((token) => token.trim())
        .where((token) => token.isNotEmpty);
    final hasMileageUnit = tokens.any(
      (token) => _mileageUnitTokens.contains(token),
    );
    final digitTokens = _extractDigitRuns(
      flattened,
    ).where((run) => run.length >= 3 && run.length <= 7).toList();

    // Accept if OCR sees an odometer-ish context and a plausible reading.
    final hasPlausibleReading = digitTokens.isNotEmpty;
    if (hasPlausibleReading && (hasMileageKeyword || hasMileageUnit)) {
      return null;
    }

    return 'trips.odometer_image_not_detected';
  } catch (_) {
    return 'trips.odometer_image_unreadable';
  } finally {
    try {
      await textRecognizer.close();
    } on MissingPluginException {
      // Can happen during hot-reload/hot-restart transitions after adding plugin.
      // Do not crash capture flow; a full app restart registers the plugin correctly.
    }
  }
}

const List<String> _odometerKeywords = [
  'ODO',
  'ODOMETER',
  'TOTAL',
  'TRIP',
  'KILOMETER',
  'KILOMETRE',
  'MILEAGE',
  // Common Arabic words that may appear on some clusters/UI overlays.
  'عداد',
  'المسافة',
  'المسافات',
  'كم',
];

List<String> _extractDigitRuns(String input) {
  final runs = <String>[];
  final buffer = StringBuffer();

  void flush() {
    if (buffer.isNotEmpty) {
      runs.add(buffer.toString());
      buffer.clear();
    }
  }

  for (final codeUnit in input.codeUnits) {
    final isDigit = codeUnit >= 48 && codeUnit <= 57;
    if (isDigit) {
      buffer.writeCharCode(codeUnit);
    } else {
      flush();
    }
  }
  flush();
  return runs;
}

const Set<String> _mileageUnitTokens = {'KM', 'KMS', 'MI', 'MILE', 'MILES'};

/// Terms that indicate a screen/monitor rather than a physical odometer.
/// If any of these appear in the OCR text, the image is rejected.
const List<String> _screenIndicators = [
  'HTTP',
  'HTTPS',
  'WWW.',
  '.COM',
  '.NET',
  '.ORG',
  'CHROME',
  'FIREFOX',
  'SAFARI',
  'EDGE',
  'BROWSER',
  'WINDOWS',
  'MACOS',
  'RESOLUTION',
  'PIXEL',
  'DOWNLOAD',
  'REFRESH',
  'ZOOM',
  'SCROLL',
];
