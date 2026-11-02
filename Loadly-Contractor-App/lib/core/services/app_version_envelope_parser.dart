import '../models/app_version_check_dto.dart';

/// Extracts version policy from standard API envelopes (login, profile, etc.).
abstract final class AppVersionEnvelopeParser {
  static const _nestedKeys = ['appVersionCheck', 'versionCheck', 'appVersion'];

  static AppVersionCheckDto? extractDto(Map<String, dynamic> envelope) {
    for (final key in _nestedKeys) {
      final nested = envelope[key];
      if (nested is Map<String, dynamic>) {
        return AppVersionCheckDto.fromJson(nested);
      }
    }

    final data = envelope['data'];
    if (data is Map<String, dynamic>) {
      for (final key in _nestedKeys) {
        final nested = data[key];
        if (nested is Map<String, dynamic>) {
          return AppVersionCheckDto.fromJson(nested);
        }
      }
      if (_hasVersionPolicyFields(data)) {
        return AppVersionCheckDto.fromJson(data);
      }
    }

    if (_hasVersionPolicyFields(envelope)) {
      return AppVersionCheckDto.fromJson(envelope);
    }

    return null;
  }

  static bool _hasVersionPolicyFields(Map<String, dynamic> json) {
    return json.containsKey('forceUpdate') ||
        json.containsKey('softUpdate') ||
        json.containsKey('minVersion') ||
        json.containsKey('latestVersion');
  }
}
