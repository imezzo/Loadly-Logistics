import 'package:flutter/cupertino.dart';

import '../../l10n/gen/app_localizations.dart';

/// In-app disclosure shown immediately before a runtime permission request
/// (Google Play User Data policy — prominent disclosure and consent).
Future<bool> showProminentUserDataDisclosure(
  BuildContext context, {
  required String title,
  required String body,
}) async {
  final l10n = AppLocalizations.of(context)!;
  final result = await showCupertinoDialog<bool>(
    context: context,
    barrierDismissible: false,
    builder: (BuildContext ctx) => CupertinoAlertDialog(
      title: Text(title),
      content: Padding(
        padding: const EdgeInsets.only(top: 8),
        child: Text(body),
      ),
      actions: <CupertinoDialogAction>[
        CupertinoDialogAction(
          onPressed: () => Navigator.pop(ctx, false),
          child: Text(l10n.prominentDisclosureNotNow),
        ),
        CupertinoDialogAction(
          isDefaultAction: true,
          onPressed: () => Navigator.pop(ctx, true),
          child: Text(l10n.prominentDisclosureContinue),
        ),
      ],
    ),
  );
  return result == true;
}
