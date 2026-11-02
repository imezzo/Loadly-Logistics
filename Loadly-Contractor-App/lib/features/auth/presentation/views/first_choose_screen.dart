import 'package:flutter/cupertino.dart';
import 'package:go_router/go_router.dart';
import '../../../../core/router/route_constants.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/app_version.dart';
import '../../../../core/widgets/language_switcher.dart';
import '../../../../l10n/gen/app_localizations.dart';

class FirstChooseScreen extends StatelessWidget {
  const FirstChooseScreen({super.key});

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return CupertinoPageScaffold(
      child: Container(
        padding: const EdgeInsets.symmetric(horizontal: 24),
        color: AppColors.white,

        child: Column(
          children: [
            const SizedBox(height: 48),
            Image.asset(
              'assets/images/loadly_logo.png',
              width: 100,
              height: 100,
              fit: BoxFit.contain,
            ),
            const SizedBox(height: 48),
            Text(
              l10n.welcomeToLoadly,
              style: CupertinoTheme.of(context).textTheme.navLargeTitleTextStyle
                  .copyWith(
                    fontSize: 18,
                    color: AppColors.darkGray,
                    fontWeight: FontWeight.w400,
                  ),
            ),
            const SizedBox(height: 12),
            Text(
              l10n.welcomeDesc,
              style: CupertinoTheme.of(context).textTheme.textStyle.copyWith(
                color: AppColors.mediumBlueGray,
                fontSize: 14,
                height: 1.4,
                fontWeight: FontWeight.w400,
              ),
            ),
            const Spacer(),
            SizedBox(
              width: double.infinity,
              child: AppButton(
                label: l10n.signIn,
                onPressed: () => context.go(AppRoutePaths.login),
              ),
            ),
            const SizedBox(height: 16),
            GestureDetector(
              onTap: () => context.go(AppRoutePaths.register),
              child: Container(
                width: double.infinity,
                padding: const EdgeInsets.symmetric(
                  horizontal: 24,
                  vertical: 14,
                ),
                decoration: BoxDecoration(
                  color: CupertinoColors.transparent,
                  border: Border.all(color: AppColors.mainBlue, width: 1.5),
                  borderRadius: BorderRadius.circular(10),
                ),
                child: Center(
                  child: Text(
                    l10n.createNewAccount,
                    style: const TextStyle(
                      color: AppColors.mainBlue,
                      fontSize: 17,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                ),
              ),
            ),
            const Spacer(),
            const LanguageSwitcher(),
            const SizedBox(height: 5),
            const Center(child: AppVersionWidget()),
            const SizedBox(height: 24),
          ],
        ),
      ),
    );
  }
}
