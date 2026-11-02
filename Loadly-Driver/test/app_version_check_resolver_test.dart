import 'package:flutter_test/flutter_test.dart';
import 'package:loadly_driver/core/models/app_update_info.dart';
import 'package:loadly_driver/core/models/app_version_check_dto.dart';
import 'package:loadly_driver/core/services/app_version_check_resolver.dart';

void main() {
  group('AppVersionCheckResolver', () {
    test('returns null when app is up to date', () {
      const dto = AppVersionCheckDto(
        clientVersion: '1.0.0',
        minVersion: '1.0.0',
        latestVersion: '1.0.0',
        forceUpdate: false,
        softUpdate: false,
        storeUrl: 'https://play.google.com/store/apps/details?id=com.loadly.driver',
      );

      expect(
        AppVersionCheckResolver.resolve(currentVersion: '1.0.0', dto: dto),
        isNull,
      );
    });

    test('returns force update when forceUpdate is true', () {
      const dto = AppVersionCheckDto(
        clientVersion: '1.0.0',
        minVersion: '1.0.0',
        latestVersion: '1.0.0',
        forceUpdate: true,
        softUpdate: false,
        storeUrl: 'https://example.com/store',
      );

      final info = AppVersionCheckResolver.resolve(
        currentVersion: '1.0.0',
        dto: dto,
      );

      expect(info?.type, AppUpdateType.force);
      expect(info?.storeUrl, 'https://example.com/store');
    });

    test('returns force update when below minVersion', () {
      const dto = AppVersionCheckDto(
        clientVersion: '1.0.0',
        minVersion: '1.1.0',
        latestVersion: '1.2.0',
        forceUpdate: false,
        softUpdate: false,
        storeUrl: 'https://example.com/store',
      );

      final info = AppVersionCheckResolver.resolve(
        currentVersion: '1.0.0',
        dto: dto,
      );

      expect(info?.type, AppUpdateType.force);
      expect(info?.latestVersion, '1.2.0');
    });

    test('returns optional update when softUpdate is true', () {
      const dto = AppVersionCheckDto(
        clientVersion: '1.0.0',
        minVersion: '1.0.0',
        latestVersion: '1.0.0',
        forceUpdate: false,
        softUpdate: true,
        storeUrl: 'https://example.com/store',
      );

      final info = AppVersionCheckResolver.resolve(
        currentVersion: '1.0.0',
        dto: dto,
      );

      expect(info?.type, AppUpdateType.optional);
    });

    test('returns optional update when below latestVersion', () {
      const dto = AppVersionCheckDto(
        clientVersion: '1.0.0',
        minVersion: '1.0.0',
        latestVersion: '1.1.0',
        forceUpdate: false,
        softUpdate: false,
        storeUrl: 'https://example.com/store',
      );

      final info = AppVersionCheckResolver.resolve(
        currentVersion: '1.0.0',
        dto: dto,
      );

      expect(info?.type, AppUpdateType.optional);
      expect(info?.latestVersion, '1.1.0');
    });

    test('force update takes priority over soft update', () {
      const dto = AppVersionCheckDto(
        clientVersion: '1.0.0',
        minVersion: '1.1.0',
        latestVersion: '1.2.0',
        forceUpdate: true,
        softUpdate: true,
        storeUrl: 'https://example.com/store',
      );

      final info = AppVersionCheckResolver.resolve(
        currentVersion: '1.0.0',
        dto: dto,
      );

      expect(info?.type, AppUpdateType.force);
    });
  });
}
