import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../widgets/auth_navbar.dart';
import '../widgets/privacy_policy.dart';

class OnboardingScreen extends StatelessWidget {
  const OnboardingScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return CupertinoPageScaffold(
      backgroundColor: AppColors.white,
      navigationBar: const AuthNavbar(),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          const Divider(thickness: 0.5),
          Expanded(
            child: SingleChildScrollView(
              padding: const EdgeInsets.symmetric(
                horizontal: 15,
              ).copyWith(top: 20),
              child: ConstrainedBox(
                constraints: BoxConstraints(
                  minHeight:
                      MediaQuery.of(context).size.height -
                      MediaQuery.of(context).padding.top -
                      MediaQuery.of(context).padding.bottom -
                      80,
                ),
                child: IntrinsicHeight(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        l10n.appTitle,
                        textAlign: TextAlign.start,
                        style: const TextStyle(
                          fontSize: 18,
                          fontWeight: FontWeight.w500,
                          color: CupertinoColors.black,
                        ),
                      ),
                      const SizedBox(height: 10),
                      Text(
                        l10n.onboardingDesc,
                        textAlign: TextAlign.start,
                        style: const TextStyle(
                          fontSize: 14,
                          color: CupertinoColors.systemGrey,
                          fontWeight: FontWeight.w400,
                        ),
                      ),
                      const SizedBox(height: 40),
                      OnboardingListTile(
                        icon: 'assets/images/package_box.svg',
                        title: l10n.onboardingItem1,
                        subTitle: l10n.onboardingItem1Desc,
                      ),
                      const Divider(thickness: 0.5),
                      OnboardingListTile(
                        icon: 'assets/images/spark.svg',
                        title: l10n.onboardingItem2,
                        subTitle: l10n.onboardingItem2Desc,
                      ),
                      const Divider(thickness: 0.5),
                      OnboardingListTile(
                        icon: 'assets/images/shield.svg',
                        title: l10n.onboardingItem3,
                        subTitle: l10n.onboardingItem3Desc,
                      ),
                      const Spacer(),
                      SizedBox(
                        width: double.infinity,
                        child: AppPrimaryButton(
                          label: l10n.continuee,
                          onPressed: () => context.go('/login'),
                        ),
                      ),
                      const SizedBox(height: 10),
                      const PrivacyPolicy(),
                    ],
                  ),
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}

class OnboardingListTile extends StatelessWidget {
  const OnboardingListTile({
    super.key,
    required this.icon,
    required this.title,
    required this.subTitle,
  });

  final String icon;
  final String title;
  final String subTitle;

  @override
  Widget build(BuildContext context) {
    return CupertinoListTile(
      padding: const EdgeInsets.symmetric(vertical: 10),
      leading: CircleAvatar(
        backgroundColor: const Color(0xF6F6F6F6),
        child: SvgPicture.asset(icon),
      ),
      title: Text(
        title,
        style: const TextStyle(
          color: CupertinoColors.black,
          fontSize: 14,
          fontWeight: FontWeight.w500,
        ),
      ),
      subtitle: Text(
        subTitle,
        maxLines: 5,
        style: const TextStyle(fontSize: 12, fontWeight: FontWeight.w400),
      ),
    );
  }
}
