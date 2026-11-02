import 'package:flutter/cupertino.dart';
import 'package:package_info_plus/package_info_plus.dart';

import '../theme/app_colors.dart';
import '../../l10n/gen/app_localizations.dart';

/// Displays the app version (e.g. "Version 1.0.0") using package_info_plus.
/// Use [style] and [textAlign] to customize appearance.
class AppVersionWidget extends StatelessWidget {
  const AppVersionWidget({
    super.key,
    this.style,
    this.textAlign = TextAlign.center,
  });

  final TextStyle? style;
  final TextAlign textAlign;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final defaultStyle = const TextStyle(
      fontSize: 13,
      color: AppColors.mediumBlueGray,
    );

    return FutureBuilder<PackageInfo>(
      future: PackageInfo.fromPlatform(),
      builder: (context, snapshot) {
        final version = snapshot.hasData ? snapshot.data!.version : '—';
        return Text(
          l10n.appVersion(version),
          textAlign: textAlign,
          style: style ?? defaultStyle,
        );
      },
    );
  }
}
