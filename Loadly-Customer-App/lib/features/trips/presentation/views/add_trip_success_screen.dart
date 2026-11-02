import 'package:flutter/cupertino.dart';
import 'package:flutter_svg/svg.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_nav_bar.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../l10n/gen/app_localizations.dart';

class AddTripSuccessScreen extends StatelessWidget {
  const AddTripSuccessScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return CupertinoPageScaffold(
      backgroundColor: AppColors.mainGray.withValues(alpha: 1.0),
      navigationBar: const AppNavBar(),
      child: Padding(
        padding: const EdgeInsets.symmetric(vertical: 5),
        child: ClipRRect(
          borderRadius: AppLayout.authContentBorderRadius,
          child: Container(
            padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 15),
            color: AppColors.white,
            child: Column(
              mainAxisSize: MainAxisSize.min,
              children: [
                Expanded(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      SvgPicture.asset('assets/images/auth_success.svg'),
                      Text(
                        l10n.tripSuccessfullyRequest,
                        textAlign: TextAlign.center,
                        style: const TextStyle(
                          fontSize: 24,
                          fontWeight: FontWeight.w500,
                          color: AppColors.darkGray,
                        ),
                      ),
                      const SizedBox(height: 10),
                      Text(
                        l10n.tripSuccessfullyRequestDesc,
                        textAlign: TextAlign.center,
                        style: const TextStyle(
                          fontSize: 14,
                          color: CupertinoColors.systemGrey,
                          fontWeight: FontWeight.w400,
                        ),
                      ),
                    ],
                  ),
                ),
                SizedBox(
                  width: double.infinity,
                  child: AppPrimaryButton(
                    onPressed: () {
                      context.go('/trips');
                    },
                    label: l10n.continuee,
                  ),
                ),
              ],
            ),
          ),
        ),
      ),
    );
  }
}
