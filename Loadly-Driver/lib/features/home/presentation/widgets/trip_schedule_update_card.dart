import 'package:flutter/cupertino.dart';
import 'package:go_router/go_router.dart';

import '../../../../core/models/trip_schedule_update.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../l10n/gen/app_localizations.dart';

/// Card overlay for trip schedule update notifications, styled like the "New Trip" card.
/// Shown at the bottom of the home screen when a "Trip Schedule Updated" push is received.
class TripScheduleUpdateCard extends StatelessWidget {
  const TripScheduleUpdateCard({
    super.key,
    required this.update,
    required this.onDismiss,
  });

  final TripScheduleUpdate update;
  final VoidCallback onDismiss;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return Container(
      margin: const EdgeInsets.fromLTRB(16, 0, 16, 100),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: const BorderRadius.vertical(top: Radius.circular(24)),
        boxShadow: [
          BoxShadow(
            color: AppColors.darkGray.withValues(alpha: 0.15),
            blurRadius: 16,
            offset: const Offset(0, -4),
          ),
        ],
      ),
      child: Column(
        mainAxisSize: MainAxisSize.min,
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          // Header with green "Trip Schedule Updated" label
          Padding(
            padding: const EdgeInsets.fromLTRB(20, 20, 20, 8),
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                const SizedBox.shrink(),
                Text(
                  l10n.tripScheduleUpdated,
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w600,
                    color: AppColors.green,
                  ),
                ),
                CupertinoButton(
                  padding: EdgeInsets.zero,
                  minimumSize: const Size(32, 32),
                  onPressed: onDismiss,
                  child: const Icon(
                    CupertinoIcons.xmark_circle_fill,
                    color: AppColors.mediumBlueGray,
                    size: 24,
                  ),
                ),
              ],
            ),
          ),
          // Schedule update message
          Padding(
            padding: const EdgeInsets.fromLTRB(20, 0, 20, 12),
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                const Icon(
                  CupertinoIcons.calendar,
                  size: 20,
                  color: AppColors.mainBlue,
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        l10n.tripScheduleUpdatedMessage(
                          '${update.oldDate} ${update.oldTime}'.trim(),
                          '${update.newDate} ${update.newTime}'.trim(),
                        ),
                        style: const TextStyle(
                          fontSize: 14,
                          color: AppColors.darkGray,
                          height: 1.4,
                        ),
                      ),
                      const SizedBox(height: 8),
                      Text(
                        '${l10n.newSchedule}: ${update.newScheduleDisplay}',
                        style: const TextStyle(
                          fontSize: 15,
                          fontWeight: FontWeight.w600,
                          color: AppColors.mainBlue,
                        ),
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ),
          // Trip Details button
          Padding(
            padding: const EdgeInsets.fromLTRB(20, 0, 20, 20),
            child: AppButton(
              label: l10n.tripDetails,
              onPressed: () {
                onDismiss();
                context.go('/trips');
              },
            ),
          ),
        ],
      ),
    );
  }
}
