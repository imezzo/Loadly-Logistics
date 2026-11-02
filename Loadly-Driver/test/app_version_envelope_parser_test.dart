import 'package:flutter_test/flutter_test.dart';
import 'package:loadly_driver/core/services/app_version_envelope_parser.dart';

void main() {
  group('AppVersionEnvelopeParser', () {
    test('reads nested appVersionCheck on envelope root', () {
      final dto = AppVersionEnvelopeParser.extractDto({
        'success': true,
        'data': {'accessToken': 'token'},
        'appVersionCheck': {
          'clientVersion': '1.0.0',
          'minVersion': '1.0.0',
          'latestVersion': '1.1.0',
          'forceUpdate': false,
          'softUpdate': true,
          'storeUrl': 'https://example.com/store',
        },
      });

      expect(dto?.latestVersion, '1.1.0');
      expect(dto?.softUpdate, isTrue);
    });

    test('reads version policy fields from data payload', () {
      final dto = AppVersionEnvelopeParser.extractDto({
        'success': true,
        'data': {
          'accessToken': 'token',
          'minVersion': '1.2.0',
          'latestVersion': '1.3.0',
          'forceUpdate': true,
          'softUpdate': false,
          'storeUrl': '',
        },
      });

      expect(dto?.forceUpdate, isTrue);
      expect(dto?.minVersion, '1.2.0');
    });

    test('returns null when no version policy is present', () {
      final dto = AppVersionEnvelopeParser.extractDto({
        'success': true,
        'data': {'accessToken': 'token'},
      });

      expect(dto, isNull);
    });
  });
}
