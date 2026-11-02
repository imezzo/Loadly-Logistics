import 'package:flutter/cupertino.dart';

import '../../../../../core/theme/app_colors.dart';
import '../../../../../l10n/gen/app_localizations.dart';
import '../../../domain/entities/trip.dart';

String _statusCodeToLabel(int? code, AppLocalizations l10n) {
  if (code == null) return l10n.statusUnknown;
  switch (code) {
    case 0:
      return l10n.statusRequested;
    case 1:
      return l10n.statusPending;
    case 2:
      return l10n.statusScheduled;
    case 3:
      return l10n.statusDriverAccepted;
    case 4:
      return l10n.statusFirstPickupLoading;
    case 5:
      return l10n.statusInProgress;
    case kTripStatusCodeCompleted:
    case kTripStatusCodeCompletedReviewed:
      return l10n.statusCompleted;
    case kTripStatusCodeCancelled:
      return l10n.statusCancelled;
    default:
      return l10n.statusUnknown;
  }
}

String _companyName(BuildContext context, Trip trip) {
  final rtl = Directionality.of(context) == TextDirection.rtl;
  return rtl
      ? (trip.companyNameAr ?? trip.companyNameEn ?? trip.title)
      : (trip.companyNameEn ?? trip.companyNameAr ?? trip.title);
}

/// Trip ID, company name, and status badge bar.
class TripIdStatusBar extends StatelessWidget {
  const TripIdStatusBar({super.key, required this.trip, required this.l10n});

  final Trip trip;
  final AppLocalizations l10n;

  @override
  Widget build(BuildContext context) {
    final statusText = _statusCodeToLabel(trip.statusCode, l10n);
    final companyName = _companyName(context, trip);
    return Container(
      margin: const EdgeInsets.fromLTRB(16, 16, 16, 12),
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
      decoration: BoxDecoration(
        color: AppColors.mainBlue.withValues(alpha: 0.08),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.mainBlue.withValues(alpha: 0.2)),
      ),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        crossAxisAlignment: CrossAxisAlignment.center,
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  '#${trip.id}',
                  style: const TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.w600,
                    color: AppColors.darkGray,
                  ),
                ),
                if (companyName.isNotEmpty) ...[
                  const SizedBox(height: 4),
                  Text(
                    companyName,
                    style: const TextStyle(
                      fontSize: 14,
                      color: AppColors.mediumBlueGray,
                    ),
                    maxLines: 2,
                    overflow: TextOverflow.ellipsis,
                  ),
                ],
              ],
            ),
          ),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 4),
            decoration: BoxDecoration(
              color: AppColors.mainBlue,
              borderRadius: BorderRadius.circular(8),
            ),
            child: Text(
              statusText,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w600,
                color: AppColors.white,
              ),
            ),
          ),
        ],
      ),
    );
  }
}
