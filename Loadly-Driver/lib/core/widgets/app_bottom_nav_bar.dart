import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';

import '../../features/home/presentation/bloc/home_bloc.dart';
import '../../features/home/presentation/bloc/home_event.dart';
import '../../features/home/presentation/bloc/home_state.dart';
import '../../features/home/presentation/widgets/online_offline_button.dart';
import '../../features/notifications/presentation/bloc/notifications_bloc.dart';
import '../../features/notifications/presentation/bloc/notifications_event.dart';
import '../../features/notifications/presentation/bloc/notifications_state.dart';
import '../../l10n/gen/app_localizations.dart';
import '../theme/app_colors.dart';

/// Custom clipper that creates a bar with a circular cutout for the button.
/// The bar covers the button, with the button showing through the cutout.
class _NavBarNotchClipper extends CustomClipper<Path> {
  _NavBarNotchClipper({
    required this.buttonCenterY,
    required this.buttonRadius,
  });

  final double buttonCenterY;
  final double buttonRadius;

  @override
  Path getClip(Size size) {
    const cornerRadius = 20.0;
    final centerX = size.width / 2;

    // Outer bar: full rounded rectangle
    final outerPath = Path()
      ..addRRect(
        RRect.fromRectAndRadius(
          Rect.fromLTWH(0, 0, size.width, size.height),
          const Radius.circular(cornerRadius),
        ),
      );

    // Inner cutout: circle for the button (bar overlaps around it)
    final cutoutRadius = buttonRadius + 4;
    final innerPath = Path()
      ..addOval(
        Rect.fromCircle(
          center: Offset(centerX, buttonCenterY),
          radius: cutoutRadius,
        ),
      );

    return Path.combine(PathOperation.difference, outerPath, innerPath);
  }

  @override
  bool shouldReclip(covariant CustomClipper<Path> oldClipper) =>
      oldClipper is! _NavBarNotchClipper ||
      oldClipper.buttonCenterY != buttonCenterY ||
      oldClipper.buttonRadius != buttonRadius;
}

/// Bottom navigation bar used across the app.
/// Provides navigation between main sections: Profile, Notifications, Trips, and Home.
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
      case '/profile':
        return 0;
      case '/notifications':
        return 1;
      case '/trips':
        return 2;
      case '/home':
        return 3;
      default:
        return 0; // Default to profile
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final isRTL = Directionality.of(context) == TextDirection.rtl;
    final currentIndex = selectedIndex >= 0
        ? selectedIndex
        : getSelectedIndexFromRoute(context);
    final bottomPadding = MediaQuery.of(context).viewPadding.bottom;

    void handleItemTap(int index) {
      if (onItemTapped != null) {
        onItemTapped!(index);
        return;
      }

      // Default navigation behavior
      switch (index) {
        case 0:
          context.go('/profile');
          break;
        case 1:
          context.go('/notifications');
          break;
        case 2:
          context.go('/trips');
          break;
        case 3:
          context.go('/home');
          break;
      }
    }

    final homeBloc = context.read<HomeBloc>();
    final notificationsBloc = context.read<NotificationsBloc>();

    return BlocBuilder<NotificationsBloc, NotificationsState>(
      bloc: notificationsBloc,
      buildWhen: (prev, curr) =>
          prev is! NotificationsLoaded ||
          curr is! NotificationsLoaded ||
          _unreadCount(prev) != _unreadCount(curr),
      builder: (context, notificationsState) {
        if (notificationsState is NotificationsInitial) {
          WidgetsBinding.instance.addPostFrameCallback((_) {
            if (context.mounted) {
              context.read<NotificationsBloc>().add(
                const NotificationsLoad(page: 1, pageSize: 15),
              );
            }
          });
        }
        final unreadCount = _unreadCount(notificationsState);

        return BlocBuilder<HomeBloc, HomeState>(
          bloc: homeBloc,
          builder: (context, homeState) {
            final isOnline = homeState is HomeLoaded && homeState.isOnline;

            const buttonSize = 56.0;
            const buttonRadius = buttonSize / 2;
            // Button bottom: 44 from container bottom; center = (88-44) - 28 = 16 from top
            const buttonCenterY = 16.0;

            return Container(
              padding: EdgeInsets.only(bottom: bottomPadding),
              color: AppColors.lightGray,
              child: SizedBox(
                height: 75,
                child: Stack(
                  clipBehavior: Clip.none,
                  alignment: Alignment.bottomCenter,
                  children: [
                    // Button first (bottom layer) - shows through the bar's cutout
                    Positioned(
                      left: 0,
                      right: 0,
                      bottom: 32,
                      child: Center(
                        child: SizedBox(
                          width: buttonSize,
                          height: buttonSize,
                          child: FittedBox(
                            fit: BoxFit.contain,
                            child: OnlineOfflineButton(
                              isOnline: isOnline,
                              isLoading: false,
                              onToggle: () {
                                homeBloc.add(const HomeToggleOnlineStatus());
                              },
                            ),
                          ),
                        ),
                      ),
                    ),
                    // Bar on top - covers around the button with circular cutout
                    Positioned.fill(
                      child: ClipPath(
                        clipper: _NavBarNotchClipper(
                          buttonCenterY: buttonCenterY,
                          buttonRadius: buttonRadius,
                        ),
                        child: Container(
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
                      ),
                    ),
                    // Nav items
                    SafeArea(
                      top: false,
                      bottom: false,
                      child: Padding(
                        padding: const EdgeInsets.only(bottom: 8),
                        child: Row(
                          textDirection: isRTL
                              ? TextDirection.rtl
                              : TextDirection.ltr,
                          mainAxisAlignment: MainAxisAlignment.spaceAround,
                          children: [
                            _BottomNavItem(
                              label: l10n.profilePage,
                              icon: CupertinoIcons.person_crop_square,
                              isSelected: currentIndex == 0,
                              onTap: () => handleItemTap(0),
                            ),
                            _BottomNavItem(
                              label: l10n.notifications,
                              icon: CupertinoIcons.bell,
                              isSelected: currentIndex == 1,
                              onTap: () => handleItemTap(1),
                              badgeCount: unreadCount,
                            ),
                            const SizedBox(width: buttonSize),
                            _BottomNavItem(
                              label: l10n.tripsNav,
                              icon: CupertinoIcons.speedometer,
                              isSelected: currentIndex == 2,
                              onTap: () => handleItemTap(2),
                            ),
                            _BottomNavItem(
                              label: l10n.homeNav,
                              icon: CupertinoIcons.house_fill,
                              isSelected: currentIndex == 3,
                              onTap: () => handleItemTap(3),
                            ),
                          ],
                        ),
                      ),
                    ),
                  ],
                ),
              ),
            );
          },
        );
      },
    );
  }

  static int _unreadCount(NotificationsState state) {
    if (state is! NotificationsLoaded) return 0;
    return state.notifications.where((n) => !n.isRead).length;
  }
}

class _BottomNavItem extends StatelessWidget {
  const _BottomNavItem({
    required this.label,
    required this.icon,
    required this.isSelected,
    required this.onTap,
    this.badgeCount = 0,
  });

  final String label;
  final IconData icon;
  final bool isSelected;
  final VoidCallback onTap;
  final int badgeCount;

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
                if (badgeCount > 0)
                  Positioned(
                    top: -4,
                    right: -8,
                    child: Container(
                      padding: const EdgeInsets.symmetric(
                        horizontal: 5,
                        vertical: 2,
                      ),
                      constraints: const BoxConstraints(minWidth: 16),
                      decoration: BoxDecoration(
                        color: AppColors.red,
                        borderRadius: BorderRadius.circular(10),
                      ),
                      child: Text(
                        badgeCount > 99 ? '99+' : '$badgeCount',
                        style: const TextStyle(
                          color: AppColors.white,
                          fontSize: 10,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ),
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
