import 'package:flutter/cupertino.dart';
import 'package:flutter_svg/flutter_svg.dart';

import '../theme/app_colors.dart';

const _bgAsset = 'assets/images/bg_blue.png';

/// Full-screen background for the app using the blue design image.
/// Use as the bottom layer of a [Stack] so content appears on top.
class AppBackground extends StatelessWidget {
  const AppBackground({super.key});

  @override
  Widget build(BuildContext context) {
    return Positioned.fill(
      child: ColoredBox(
        color: AppColors.mainBlue,
        child: SvgPicture.asset(
          _bgAsset,
          fit: BoxFit.cover,
          width: double.infinity,
          height: double.infinity,
        ),
      ),
    );
  }
}
