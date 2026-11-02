import 'package:flutter/cupertino.dart';
import 'package:go_router/go_router.dart';
import 'package:intl/intl.dart' hide TextDirection;

import '../../../../core/router/route_constants.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../trips/domain/entities/trip/trip.dart';
import 'trip_card_details.dart';

class TripRequestCard extends StatelessWidget {
  const TripRequestCard({super.key, required this.trip});

  final Trip trip;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final isRtl = Directionality.of(context) == TextDirection.rtl;
    final isArabic = Localizations.localeOf(context).languageCode == 'ar';
    final companyName = trip.companyDisplayName(preferArabic: isRtl);
    final vehicleName = trip.vehicleTypeDisplayName(preferArabic: isRtl) ?? '—';
    final dateStr = _dateText(l10n, trip.startDate, isArabic: isArabic);
    final cost = trip.contractorPrice;
    final priceText = cost == null
        ? '—'
        : NumberFormat('#,###').format(cost.round()).replaceAll(',', '.');
    final waypoints = trip.waypoints;
    final from = (waypoints != null && waypoints.isNotEmpty)
        ? waypoints.first.addressName ?? '—'
        : '—';
    final to = (waypoints != null && waypoints.isNotEmpty)
        ? waypoints.last.addressName ?? '—'
        : '—';
    final tripId = trip.id;
    final tripNumber = tripId == null ? '—' : '$tripId';

    return Container(
      padding: const EdgeInsets.all(10),
      decoration: BoxDecoration(
        color: AppColors.mainBlue,
        borderRadius: BorderRadius.circular(12),
      ),
      child: Container(
        padding: const EdgeInsets.all(10),
        decoration: BoxDecoration(
          color: AppColors.white,
          borderRadius: BorderRadius.circular(12),
        ),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text(
                        companyName ?? '—',
                        maxLines: 2,
                        overflow: TextOverflow.ellipsis,
                        style: const TextStyle(
                          fontSize: 16,
                          fontWeight: FontWeight.w700,
                          color: AppColors.darkGray,
                        ),
                      ),
                      const SizedBox(height: 4),
                      Text(
                        '${l10n.trip} #$tripNumber',
                        style: const TextStyle(
                          fontSize: 12,
                          fontWeight: FontWeight.w500,
                          color: Color(0xFF7A8A99),
                        ),
                      ),
                    ],
                  ),
                ),
                const SizedBox(width: 8),
                Container(
                  constraints: const BoxConstraints(maxWidth: 140),
                  padding: const EdgeInsets.symmetric(
                    horizontal: 12,
                    vertical: 6,
                  ),
                  decoration: BoxDecoration(
                    color: AppColors.white,
                    borderRadius: BorderRadius.circular(20),
                    border: Border.all(color: AppColors.mainBlue),
                  ),
                  child: Text(
                    '${l10n.tripValue}: $priceText',
                    textAlign: TextAlign.center,
                    softWrap: true,
                    style: const TextStyle(
                      fontSize: 12,
                      fontWeight: FontWeight.w600,
                      color: AppColors.mainBlue,
                    ),
                  ),
                ),
              ],
            ),
            const SizedBox(height: 10),
            Text(
              '${l10n.tripDate} $dateStr',
              style: const TextStyle(
                fontSize: 13,
                fontWeight: FontWeight.w400,
                color: Color(0xFF7A8A99),
              ),
            ),
            const SizedBox(height: 12),
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: _LabeledValue(label: l10n.tripFrom, value: from),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: _LabeledValue(label: l10n.tripTo, value: to),
                ),
              ],
            ),
            const SizedBox(height: 12),
            Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: _LabeledValue(
                    label: l10n.vehicleType,
                    value: vehicleName,
                  ),
                ),
                const SizedBox(width: 8),
                AppButton(
                  minSize: 0,
                  padding: const EdgeInsets.symmetric(
                    horizontal: 12,
                    vertical: 8,
                  ),
                  onPressed: () {
                    final id = trip.id;
                    if (id != null) {
                      context.go(
                        AppRoutePaths.tripById(id),
                        extra: AppRoutePaths.home,
                      );
                    }
                  },
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Text(
                        l10n.requestTripBooking,
                        style: const TextStyle(
                          color: AppColors.white,
                          fontSize: 13,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                      const SizedBox(width: 6),
                      const Icon(
                        CupertinoIcons.add_circled_solid,
                        color: AppColors.white,
                        size: 18,
                      ),
                    ],
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  String _dateText(
    AppLocalizations l10n,
    String? startDate, {
    required bool isArabic,
  }) {
    final parsed = startDate == null ? null : DateTime.tryParse(startDate);
    if (parsed == null) return l10n.statusUnknown;
    return formatTripCardDateTime(parsed, l10n: l10n, isArabic: isArabic);
  }
}

class _LabeledValue extends StatelessWidget {
  const _LabeledValue({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return SizedBox(
      width: double.infinity,
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            label,
            style: const TextStyle(
              fontSize: 10,
              fontWeight: FontWeight.w400,
              color: Color(0xFF7A8A99),
            ),
          ),
          const SizedBox(height: 2),
          Text(
            value,
            softWrap: true,
            style: const TextStyle(
              fontSize: 13,
              fontWeight: FontWeight.w600,
              color: AppColors.darkGray,
            ),
          ),
        ],
      ),
    );
  }
}
