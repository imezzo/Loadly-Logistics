import 'package:flutter/cupertino.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:go_router/go_router.dart';

import '../../l10n/gen/app_localizations.dart';
import '../router/navigation_state.dart';
import '../router/route_constants.dart';
import '../theme/app_colors.dart';

/// Bottom navigation bar used across the app.
/// Provides navigation between main sections: Home, Trips, Real-time, and Notifications.
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
      case AppRoutePaths.home:
        return 0;
      case AppRoutePaths.trips:
        return 1;
      case AppRoutePaths.market:
        return 2;
      case AppRoutePaths.settings:
        return 3;
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
      NavigationState.slideFromRight = index > currentIndex;
      // Default navigation behavior
      switch (index) {
        case 0:
          context.go(AppRoutePaths.home);
          break;
        case 1:
          context.go(AppRoutePaths.trips);
          break;
        case 2:
          context.go(AppRoutePaths.market);
          break;
        case 3:
          context.go(AppRoutePaths.settings);
          break;
      }
    }

    return Container(
      padding: EdgeInsets.only(bottom: bottomPadding),
      child: SizedBox(
        height: 70,
        child: Container(
          decoration: const BoxDecoration(
            color: AppColors.white,
            // borderRadius: BorderRadius.circular(10),
            boxShadow: [
              BoxShadow(
                color: AppColors.lightGray,
                blurRadius: 4,
                offset: Offset(0, -2),
              ),
            ],
          ),
          child: SafeArea(
            top: false,
            bottom: false,
            child: Row(
              textDirection: isRTL ? TextDirection.rtl : TextDirection.ltr,
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              mainAxisSize: MainAxisSize.min,
              children: [
                _BottomNavItem(
                  label: l10n.homeNav,
                  icon: 'assets/images/home.svg',
                  isSelected: currentIndex == 0,
                  onTap: () => handleItemTap(0),
                ),
                _BottomNavItem(
                  label: l10n.trips,
                  icon: 'assets/images/trips.svg',
                  isSelected: currentIndex == 1,
                  onTap: () => handleItemTap(1),
                ),
                _BottomNavItem(
                  label: l10n.market,
                  icon: 'assets/images/market.svg',
                  isSelected: currentIndex == 2,
                  onTap: () => handleItemTap(2),
                ),
                _BottomNavItem(
                  label: l10n.account,
                  icon: 'assets/images/account.svg',
                  isSelected: currentIndex == 3,
                  onTap: () => handleItemTap(3),
                ),
              ],
            ),
          ),
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
  final String icon;
  final bool isSelected;
  final VoidCallback onTap;

  @override
  Widget build(BuildContext context) {
    return Expanded(
      child: Container(
        padding: const EdgeInsets.symmetric(vertical: 10),
        decoration: isSelected
            ? const BoxDecoration(color: AppColors.mainGray)
            : null,
        child: CupertinoButton(
          padding: EdgeInsets.zero,
          onPressed: onTap,
          child: Column(
            mainAxisSize: MainAxisSize.min,
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              SvgPicture.asset(
                icon,
                width: 20,
                height: 20,
                colorFilter: ColorFilter.mode(
                  isSelected ? AppColors.mainBlue : AppColors.darkGray,
                  BlendMode.srcIn,
                ),
              ),
              const SizedBox(height: 5),
              Flexible(
                child: FittedBox(
                  child: Text(
                    label,
                    style: TextStyle(
                      fontSize: 12,
                      color: isSelected
                          ? AppColors.mainBlue
                          : AppColors.darkGray,
                      fontWeight: isSelected
                          ? FontWeight.w600
                          : FontWeight.w400,
                    ),
                  ),
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
