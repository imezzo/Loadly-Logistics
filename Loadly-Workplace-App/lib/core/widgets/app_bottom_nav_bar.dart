import 'package:flutter/cupertino.dart';
import 'package:go_router/go_router.dart';

import '../../l10n/gen/app_localizations.dart';
import '../theme/app_colors.dart';

/// Bottom navigation bar used across the app.
/// Provides navigation between main sections: Profile, , Trips, and Home.
class AppBottomNavBar extends StatelessWidget {
  const AppBottomNavBar({
    super.key,
    this.selectedIndex = -1,
    this.onItemTapped,
  });

  /// The currently selected tab index (0-3).
  /// If negative, will be auto-detected from current route.
  final int selectedIndex;

  /// Optional callback when an item is tapped.
  /// If not provided, default navigation will be used.
  final ValueChanged<int>? onItemTapped;

  /// Get the selected index based on the current route
  static int getSelectedIndexFromRoute(BuildContext context) {
    final location = GoRouterState.of(context).uri.path;
    switch (location) {
      case '/home':
        return 0;
      case '/profile':
        return 1;
      default:
        return 0; // Default to home
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final isRTL = Directionality.of(context) == TextDirection.rtl;
    final bottomPadding = MediaQuery.of(context).viewPadding.bottom;
    // final currentIndex = selectedIndex >= 0 ? selectedIndex : getSelectedIndexFromRoute(context);
    final currentIndex = getSelectedIndexFromRoute(context);

    void handleItemTap(int index) {
      if (onItemTapped != null) {
        onItemTapped!(index);
        return;
      }

      // Default navigation behavior
      switch (index) {
        case 0:
          context.go('/home');
          break;
        case 1:
          context.go('/profile');
          break;
      }
    }

    return Container(
      padding: EdgeInsets.only(bottom: bottomPadding),
      color: AppColors.lightGray,
      child: SizedBox(
        height: 60,
        child: Stack(
          clipBehavior: Clip.none,
          alignment: Alignment.bottomCenter,
          children: [
            // Bar on top - covers around the button with circular cutout
            Container(
              decoration: const BoxDecoration(
                color: AppColors.lightGray,
                boxShadow: [
                  BoxShadow(
                    color: AppColors.lightGray,
                    blurRadius: 4,
                    offset: Offset(0, -2),
                  ),
                ],
              ),
            ),
            // Nav items
            SafeArea(
              top: false,
              bottom: false,
              child: Padding(
                padding: const EdgeInsets.only(bottom: 8),
                child: Row(
                  textDirection: isRTL ? TextDirection.rtl : TextDirection.ltr,
                  mainAxisAlignment: MainAxisAlignment.spaceAround,
                  children: [
                    _BottomNavItem(
                      label: l10n.homeNav,
                      icon: CupertinoIcons.house_fill,
                      isSelected: currentIndex == 0,
                      onTap: () => handleItemTap(0),
                    ),
                    // _BottomNavItem(
                    //   label: l10n.tripsNav,
                    //   icon: CupertinoIcons.speedometer,
                    //   isSelected: currentIndex == 1,
                    //   onTap: () => handleItemTap(1),
                    // ),
                    _BottomNavItem(
                      label: l10n.profilePage,
                      icon: CupertinoIcons.person_crop_square,
                      isSelected: currentIndex == 1,
                      onTap: () => handleItemTap(1),
                    ),
                  ],
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }
}

class _BottomNavItem extends StatelessWidget {
  const _BottomNavItem({
    required this.label,
    required this.icon,
    required this.isSelected,
    required this.onTap,
  });

  final String label;
  final IconData icon;
  final bool isSelected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: CupertinoButton(
        padding: EdgeInsets.zero,
        onPressed: onTap,
        child: Column(
          mainAxisSize: MainAxisSize.min,
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            Stack(
              clipBehavior: Clip.none,
              children: [
                Icon(
                  icon,
                  color: isSelected ? AppColors.mainBlue : AppColors.darkGray,
                  size: 22,
                ),
              ],
            ),
            const SizedBox(height: 2),
            Text(
              label,
              style: TextStyle(
                fontSize: 11,
                color: isSelected ? AppColors.mainBlue : AppColors.darkGray,
                fontWeight: isSelected ? FontWeight.w600 : FontWeight.normal,
              ),
            ),
            if (isSelected) ...[
              const SizedBox(height: 2),
              Container(
                width: 28,
                height: 2,
                decoration: BoxDecoration(
                  color: AppColors.mainBlue,
                  borderRadius: BorderRadius.circular(1),
                ),
              ),
            ],
          ],
        ),
      ),
    );
  }
}
