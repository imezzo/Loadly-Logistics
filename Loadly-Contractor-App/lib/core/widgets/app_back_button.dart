import 'package:flutter/cupertino.dart';

import '../theme/app_colors.dart';

/// Shared circular back button. Arrow points to the end in RTL and start in LTR.
class AppBackButton extends StatelessWidget {
  const AppBackButton({super.key, required this.onPressed});

  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) {
    final isRtl = Directionality.of(context) == TextDirection.rtl;

    return CupertinoButton(
      padding: EdgeInsets.zero,
      minimumSize: Size.zero,
      onPressed: onPressed,
      child: Container(
        width: 30,
        height: 30,
        decoration: const BoxDecoration(
          color: AppColors.white,
          shape: BoxShape.circle,
        ),
        child: Icon(
          isRtl ? CupertinoIcons.chevron_forward : CupertinoIcons.chevron_back,
          size: 18,
          color: AppColors.mainBlue,
        ),
      ),
    );
  }
}
