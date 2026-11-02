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
  final String? title;
  final String? message;
  final String? storeUrl;
  final String? latestVersion;

  bool get isForced => type == AppUpdateType.force;
}
