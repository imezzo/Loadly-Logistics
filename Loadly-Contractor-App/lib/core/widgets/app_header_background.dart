import 'package:flutter/material.dart';

import '../theme/app_colors.dart';
import 'app_blue_pattern_background.dart';

/// Blue header background with geometric pattern and a fixed height.
/// Place behind content in a [Stack] inside a scroll view.
class AppHeaderBackground extends StatelessWidget {
  const AppHeaderBackground({
    super.key,
    required this.height,
    this.color = AppColors.mainBlue,
  });

  final double height;
  final Color color;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      height: height,
      width: double.infinity,
      child: AppBluePatternFill(color: color),
    );
  }
}
