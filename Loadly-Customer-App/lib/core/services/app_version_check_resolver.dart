import '../models/app_update_info.dart';
import '../models/app_version_check_dto.dart';
import '../utils/version_comparator.dart';

/// Maps API version-check data to an [AppUpdateInfo] prompt, if needed.
abstract final class AppVersionCheckResolver {
  static AppUpdateInfo? resolve({
    required String currentVersion,
    required AppVersionCheckDto dto,
  }) {
    final storeUrl = dto.storeUrl.trim().isEmpty ? null : dto.storeUrl.trim();
    final latestVersion = dto.latestVersion.trim().isEmpty
        ? null
        : dto.latestVersion.trim();

    final belowMinimum =
        dto.minVersion.isNotEmpty &&
        VersionComparator.isLower(currentVersion, dto.minVersion);
    if (dto.forceUpdate || belowMinimum) {
      return AppUpdateInfo(
        type: AppUpdateType.force,
        storeUrl: storeUrl,
        latestVersion: latestVersion ?? dto.minVersion,
      );
    }

    final belowLatest =
        dto.latestVersion.isNotEmpty &&
        VersionComparator.isLower(currentVersion, dto.latestVersion);
    if (dto.softUpdate || belowLatest) {
      return AppUpdateInfo(
        type: AppUpdateType.optional,
        storeUrl: storeUrl,
        latestVersion: latestVersion,
      );
    }

    return null;
  }
}
