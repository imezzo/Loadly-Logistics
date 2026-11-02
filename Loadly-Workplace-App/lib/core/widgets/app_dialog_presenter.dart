import 'package:flutter/cupertino.dart';

Future<void> showAppAlertDialog({
  required BuildContext context,
  required String title,
  required String message,
  String actionText = 'OK',
}) {
  return showCupertinoDialog<void>(
    context: context,
    builder: (_) => CupertinoAlertDialog(
      title: Text(title),
      content: Text(message),
      actions: [
        CupertinoDialogAction(
          onPressed: () => Navigator.of(context).pop(),
          child: Text(actionText),
        ),
      ],
    ),
  );
}
