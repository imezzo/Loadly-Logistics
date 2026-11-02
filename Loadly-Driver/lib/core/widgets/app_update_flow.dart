import 'package:chucker_flutter/chucker_flutter.dart';
import 'package:flutter/cupertino.dart';

import '../services/app_update_notifier.dart';
import 'app_update_presenter.dart';

/// Blocks the flow when a force update is pending. Optional updates are ignored.
Future<bool> blockOnForcedAppUpdate(BuildContext context) async {
  final info = AppUpdateNotifier.instance.pending;
  if (info?.isForced != true || !AppUpdateNotifier.instance.shouldShow(info!)) {
    return true;
  }

  if (!context.mounted) return false;

  await showAppUpdate(context: context, info: info);
  AppUpdateNotifier.instance.markShown(info);
  return false;
}

/// Presents a pending update on the current screen (e.g. profile).
///
/// Returns `false` when a force update is shown.
Future<bool> presentPendingAppUpdate({required BuildContext context}) async {
  final info = AppUpdateNotifier.instance.pending;
  if (info == null || !AppUpdateNotifier.instance.shouldShow(info)) {
    return true;
  }

  if (!context.mounted) return !info.isForced;

  await showAppUpdate(context: context, info: info);
  AppUpdateNotifier.instance.markShown(info);
  return !info.isForced;
}

/// Handles login navigation with force-update blocking and optional prompts after
/// navigation (same pattern as splash).
Future<bool> handleLoginNavigationWithAppUpdate({
  required BuildContext context,
  required VoidCallback navigate,
}) async {
  final info = AppUpdateNotifier.instance.pending;

  if (info?.isForced == true && AppUpdateNotifier.instance.shouldShow(info!)) {
    if (!context.mounted) return false;
    await showAppUpdate(context: context, info: info);
    AppUpdateNotifier.instance.markShown(info);
    return false;
  }

  navigate();

  if (info != null && AppUpdateNotifier.instance.shouldShow(info)) {
    WidgetsBinding.instance.addPostFrameCallback((_) async {
      final rootContext = ChuckerFlutter.navigatorKey.currentContext;
      if (rootContext != null && rootContext.mounted) {
        await showAppUpdate(context: rootContext, info: info);
      }
    });
    AppUpdateNotifier.instance.markShown(info);
  }

  return true;
}
