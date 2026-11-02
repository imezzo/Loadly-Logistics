import 'package:flutter/cupertino.dart';
import 'package:go_router/go_router.dart';

import '../router/route_constants.dart';
import 'app_bottom_nav_bar.dart';

/// Main scaffold wrapper that includes the bottom navigation bar.
/// Use this for all main app screens (home, profile, trips, notifications).
class MainScaffold extends StatelessWidget {
  const MainScaffold({super.key, required this.child, this.selectedIndex});

  /// The content to display above the navigation bar
  final Widget child;

  /// Optional selected index. If not provided, will be determined from current route.
  final int? selectedIndex;

  /// Get the selected index based on the current route
  static int getSelectedIndex(BuildContext context) {
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
    final currentIndex = selectedIndex ?? getSelectedIndex(context);

    return Column(
      children: [
        Expanded(child: child),
        AppBottomNavBar(selectedIndex: currentIndex),
      ],
    );
  }
}

/// Cupertino page scaffold for main tabs with a fixed bottom navigation bar.
class AppMainPageScaffold extends StatelessWidget {
  const AppMainPageScaffold({
    super.key,
    this.navigationBar,
    required this.child,
    this.selectedIndex,
    this.backgroundColor = CupertinoColors.systemBackground,
  });

  final ObstructingPreferredSizeWidget? navigationBar;
  final Widget child;
  final int? selectedIndex;
  final Color backgroundColor;

  @override
  Widget build(BuildContext context) {
    return CupertinoPageScaffold(
      backgroundColor: backgroundColor,
      resizeToAvoidBottomInset: false,
      navigationBar: navigationBar,
      child: MainScaffold(
        selectedIndex: selectedIndex,
        child: child,
      ),
    );
  }
}
