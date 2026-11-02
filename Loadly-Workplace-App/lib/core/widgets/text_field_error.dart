import 'package:flutter/cupertino.dart';

class TextFieldError extends StatelessWidget {
  final String? error;
  const TextFieldError({super.key, required this.error});

  @override
  Widget build(BuildContext context) {
    if (error != null) {
      return Padding(
        padding: const EdgeInsets.only(top: 8.0),
        child: Align(
          alignment: AlignmentDirectional.centerStart,
          child: Text(
            error!,
            style: const TextStyle(
              color: CupertinoColors.systemRed,
              fontSize: 13,
            ),
          ),
        ),
      );
    } else {
      return const SizedBox();
    }
  }
}
