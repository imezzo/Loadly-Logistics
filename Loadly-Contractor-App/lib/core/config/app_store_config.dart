import 'dart:io' show Platform;

/// Store listing URLs for opening the update flow in the platform store.
abstract final class AppStoreConfig {
  static const androidPackageId = 'com.loadly.contractor';

  /// Numeric App Store id (e.g. 1234567890). Pass via:
  /// `--dart-define=IOS_APP_STORE_ID=...`
  static const iosAppStoreId = String.fromEnvironment('IOS_APP_STORE_ID');

  /// Huawei AppGallery app id (e.g. C101234567). Pass via:
  /// `--dart-define=HUAWEI_APP_GALLERY_ID=...`
  static const huaweiAppGalleryId = String.fromEnvironment(
    'HUAWEI_APP_GALLERY_ID',
  );

  /// Android distribution channel. Use `harmonyos` for Huawei AppGallery builds:
  /// `--dart-define=APP_STORE_DISTRIBUTION=harmonyos`
  static const storeDistribution = String.fromEnvironment(
    'APP_STORE_DISTRIBUTION',
    defaultValue: 'play',
  );

  /// Override the full store URL via:
  /// `--dart-define=APP_STORE_URL=...`
  static const storeUrlOverride = String.fromEnvironment('APP_STORE_URL');

  static bool get isHarmonyOsDistribution {
    final value = storeDistribution.toLowerCase();
    return value == 'harmonyos' || value == 'appgallery';
  }

  /// Platform value sent to `/app-version/check`.
  static String get versionCheckPlatform {
    if (Platform.isIOS) return 'iOS';
    if (isHarmonyOsDistribution) return 'HarmonyOS';
    return 'Android';
  }

  static String get storeUrl {
    if (storeUrlOverride.isNotEmpty) {
      return storeUrlOverride;
    }
    if (Platform.isIOS && iosAppStoreId.isNotEmpty) {
      return 'https://apps.apple.com/app/id$iosAppStoreId';
    }
    if (isHarmonyOsDistribution && huaweiAppGalleryId.isNotEmpty) {
      return 'https://appgallery.huawei.com/app/C$huaweiAppGalleryId';
    }
    return 'https://play.google.com/store/apps/details?id=$androidPackageId';
  }
}
