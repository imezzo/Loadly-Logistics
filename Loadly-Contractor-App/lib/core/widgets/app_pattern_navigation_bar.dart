import 'package:flutter/cupertino.dart';

import 'app_blue_pattern_background.dart';

/// Navigation bar with blue patterned background (back + title screens).
class AppPatternNavigationBar extends StatelessWidget
    implements ObstructingPreferredSizeWidget {
  const AppPatternNavigationBar({
    super.key,
    this.leading,
    this.middle,
    this.trailing,
  });

  final Widget? leading;
  final Widget? middle;
  final Widget? trailing;

  static const _contentHeight = 44.0;

  @override
  Widget build(BuildContext context) {
    final topPadding = MediaQuery.paddingOf(context).top;

    return SizedBox(
      height: topPadding + _contentHeight,
      child: AppBluePatternBackground(
        child: Padding(
          padding: EdgeInsets.only(top: topPadding),
          child: SizedBox(
            height: _contentHeight,
            child: NavigationToolbar(
              leading: leading,
              middle: middle,
              trailing: trailing,
              centerMiddle: true,
            ),
          ),
        ),
      ),
    );
  }

  @override
  Size get preferredSize => const Size.fromHeight(_contentHeight);

  @override
  bool shouldFullyObstruct(BuildContext context) => true;
}
