import 'package:flutter/cupertino.dart';
import 'package:flutter_svg/svg.dart';
import 'package:go_router/go_router.dart';

import '../../l10n/gen/app_localizations.dart';
import '../theme/app_colors.dart';

class AppNavBar extends StatelessWidget
    implements ObstructingPreferredSizeWidget {
  final Widget? trailng;
  const AppNavBar({super.key, this.trailng});

  @override
  Widget build(BuildContext context) {
    return CupertinoNavigationBar(
      backgroundColor: AppColors.mainGray,
      enableBackgroundFilterBlur: false,
      leading: Row(
        mainAxisAlignment: MainAxisAlignment.start,
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
      trailing: Row(
        mainAxisAlignment: MainAxisAlignment.end,
        children: [
          ?trailng,
          // NavbarButton(icon: 'assets/images/search.svg', onPress: () {}),
          NavbarButton(
            icon: 'assets/images/setting.svg',
            onPress: () => context.go('/settings'),
          ),
        ],
      ),
    );
  }

  @override
  Size get preferredSize => const Size.fromHeight(45);

  @override
  bool shouldFullyObstruct(BuildContext context) {
    return true;
  }
}

class NavbarButton extends StatelessWidget {
  final String icon;
  final VoidCallback onPress;
  const NavbarButton({super.key, required this.icon, required this.onPress});

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.symmetric(vertical: 3, horizontal: 1),
      decoration: const BoxDecoration(
        shape: BoxShape.circle,
        color: AppColors.white,
      ),
      child: CupertinoButton(
        padding: EdgeInsets.zero,
        onPressed: onPress,
        child: SvgPicture.asset(icon, width: 20, height: 20, fit: BoxFit.cover),
      ),
    );
  }
}
