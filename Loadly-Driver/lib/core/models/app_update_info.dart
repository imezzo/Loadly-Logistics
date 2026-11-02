/// Whether the user must update before continuing or may dismiss the prompt.
enum AppUpdateType { force, optional }

/// Data needed to show a force or optional app update prompt.
class AppUpdateInfo {
  const AppUpdateInfo({
    required this.type,
    this.title,
    this.message,
    this.storeUrl,
    this.latestVersion,
  });

  final AppUpdateType type;

  /// Optional override; defaults to localized title when null.
  final String? title;

  /// Optional override; defaults to localized message when null.
  final String? message;

  /// Play Store, App Store, or AppGallery URL. Falls back to [AppStoreConfig.storeUrl].
  final String? storeUrl;

  /// Shown below the message when provided (e.g. "1.2.0").
  final String? latestVersion;

  bool get isForced => type == AppUpdateType.force;
}
