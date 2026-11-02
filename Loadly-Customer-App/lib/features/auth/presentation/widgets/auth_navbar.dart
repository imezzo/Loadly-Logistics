import 'package:flutter/cupertino.dart';
import 'package:flutter_svg/svg.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/language_switcher.dart';
import '../../../../l10n/gen/app_localizations.dart';

class AuthNavbar extends StatelessWidget
    implements ObstructingPreferredSizeWidget {
  const AuthNavbar({super.key});

  @override
  Widget build(BuildContext context) {
    return CupertinoNavigationBar(
      leading: Row(
        children: [
          SvgPicture.asset(
            'assets/images/loadly_logo.svg',
            width: 50,
            height: 35,
            fit: BoxFit.contain,
          ),
          const SizedBox(width: 5),
          Text(
            AppLocalizations.of(context)!.appTitle,
            style: CupertinoTheme.of(
              context,
            ).textTheme.navTitleTextStyle.copyWith(color: AppColors.darkGray),
          ),
        ],
      ),
      trailing: const LanguageSwitcher(),
    );
  }

  @override
  Size get preferredSize => const Size.fromHeight(45);

  @override
  bool shouldFullyObstruct(BuildContext context) {
    return true;
  }
}
