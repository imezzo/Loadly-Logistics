import 'package:flutter/cupertino.dart';

import '../../l10n/gen/app_localizations.dart';
import '../models/app_update_info.dart';
import '../theme/app_colors.dart';
import 'app_button.dart';

const _logoAsset = 'assets/images/loadly_logo.png';

/// Full-screen update prompt.
///
/// For [AppUpdateInfo.isForced], only the update action is shown and back
/// navigation is blocked.
class AppUpdateScreen extends StatelessWidget {
  const AppUpdateScreen({
    super.key,
    required this.info,
    required this.onUpdate,
    this.onDismiss,
  });

  final AppUpdateInfo info;
  final VoidCallback onUpdate;
  final VoidCallback? onDismiss;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final title =
        info.title ??
        (info.isForced
            ? l10n.appUpdateRequiredTitle
            : l10n.appUpdateAvailableTitle);
    final message =
        info.message ??
        (info.isForced
            ? l10n.appUpdateRequiredMessage
            : l10n.appUpdateAvailableMessage);

    return PopScope(
      canPop: !info.isForced,
      child: CupertinoPageScaffold(
        backgroundColor: AppColors.white,
        child: SafeArea(
          child: Padding(
            padding: const EdgeInsets.symmetric(horizontal: 24),
            child: Column(
              children: [
                const Spacer(flex: 2),
                Image.asset(
                  _logoAsset,
                  width: 120,
                  height: 144,
                  fit: BoxFit.contain,
                ),
                const SizedBox(height: 32),
                Icon(
                  info.isForced
                      ? CupertinoIcons.exclamationmark_circle_fill
                      : CupertinoIcons.arrow_down_circle_fill,
                  size: 56,
                  color: info.isForced ? AppColors.red : AppColors.mainBlue,
                ),
                const SizedBox(height: 20),
                Text(
                  title,
                  textAlign: TextAlign.center,
                  style: const TextStyle(
                    fontSize: 24,
                    fontWeight: FontWeight.w700,
                    color: AppColors.darkGray,
                  ),
                ),
                const SizedBox(height: 12),
                Text(
                  message,
                  textAlign: TextAlign.center,
                  style: const TextStyle(
                    fontSize: 16,
                    height: 1.45,
                    color: AppColors.darkGray,
                  ),
                ),
                if (info.latestVersion != null &&
                    info.latestVersion!.isNotEmpty) ...[
                  const SizedBox(height: 12),
                  Text(
                    l10n.appUpdateLatestVersion(info.latestVersion!),
                    textAlign: TextAlign.center,
                    style: const TextStyle(
                      fontSize: 15,
                      fontWeight: FontWeight.w600,
                      color: AppColors.mainBlue,
                    ),
                  ),
                ],
                const Spacer(flex: 3),
                AppButton(label: l10n.appUpdateNow, onPressed: onUpdate),
                if (!info.isForced && onDismiss != null) ...[
                  const SizedBox(height: 12),
                  CupertinoButton(
                    padding: const EdgeInsets.symmetric(vertical: 14),
                    onPressed: onDismiss,
                    child: Text(
                      l10n.appUpdateLater,
                      style: const TextStyle(
                        color: AppColors.mediumBlueGray,
                        fontSize: 17,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                  ),
                ],
                const SizedBox(height: 24),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
