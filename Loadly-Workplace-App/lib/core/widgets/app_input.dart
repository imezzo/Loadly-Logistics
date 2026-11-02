import 'package:flutter/cupertino.dart';
import 'package:flutter/services.dart';

import '../theme/app_colors.dart';

/// Reusable text input following the app design system.
class AppInput extends StatelessWidget {
  const AppInput({
    super.key,
    this.controller,
    this.label,
    this.placeholder,
    this.obscureText = false,
    this.keyboardType,
    this.textInputAction,
    this.prefix,
    this.suffix,
    this.onSubmitted,
    this.onChanged,
    this.enabled = true,
    this.autofocus = false,
    this.inputFormatters,
    this.maxLength,
    this.textDirection,
  });

  final TextEditingController? controller;
  final String? label;
  final String? placeholder;
  final bool obscureText;
  final TextInputType? keyboardType;
  final TextInputAction? textInputAction;
  final Widget? prefix;
  final Widget? suffix;
  final void Function(String)? onSubmitted;
  final void Function(String)? onChanged;
  final bool enabled;
  final bool autofocus;
  final List<TextInputFormatter>? inputFormatters;
  final int? maxLength;
  final TextDirection? textDirection;

  @override
  Widget build(BuildContext context) {
    final input = CupertinoTextField(
      controller: controller,
      placeholder: placeholder,
      obscureText: obscureText,
      keyboardType: keyboardType,
      textInputAction: textInputAction,
      textDirection: textDirection,
      onSubmitted: onSubmitted,
      onChanged: onChanged,
      enabled: enabled,
      autofocus: autofocus,
      inputFormatters: inputFormatters,
      maxLength: maxLength,
      prefix: prefix != null
          ? Padding(
              padding: const EdgeInsetsDirectional.only(start: 12),
              child: prefix,
            )
          : null,
      suffix: suffix,
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(10),
        border: Border.all(color: AppColors.lightGray),
      ),
    );

    if (label != null) {
      return Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        mainAxisSize: MainAxisSize.min,
        children: [
          Align(
            alignment: AlignmentDirectional.centerStart,
            child: Text(
              label!,
              textAlign: TextAlign.start,
              style: const TextStyle(
                color: AppColors.darkGray,
                fontSize: 15,
                fontWeight: FontWeight.w500,
              ),
            ),
          ),
          const SizedBox(height: 8),
          input,
        ],
      );
    }

    return input;
  }
}
