import 'package:flutter/cupertino.dart';
import 'package:flutter/services.dart';

import '../theme/app_colors.dart';

/// Reusable phone input widget with country code prefix (+2 🇪🇬).
/// Matches the design from login screen.
class AppPhoneInput extends StatelessWidget {
  const AppPhoneInput({
    super.key,
    required this.controller,
    this.label,
    this.placeholder,
    this.onChanged,
    this.onSubmitted,
    this.textInputAction,
    this.error,
    this.enabled = true,
    this.autofocus = false,
    this.trailing,
  });

  final TextEditingController controller;
  final String? label;
  final String? placeholder;
  final void Function(String)? onChanged;
  final void Function(String)? onSubmitted;
  final TextInputAction? textInputAction;
  final String? error;
  final bool enabled;
  final bool autofocus;

  /// Optional widget to show beside the input (e.g. biometric button).
  final Widget? trailing;

  @override
  Widget build(BuildContext context) {
    final phoneInput = Container(
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(10),
        border: Border.all(
          color: error != null
              ? CupertinoColors.systemRed
              : AppColors.lightGray,
        ),
      ),
      child: Row(
        textDirection: TextDirection.ltr,
        children: [
          Directionality(
            textDirection: TextDirection.ltr,
            child: Padding(
              padding: const EdgeInsets.only(left: 12),
              child: Row(
                mainAxisSize: MainAxisSize.min,
                children: [
                  const Text(
                    '🇪🇬',
                    style: TextStyle(fontSize: 20, height: 1.2),
                  ),
                  const SizedBox(width: 6),
                  const Text(
                    '+2',
                    style: TextStyle(
                      fontSize: 16,
                      color: AppColors.mediumBlueGray,
                      fontWeight: FontWeight.w600,
                    ),
                  ),
                  const SizedBox(width: 10),
                  Container(width: 1, height: 20, color: AppColors.lightGray),
                ],
              ),
            ),
          ),
          Expanded(
            child: CupertinoTextField(
              controller: controller,
              placeholder: placeholder,
              keyboardType: TextInputType.phone,
              textInputAction: textInputAction ?? TextInputAction.done,
              onChanged: onChanged,
              onSubmitted: onSubmitted,
              enabled: enabled,
              autofocus: autofocus,
              padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
              decoration: null,
              inputFormatters: [
                FilteringTextInputFormatter.digitsOnly,
                LengthLimitingTextInputFormatter(11),
              ],
              maxLength: 11,
            ),
          ),
        ],
      ),
    );

    final inputWidget = trailing != null
        ? Row(
            crossAxisAlignment: CrossAxisAlignment.center,
            children: [
              Expanded(child: phoneInput),
              trailing!,
            ],
          )
        : phoneInput;

    if (label != null || error != null) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        mainAxisSize: MainAxisSize.min,
        children: [
          if (label != null) ...[
            Align(
              alignment: AlignmentDirectional.centerStart,
              child: Text(
                label!,
                style: const TextStyle(
                  color: AppColors.darkGray,
                  fontSize: 15,
                  fontWeight: FontWeight.w500,
                ),
              ),
            ),
            const SizedBox(height: 8),
          ],
          inputWidget,
          if (error != null) ...[
            const SizedBox(height: 8),
            Align(
              alignment: AlignmentDirectional.centerStart,
              child: Text(
                error!,
                style: const TextStyle(
                  color: CupertinoColors.systemRed,
                  fontSize: 13,
                ),
              ),
            ),
          ],
        ],
      );
    }

    return inputWidget;
  }
}
