import 'package:flutter/cupertino.dart';
import 'package:flutter_svg/svg.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../l10n/gen/app_localizations.dart';

// ignore: must_be_immutable
class EmailField extends StatefulWidget {
  String? error;
  final TextEditingController controller;
  final FocusNode focusNode;
  final FocusNode nextFocusNode;
  final ValueChanged<String> onChanged;
  final bool isPhone;
  EmailField({
    super.key,
    required this.error,
    required this.controller,
    required this.focusNode,
    required this.nextFocusNode,
    required this.onChanged,
    this.isPhone = false,
  });

  @override
  State<EmailField> createState() => _EmailFieldState();
}

class _EmailFieldState extends State<EmailField> {
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
        placeholder: widget.isPhone
            ? '${AppLocalizations.of(context)!.email} / ${AppLocalizations.of(context)!.phone}'
            : AppLocalizations.of(context)!.email,
        keyboardType: TextInputType.emailAddress,
        textInputAction: TextInputAction.next,
        style: const TextStyle(color: CupertinoColors.black),
        prefix: SvgPicture.asset('assets/images/email_icon.svg'),
        onTapOutside: (event) {
          FocusManager.instance.primaryFocus?.unfocus();
        },
        onChanged: widget.onChanged,
        onSubmitted: (_) {
          widget.nextFocusNode.requestFocus();
        },
        padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 14),
        decoration: null,
      ),
    );
  }
}
