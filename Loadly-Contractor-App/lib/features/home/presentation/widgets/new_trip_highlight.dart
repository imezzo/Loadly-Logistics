import 'package:flutter/cupertino.dart';
import 'package:flutter_svg/flutter_svg.dart';

import '../../../../core/theme/app_colors.dart';

/// Orange glow frame and "new" badge while [highlight] is true (matches home design).
class NewTripHighlight extends StatefulWidget {
  const NewTripHighlight({
    super.key,
    required this.child,
    required this.highlight,
    required this.badgeLabel,
  });

  final Widget child;
  final bool highlight;
  final String badgeLabel;

  @override
  State<NewTripHighlight> createState() => _NewTripHighlightState();
}

class _NewTripHighlightState extends State<NewTripHighlight>
    with SingleTickerProviderStateMixin {
  AnimationController? _controller;

  static const _radius = 14.0;
  static const _borderWidth = 2.0;
  static const _whiteGapWidth = 2.5;
  static const _badgeRotation = -0.11;

  @override
  void initState() {
    super.initState();
    _syncController();
  }

  @override
  void didUpdateWidget(covariant NewTripHighlight oldWidget) {
    super.didUpdateWidget(oldWidget);
    _syncController();
  }

  void _syncController() {
    if (widget.highlight) {
      _controller ??= AnimationController(
        vsync: this,
        duration: const Duration(milliseconds: 1300),
      )..repeat(reverse: true);
    } else {
      _controller?.dispose();
      _controller = null;
    }
  }

  @override
  void dispose() {
    _controller?.dispose();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    if (!widget.highlight || _controller == null) {
      return widget.child;
    }

    return AnimatedBuilder(
      animation: _controller!,
      builder: (context, child) {
        final t = Curves.easeInOut.transform(_controller!.value);
        final glowAlpha = 0.5 + 0.2 * t;

        const frameRadius = _radius + 2;

        return Padding(
          padding: const EdgeInsets.fromLTRB(6, 14, 6, 4),
          child: Stack(
            clipBehavior: Clip.none,
            children: [
              // Glow outside the solid border.
              DecoratedBox(
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(frameRadius),
                  boxShadow: [
                    BoxShadow(
                      color: AppColors.newTripHighlightBorder.withValues(
                        alpha: glowAlpha,
                      ),
                      blurRadius: 6 + 4 * t,
                      spreadRadius: 0.5 * t,
                    ),
                    BoxShadow(
                      color: AppColors.newTripHighlightBorder.withValues(
                        alpha: 0.32 + 0.14 * t,
                      ),
                      blurRadius: 14 + 6 * t,
                      spreadRadius: 0,
                    ),
                  ],
                ),
                child: Container(
                  decoration: BoxDecoration(
                    color: AppColors.white,
                    borderRadius: BorderRadius.circular(frameRadius),
                    border: Border.all(
                      color: AppColors.newTripHighlightBorder,
                      width: _borderWidth,
                    ),
                  ),
                  padding: const EdgeInsets.all(_whiteGapWidth),
                  child: child,
                ),
              ),
              Positioned(
                top: -10,
                right: 5,
                child: Transform.rotate(
                  angle: _badgeRotation,
                  child: _NewTripBadge(label: widget.badgeLabel),
                ),
              ),
            ],
          ),
        );
      },
      child: widget.child,
    );
  }
}

class _NewTripBadge extends StatelessWidget {
  const _NewTripBadge({required this.label});

  final String label;

  @override
  Widget build(BuildContext context) {
    return DecoratedBox(
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(22),
        color: AppColors.newTripHighlightBadge,
      ),
      child: Padding(
        padding: const EdgeInsets.fromLTRB(6, 5, 12, 5),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Text(
              label,
              style: const TextStyle(
                fontSize: 17,
                fontWeight: FontWeight.w800,
                color: AppColors.white,
                height: 1,
              ),
            ),
            SvgPicture.asset('assets/images/seal.svg', width: 28, height: 28),
            const SizedBox(width: 4),
          ],
        ),
      ),
    );
  }
}
