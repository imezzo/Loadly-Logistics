import 'package:flutter/cupertino.dart';

import '../../l10n/gen/app_localizations.dart';
import '../theme/app_colors.dart';

class AppSecondaryButton extends StatelessWidget {
  final String? label;
  final Widget? child;
  final bool isLoading;
  final VoidCallback onPressed;
  final Color? color;
  final EdgeInsetsGeometry? padding;
  const AppSecondaryButton({
    super.key,
    this.isLoading = false,
    required this.onPressed,
    this.label,
    this.child,
    this.color,
    this.padding,
  });

  @override
  Widget build(BuildContext context) {
    return Container(
      decoration: BoxDecoration(
        border: Border.all(
          color: color ?? AppColors.mediumBlueGray,
          width: 0.5,
        ),
        borderRadius: BorderRadius.circular(50),
      ),
      child: CupertinoButton(
        padding:
            padding ?? const EdgeInsets.symmetric(horizontal: 24, vertical: 14),
        borderRadius: BorderRadius.circular(50),
        color: AppColors.white,
        disabledColor: color ?? AppColors.white.withValues(alpha: 0.5),
        onPressed: isLoading ? null : onPressed,
        child: isLoading
            ? const CupertinoActivityIndicator(color: AppColors.white)
            : DefaultTextStyle(
                style: TextStyle(
                  color: color ?? AppColors.darkGray,
                  fontSize: 16,
                  fontWeight: FontWeight.w500,
                ),
                child:
                    child ??
                    Text(
                      label != null
                          ? label!
                          : AppLocalizations.of(context)!.back,
                    ),
              ),
      ),
    );
  }
}
