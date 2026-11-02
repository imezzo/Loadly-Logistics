import 'package:flutter/cupertino.dart';

import '../../../../l10n/gen/app_localizations.dart';

class PrivacyPolicy extends StatelessWidget {
  const PrivacyPolicy({super.key});

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.only(top: 10, bottom: 30),
      child: Text(
        AppLocalizations.of(context)!.privacyPolicy,
        textAlign: TextAlign.center,
        style: const TextStyle(
          fontSize: 14,
          color: CupertinoColors.systemGrey,
          fontWeight: FontWeight.w400,
        ),
      ),
    );
  }
}
