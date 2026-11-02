import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';

import '../theme/app_colors.dart';

/// Header pattern asset (`assets/images/Group.svg`).
abstract final class AppHeaderPattern {
  static const assetPath = 'assets/images/Group.svg';
  static const tileWidth = 378.0;
  static const tileHeight = 223.0;

  /// Pattern overlay strength (40% visible).
  static const overlayOpacity = 0.2;
}

/// Solid blue with tiled geometric pattern (full bleed).
class AppBluePatternFill extends StatelessWidget {
  const AppBluePatternFill({super.key, this.color = AppColors.mainBlue});

  final Color color;

  @override
  Widget build(BuildContext context) {
    return ColoredBox(
      color: color,
      child: const RepaintBoundary(
        child: Opacity(
          opacity: AppHeaderPattern.overlayOpacity,
          child:  _TiledHeaderPattern(),
        ),
      ),
    );
  }
}

/// Blue + pattern behind foreground content (nav bar, page headers).
class AppBluePatternBackground extends StatelessWidget {
  const AppBluePatternBackground({
    super.key,
    required this.child,
    this.color = AppColors.mainBlue,
  });

  final Widget child;
  final Color color;

  @override
  Widget build(BuildContext context) {
    // Loose fit sizes this stack to [child]. Expand would turn a Column's
    // unbounded height into an infinite constraint and leave the route black.
    return Stack(
      fit: StackFit.loose,
      children: [
        Positioned.fill(child: AppBluePatternFill(color: color)),
        child,
      ],
    );
  }
}

class _TiledHeaderPattern extends StatelessWidget {
  const _TiledHeaderPattern();

  @override
  Widget build(BuildContext context) {
    return LayoutBuilder(
      builder: (context, constraints) {
        final maxW = constraints.maxWidth;
        final maxH = constraints.maxHeight;
        if (!maxW.isFinite || !maxH.isFinite || maxW <= 0 || maxH <= 0) {
          return const SizedBox.shrink();
        }

        const tileW = AppHeaderPattern.tileWidth;
        const tileH = AppHeaderPattern.tileHeight;
        final cols = (maxW / tileW).ceil() + 1;
        final rows = (maxH / tileH).ceil() + 1;

        return ClipRect(
          child: Stack(
            clipBehavior: Clip.hardEdge,
            children: [
              for (var row = 0; row < rows; row++)
                for (var col = 0; col < cols; col++)
                  Positioned(
                    left: col * tileW,
                    top: row * tileH,
                    width: tileW,
                    height: tileH,
                    child: SvgPicture.asset(
                      AppHeaderPattern.assetPath,
                      fit: BoxFit.fill,
                    ),
                  ),
            ],
          ),
        );
      },
    );
  }
}
