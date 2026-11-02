import 'package:flutter/cupertino.dart';
import 'package:go_router/go_router.dart';

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
      case '/home':
        return 0;
      case '/trips':
        return 1;
      case '/track-trips':
        return 2;
      case '/reports':
        return 3;
      case '/notifications':
        return 4;
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
