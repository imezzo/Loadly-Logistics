import '../models/app_update_info.dart';

/// Holds the latest app-update prompt resolved from login/profile API responses.
class AppUpdateNotifier {
  AppUpdateNotifier._();

  static final AppUpdateNotifier instance = AppUpdateNotifier._();

  AppUpdateInfo? _pending;
  String? _shownKey;

  AppUpdateInfo? get pending => _pending;

  void setPending(AppUpdateInfo? info) {
    if (info == null) return;
    _pending = info;
  }

  void clearPending() => _pending = null;

  bool shouldShow(AppUpdateInfo info) {
    final key = _promptKey(info);
    return _shownKey != key;
  }

  void markShown(AppUpdateInfo info) {
    _shownKey = _promptKey(info);
    if (!info.isForced) {
      _pending = null;
    }
  }

  String _promptKey(AppUpdateInfo info) {
    return '${info.type.name}:${info.latestVersion ?? ''}';
  }
}
