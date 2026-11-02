import 'package:flutter/cupertino.dart';
import 'package:intl/intl.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_theme.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../domain/entities/trip/trip.dart';

class TripsFinancialDueCardData {
  const TripsFinancialDueCardData({
    required this.tripId,
    required this.companyName,
    required this.price,
    required this.fromLocation,
    required this.toLocation,
    required this.isPaidToContractor,
  });

  final int tripId;
  final String companyName;
  final num price;
  final String fromLocation;
  final String toLocation;

  /// From list-trips API (`isPaidToContractor` / `IsPaidToContractor`).
  final bool? isPaidToContractor;

  factory TripsFinancialDueCardData.fromTrip(
    Trip trip, {
    required bool preferArabic,
  }) {
    final waypoints = trip.waypoints;
    final from = waypoints != null && waypoints.isNotEmpty
        ? (waypoints.first.addressName?.trim().isNotEmpty == true
              ? waypoints.first.addressName!.trim()
              : '—')
        : '—';
    final to = waypoints != null && waypoints.isNotEmpty
        ? (waypoints.last.addressName?.trim().isNotEmpty == true
              ? waypoints.last.addressName!.trim()
              : '—')
        : '—';
    final price =
        trip.contractorPrice ??
        trip.cargoPrice ??
        trip.spotPrice ??
        trip.totalTripCost ??
        0;
    return TripsFinancialDueCardData(
      tripId: trip.id ?? 0,
      companyName: trip.companyDisplayNameOrDash(preferArabic: preferArabic),
      price: price,
      fromLocation: from,
      toLocation: to,
      isPaidToContractor: trip.isPaidToContractor,
    );
  }
}

class TripsFinancialDueCard extends StatelessWidget {
  const TripsFinancialDueCard({super.key, required this.data});

  final TripsFinancialDueCardData data;

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final priceText = NumberFormat('#,###').format(data.price);
    // `true` → تم الاستحقاق (فاتح) | `false` → نقلة مستحقة (غامق)
    final isPaid = data.isPaidToContractor == true;
    final badgeLabel = isPaid
        ? l10n.entitlementFulfilledBadge
        : l10n.dueTransferTripBadge;
    final badgeBackground = isPaid
        ? AppColors.mainBlue.withValues(alpha: 0.12)
        : AppColors.mainBlue;
    final badgeTextColor = isPaid ? AppColors.mainBlue : AppColors.white;
    final cardBackgroundColor = isPaid
        ? AppColors.white
        : CupertinoColors.systemCyan.withValues(alpha: 0.05);

    return Container(
      width: double.infinity,
      padding: const EdgeInsets.all(12),
      decoration: BoxDecoration(
        color: cardBackgroundColor,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.lightGray),
        boxShadow: [
          BoxShadow(
            color: CupertinoColors.systemGrey.withValues(alpha: 0.12),
            blurRadius: 8,
            offset: const Offset(0, 2),
          ),
        ],
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Align(
            alignment: AlignmentDirectional.centerStart,
            child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 6),
              decoration: BoxDecoration(
                color: badgeBackground,
                borderRadius: BorderRadius.circular(20),
              ),
              child: Text(
                badgeLabel,
                style: TextStyle(
                  fontFamily: appFontFamily,
                  fontSize: 12,
                  fontWeight: FontWeight.w400,
                  color: badgeTextColor,
                ),
              ),
            ),
          ),
          const SizedBox(height: 8),
          Text(
            '${l10n.transferTripPricePrefix} ${l10n.priceInEgp(priceText)}',
            style: const TextStyle(
              fontFamily: appFontFamily,
              fontSize: 16,
              fontWeight: FontWeight.w700,
              color: AppColors.darkGray,
            ),
          ),
          const SizedBox(height: 6),
          Text(
            data.companyName,
            style: const TextStyle(
              fontFamily: appFontFamily,
              fontSize: 16,
              fontWeight: FontWeight.w400,
              color: AppColors.darkGray,
            ),
          ),
          const SizedBox(height: 4),
          Text(
            '${l10n.trip} #${data.tripId}',
            style: const TextStyle(
              fontFamily: appFontFamily,
              fontSize: 14,
              fontWeight: FontWeight.w400,
              color: AppColors.darkGray,
            ),
          ),
          const SizedBox(height: 12),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
            decoration: BoxDecoration(
              borderRadius: BorderRadius.circular(10),
              border: Border.all(color: AppColors.lightGray),
            ),
            child: Row(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: _RouteColumn(
                    label: l10n.tripFrom,
                    value: data.fromLocation,
                  ),
                ),
                const SizedBox(width: 12),
                Expanded(
                  child: _RouteColumn(
                    label: l10n.tripTo,
                    value: data.toLocation,
                  ),
                ),
              ],
            ),
          ),
        ],
      ),
    );
  }
}

class _RouteColumn extends StatelessWidget {
  const _RouteColumn({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Column(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Text(
          label,
          style: const TextStyle(
            fontFamily: appFontFamily,
            fontSize: 12,
            fontWeight: FontWeight.w400,
            color: CupertinoColors.systemGrey,
          ),
        ),
        const SizedBox(height: 4),
        Text(
          value,
          maxLines: 2,
          overflow: TextOverflow.ellipsis,
          style: const TextStyle(
            fontFamily: appFontFamily,
            fontSize: 14,
            fontWeight: FontWeight.w400,
            color: AppColors.darkGray,
          ),
        ),
      ],
    );
  }
}
