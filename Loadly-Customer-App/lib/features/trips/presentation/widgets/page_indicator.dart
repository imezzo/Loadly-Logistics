import 'package:flutter/cupertino.dart';
import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';

import '../../../../core/theme/app_colors.dart';

class AnimatedCustomPageIndicator extends StatelessWidget {
  final int currentPage;
  final int totalPages;
  final Color activeColor;
  final Color inactiveColor;
  final Color lineColor;
  final Duration animationDuration;
  final List icons = [
    'assets/images/information.svg',
    'assets/images/trips.svg',
    'assets/images/checkmark_circle.svg',
  ];

  AnimatedCustomPageIndicator({
    super.key,
    required this.currentPage,
    required this.totalPages,
    this.activeColor = AppColors.mainBlue,
    this.inactiveColor = AppColors.mainGray,
    this.lineColor = AppColors.mainBlue,
    this.animationDuration = const Duration(milliseconds: 500),
  });

  @override
  Widget build(BuildContext context) {
    return Row(
      mainAxisAlignment: MainAxisAlignment.center,
      children: List.generate(totalPages, (index) {
        bool isActive = index == currentPage;
        bool isCompleted = index < currentPage;
        bool showLine = index < totalPages - 1;

        return Row(
          children: [
            _buildAnimatedIndicator(index, isActive, isCompleted),
            if (showLine) _buildAnimatedLine(isCompleted, context),
          ],
        );
      }),
    );
  }

  Widget _buildAnimatedIndicator(int index, bool isActive, bool isCompleted) {
    return AnimatedContainer(
      duration: animationDuration,
      width: 40,
      height: 40,
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        color: AppColors.white,
        border: Border.all(
          color: isActive || isCompleted ? activeColor : inactiveColor,
          width: 4,
        ),
      ),
      child: Center(
        child: AnimatedSwitcher(
          duration: animationDuration,
          // child: Text(
          //         '${index + 1}',
          //         key: ValueKey('number_$index'),
          //         style: TextStyle(
          //           color: isActive ? Colors.white : inactiveColor,
          //           fontSize: 14,
          //           fontWeight: FontWeight.w600,
          //         ),
          //       ),
          child: SvgPicture.asset(
            icons[index],
            fit: BoxFit.cover,
            width: 18,
            height: 18,
            colorFilter: ColorFilter.mode(
              isActive ? activeColor : AppColors.darkGray,
              BlendMode.srcIn,
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildAnimatedLine(bool isCompleted, BuildContext context) {
    return AnimatedContainer(
      duration: animationDuration,
      width: MediaQuery.sizeOf(context).width * 0.25,
      height: 4,
      color: isCompleted ? activeColor : inactiveColor,
    );
  }
}
