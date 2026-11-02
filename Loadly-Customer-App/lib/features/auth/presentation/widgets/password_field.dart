import 'package:flutter/cupertino.dart';
import 'package:flutter_svg/svg.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../l10n/gen/app_localizations.dart';

// ignore: must_be_immutable
class PasswordField extends StatefulWidget {
  String? error;
  final TextEditingController controller;
  final FocusNode? focusNode;
  final bool confirmPassword;
  bool obscurePassword = true;
  final ValueChanged<String>? onChanged;
  final ValueChanged<String>? onSubmitted;
  PasswordField({
    super.key,
    this.error,
    required this.controller,
    this.focusNode,
    this.onChanged,
    this.onSubmitted,
    this.confirmPassword = false,
  });

  @override
  State<PasswordField> createState() => _PasswordFieldState();
}

class _PasswordFieldState extends State<PasswordField> {
  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 10),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(10),
        border: Border.all(
          color: widget.error != null
              ? CupertinoColors.systemRed
              : AppColors.lightGray,
        ),
      ),
      child: CupertinoTextField(
        controller: widget.controller,
        focusNode: widget.focusNode,
        placeholder: widget.confirmPassword
            ? AppLocalizations.of(context)!.confirmPassword
            : AppLocalizations.of(context)!.password,
        obscureText: widget.obscurePassword,
        textInputAction: TextInputAction.done,
        style: const TextStyle(color: CupertinoColors.black),
        prefix: SvgPicture.asset('assets/images/password_icon.svg'),
        onTapOutside: (event) {
          FocusManager.instance.primaryFocus?.unfocus();
        },
        onChanged: widget.onChanged,
        onSubmitted: widget.onSubmitted,
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        decoration: null,
        suffix: CupertinoButton(
          padding: const EdgeInsets.symmetric(horizontal: 2, vertical: 8),
          minimumSize: Size.zero,
          onPressed: () {
            setState(() {
              widget.obscurePassword = !widget.obscurePassword;
            });
          },
          child: Icon(
            widget.obscurePassword
                ? CupertinoIcons.eye_slash
                : CupertinoIcons.eye,
            color: AppColors.mediumBlueGray,
            size: 20,
          ),
        ),
      ),
    );
  }
}
