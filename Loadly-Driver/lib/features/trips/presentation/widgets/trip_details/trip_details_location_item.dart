import 'dart:math' show pi;

import 'package:flutter/cupertino.dart';

import '../../../../../core/theme/app_colors.dart';
import '../../../../../l10n/gen/app_localizations.dart';

/// Line segment between location items.
class TripDetailsLineSegment extends StatelessWidget {
  const TripDetailsLineSegment({
    super.key,
    required this.isCompleted,
    required this.isActive,
    this.animation,
  });

  final bool isCompleted;
  final bool isActive;
  final AnimationController? animation;

  @override
  Widget build(BuildContext context) {
    Widget line;
    if (isCompleted) {
      line = Container(width: 2, color: AppColors.green);
    } else if (isActive && animation != null) {
      line = Stack(
        children: [
          Container(width: 2, color: AppColors.mainBlue),
          AnimatedBuilder(
            animation: animation!,
            builder: (context, child) {
              return Container(
                width: 2,
                decoration: BoxDecoration(
                  gradient: LinearGradient(
                    begin: Alignment.topCenter,
                    end: Alignment.bottomCenter,
                    colors: [
                      AppColors.white.withValues(alpha: 0),
                      AppColors.white.withValues(alpha: 0.5),
                      AppColors.white.withValues(alpha: 0),
                    ],
                    stops: [
                      (animation!.value - 0.25).clamp(0.0, 1.0),
                      animation!.value.clamp(0.0, 1.0),
                      (animation!.value + 0.25).clamp(0.0, 1.0),
                    ],
                  ),
                ),
              );
            },
          ),
        ],
      );
    } else {
      line = Container(width: 2, color: AppColors.lightGray);
    }
    return Expanded(child: line);
  }
}

/// Single location item in the trip timeline.
class TripDetailsLocationItem extends StatelessWidget {
  const TripDetailsLocationItem({
    super.key,
    required this.label,
    required this.address,
    this.subtitle,
    required this.isCompleted,
    required this.isArrived,
    required this.isLoadingCargo,
    required this.isLoadingCompleted,
    required this.isLoadingUnloading,
    required this.isUnloadingCompleted,
    required this.isFirst,
    required this.isLast,
    this.isLineCompleted = false,
    this.isLineActive = false,
    this.lineAnimation,
  });

  final String label;
  final String address;
  final String? subtitle;
  final bool isCompleted;
  final bool isArrived;
  final bool isLoadingCargo;
  final bool isLoadingCompleted;
  final bool isLoadingUnloading;
  final bool isUnloadingCompleted;
  final bool isFirst;
  final bool isLast;
  final bool isLineCompleted;
  final bool isLineActive;
  final AnimationController? lineAnimation;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final noteText = (subtitle != null && subtitle!.isNotEmpty)
        ? subtitle!
        : l10n.tripPointNoNote;

    return IntrinsicHeight(
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Column(
            mainAxisSize: MainAxisSize.max,
            children: [
              _LocationIndicator(
                isCompleted: isCompleted,
                isArrived: isArrived,
                isLoadingCargo: isLoadingCargo,
                isLoadingCompleted: isLoadingCompleted,
                isLoadingUnloading: isLoadingUnloading,
                isUnloadingCompleted: isUnloadingCompleted,
                isFirst: isFirst,
                lineAnimation: lineAnimation,
              ),
              if (!isLast)
                TripDetailsLineSegment(
                  isCompleted: isLineCompleted,
                  isActive: isLineActive,
                  animation: lineAnimation,
                ),
            ],
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  label,
                  style: TextStyle(
                    fontSize: 12,
                    fontWeight: FontWeight.w600,
                    color: isFirst ? AppColors.mainBlue : AppColors.darkGray,
                  ),
                ),
                const SizedBox(height: 4),
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Icon(
                      CupertinoIcons.placemark_fill,
                      size: 16,
                      color: AppColors.darkGray,
                    ),
                    const SizedBox(width: 6),
                    Expanded(
                      child: Text(
                        address,
                        style: const TextStyle(
                          fontSize: 14,
                          color: AppColors.darkGray,
                        ),
                      ),
                    ),
                  ],
                ),
                const SizedBox(height: 4),
                Row(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Icon(
                      CupertinoIcons.doc_text,
                      size: 14,
                      color: AppColors.mediumBlueGray,
                    ),
                    const SizedBox(width: 6),
                    Expanded(
                      child: Text(
                        '${l10n.notePlaceholder}: $noteText',
                        style: const TextStyle(
                          fontSize: 13,
                          color: AppColors.mediumBlueGray,
                        ),
                      ),
                    ),
                  ],
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _LocationIndicator extends StatelessWidget {
  const _LocationIndicator({
    required this.isCompleted,
    required this.isArrived,
    required this.isLoadingCargo,
    required this.isLoadingCompleted,
    required this.isLoadingUnloading,
    required this.isUnloadingCompleted,
    required this.isFirst,
    this.lineAnimation,
  });

  final bool isCompleted;
  final bool isArrived;
  final bool isLoadingCargo;
  final bool isLoadingCompleted;
  final bool isLoadingUnloading;
  final bool isUnloadingCompleted;
  final bool isFirst;
  final AnimationController? lineAnimation;

  @override
  Widget build(BuildContext context) {
    final showCargoIcon =
        isLoadingCargo ||
        isLoadingCompleted ||
        isLoadingUnloading ||
        isUnloadingCompleted;
    final loadingOrUnloadingDone = isLoadingCompleted || isUnloadingCompleted;
    final circleColor = isCompleted || loadingOrUnloadingDone
        ? AppColors.green
        : (showCargoIcon || isFirst ? AppColors.mainBlue : AppColors.lightGray);
    final borderColor = isCompleted || loadingOrUnloadingDone
        ? AppColors.green
        : (showCargoIcon || isFirst
              ? AppColors.mainBlue
              : AppColors.mediumBlueGray);
    final inner = Container(
      width: 20,
      height: 20,
      decoration: BoxDecoration(
        shape: BoxShape.circle,
        color: circleColor,
        border: Border.all(color: borderColor, width: 2),
      ),
      child: isCompleted
          ? const Center(
              child: Icon(
                CupertinoIcons.checkmark,
                size: 12,
                color: AppColors.white,
              ),
            )
          : showCargoIcon
          ? const Center(
              child: Icon(
                CupertinoIcons.cube_box_fill,
                size: 12,
                color: AppColors.white,
              ),
            )
          : null,
    );

    if (isArrived && lineAnimation != null) {
      return AnimatedBuilder(
        animation: lineAnimation!,
        builder: (context, child) {
          final t = lineAnimation!.value;
          final pulse = 0.4 + 0.6 * (1 - (t - 0.5).abs() * 2).clamp(0.0, 1.0);
          return Container(
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              border: Border.all(
                color: AppColors.mainBlue.withValues(alpha: pulse),
                width: 3,
              ),
            ),
            child: child,
          );
        },
        child: inner,
      );
    }

    if (isLoadingCargo && !isLoadingCompleted && lineAnimation != null) {
      return AnimatedBuilder(
        animation: lineAnimation!,
        builder: (context, child) {
          return Transform.rotate(
            angle: lineAnimation!.value * 2 * pi,
            child: Stack(
              alignment: Alignment.center,
              children: [
                Container(
                  width: 24,
                  height: 24,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    gradient: SweepGradient(
                      center: Alignment.center,
                      colors: [
                        AppColors.mainBlue,
                        AppColors.white.withValues(alpha: 0.5),
                        AppColors.mainBlue,
                        AppColors.mainBlue,
                      ],
                      stops: const [0.0, 0.25, 0.5, 1.0],
                    ),
                  ),
                ),
                child!,
              ],
            ),
          );
        },
        child: inner,
      );
    }

    if (isLoadingUnloading && !isUnloadingCompleted && lineAnimation != null) {
      return AnimatedBuilder(
        animation: lineAnimation!,
        builder: (context, child) {
          return Transform.rotate(
            angle: -lineAnimation!.value * 2 * pi,
            child: Stack(
              alignment: Alignment.center,
              children: [
                Container(
                  width: 24,
                  height: 24,
                  decoration: BoxDecoration(
                    shape: BoxShape.circle,
                    gradient: SweepGradient(
                      center: Alignment.center,
                      colors: [
                        AppColors.mainBlue,
                        AppColors.white.withValues(alpha: 0.5),
                        AppColors.mainBlue,
                        AppColors.mainBlue,
                      ],
                      stops: const [0.0, 0.25, 0.5, 1.0],
                    ),
                  ),
                ),
                child!,
              ],
            ),
          );
        },
        child: inner,
      );
    }

    return inner;
  }
}
