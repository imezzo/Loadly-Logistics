import 'package:flutter/cupertino.dart';

/// Shared back button using the app's custom back_button_circle.png.
/// Flips horizontally in LTR so the arrow points left; uses as-is in RTL.
class AppBackButton extends StatelessWidget {
  const AppBackButton({super.key, required this.onPressed});

  final VoidCallback onPressed;

  @override
  Widget build(BuildContext context) {
    final isRtl = Directionality.of(context) == TextDirection.rtl;
    final image = Image.asset(
      'assets/images/back_button_circle.png',
      width: 32,
      height: 32,
      fit: BoxFit.contain,
    );

    return CupertinoButton(
      padding: EdgeInsets.zero,
      minimumSize: Size.zero,
      onPressed: onPressed,
      child: isRtl ? image : Transform.flip(flipX: true, child: image),
    );
  }
}
