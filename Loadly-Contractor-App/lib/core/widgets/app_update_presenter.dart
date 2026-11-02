import 'package:flutter/cupertino.dart';
import 'package:url_launcher/url_launcher.dart';

import '../../l10n/gen/app_localizations.dart';
import '../config/app_store_config.dart';
import '../models/app_update_info.dart';
import 'app_update_screen.dart';

/// Opens the platform store listing for the app.
Future<bool> openAppStoreListing({String? storeUrl}) async {
  final uri = Uri.tryParse(storeUrl ?? AppStoreConfig.storeUrl);
  if (uri == null) return false;

  return launchUrl(uri, mode: LaunchMode.externalApplication);
}

/// Shows a force or optional update UI for [info].
///
/// Force updates push a blocking full-screen route. Optional updates use a
/// Cupertino dialog. Returns `true` when the user chooses "Later" on an
/// optional update, `false` when they tap update, and `null` for force updates.
Future<bool?> showAppUpdate({
  required BuildContext context,
  required AppUpdateInfo info,
}) async {
  if (info.isForced) {
    await _pushForceUpdateRoute(context, info);
    return null;
  }

  final l10n = AppLocalizations.of(context)!;
  final title = info.title ?? l10n.appUpdateAvailableTitle;
  final message = info.message ?? l10n.appUpdateAvailableMessage;
  final versionLine =
      info.latestVersion != null && info.latestVersion!.isNotEmpty
      ? '\n\n${l10n.appUpdateLatestVersion(info.latestVersion!)}'
      : '';

  final dismissed = await showCupertinoDialog<bool>(
    context: context,
    barrierDismissible: true,
    builder: (BuildContext dialogContext) => CupertinoAlertDialog(
      title: Text(title),
      content: Text('$message$versionLine'),
      actions: [
        CupertinoDialogAction(
          onPressed: () => Navigator.of(dialogContext).pop(true),
          child: Text(l10n.appUpdateLater),
        ),
        CupertinoDialogAction(
          isDefaultAction: true,
          onPressed: () {
            Navigator.of(dialogContext).pop(false);
            openAppStoreListing(storeUrl: info.storeUrl);
          },
          child: Text(l10n.appUpdateNow),
        ),
      ],
    ),
  );

  return dismissed;
}

/// Pushes a blocking full-screen force update screen.
Future<void> showForceAppUpdate({
  required BuildContext context,
  required AppUpdateInfo info,
}) {
  assert(info.isForced, 'Use showAppUpdate for optional updates.');
  return _pushForceUpdateRoute(context, info);
}

Future<void> _pushForceUpdateRoute(BuildContext context, AppUpdateInfo info) {
  return Navigator.of(context).push<void>(
    PageRouteBuilder<void>(
      opaque: true,
      barrierDismissible: false,
      pageBuilder:
          (
            BuildContext context,
            Animation<double> animation,
            Animation<double> secondaryAnimation,
          ) {
            return AppUpdateScreen(
              info: info,
              onUpdate: () => openAppStoreListing(storeUrl: info.storeUrl),
            );
          },
    ),
  );
}

/// Pushes a full-screen optional update screen with a "Later" action.
Future<bool?> showOptionalAppUpdateScreen({
  required BuildContext context,
  required AppUpdateInfo info,
}) {
  assert(!info.isForced, 'Use showForceAppUpdate for force updates.');

  return Navigator.of(context).push<bool>(
    CupertinoPageRoute<bool>(
      fullscreenDialog: true,
      builder: (BuildContext routeContext) => AppUpdateScreen(
        info: info,
        onUpdate: () => openAppStoreListing(storeUrl: info.storeUrl),
        onDismiss: () => Navigator.of(routeContext).pop(true),
      ),
    ),
  );
}
