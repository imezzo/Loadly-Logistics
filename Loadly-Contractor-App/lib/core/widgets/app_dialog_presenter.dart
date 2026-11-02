import 'package:flutter/cupertino.dart';

import '../../l10n/gen/app_localizations.dart';

Future<void> showAppAlertDialog({
  required BuildContext context,
  required String title,
  required String message,
  String? actionText,
}) {
  final l10n = AppLocalizations.of(context)!;
  final displayMessage = message.trim().isEmpty ? l10n.error : message;

  return showCupertinoDialog<void>(
    context: context,
    builder: (_) => CupertinoAlertDialog(
      title: Text(title),
      content: Text(displayMessage),
      actions: [
        CupertinoDialogAction(
          onPressed: () => Navigator.of(context).pop(),
          child: Text(actionText ?? l10n.ok),
        ),
      ],
    ),
  );
}
