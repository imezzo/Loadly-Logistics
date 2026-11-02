import 'package:flutter/cupertino.dart';

import '../theme/app_colors.dart';

/// Primary button using the Main Color (#0F766E) from the design system.
class AppButton extends StatelessWidget {
  const AppButton({
    super.key,
    required this.onPressed,
    this.child,
    this.label,
    this.loading = false,
    this.padding,
    this.minSize = kMinInteractiveDimensionCupertino,
    this.color = AppColors.mainBlue,
  }) : assert(child != null || label != null, 'Provide either child or label');

  final VoidCallback? onPressed;
  final Widget? child;
  final String? label;
  final bool loading;
  final EdgeInsetsGeometry? padding;
  final double minSize;
  final Color color;

  @override
  Widget build(BuildContext context) {
    return CupertinoButton(
      padding:
          padding ?? const EdgeInsets.symmetric(horizontal: 24, vertical: 14),
      minimumSize: Size(minSize, minSize),
      color: color,
      disabledColor: color.withValues(alpha: 0.5),
      onPressed: loading ? null : onPressed,
      child: loading
          ? const CupertinoActivityIndicator(color: AppColors.white)
          : DefaultTextStyle(
              style: const TextStyle(
                color: AppColors.white,
                fontSize: 17,
                fontWeight: FontWeight.w600,
              ),
              child: child ?? Text(label!),
            ),
    );
  }
}
