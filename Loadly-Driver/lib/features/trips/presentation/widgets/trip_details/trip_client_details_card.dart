import 'package:flutter/cupertino.dart';

import '../../../../../core/theme/app_colors.dart';
import '../../../../../l10n/gen/app_localizations.dart';
import '../../../domain/entities/trip.dart';

/// Client details card (phone, responsible person).
class TripClientDetailsCard extends StatelessWidget {
  const TripClientDetailsCard({
    super.key,
    required this.clientDetails,
    required this.l10n,
  });

  final ClientDetails clientDetails;
  final AppLocalizations l10n;

  @override
  Widget build(BuildContext context) {
    final rows = <Widget>[];
    if (clientDetails.phoneNumber != null) {
      rows.add(
        _ClientDetailRow(
          context: context,
          label: l10n.phoneNumber,
          value: clientDetails.phoneNumber!,
        ),
      );
    }
    if (clientDetails.responsiblePersonName != null) {
      if (rows.isNotEmpty) {
        rows.add(
          Container(
            margin: const EdgeInsets.symmetric(vertical: 8),
            height: 1,
            color: AppColors.lightGray,
          ),
        );
      }
      rows.add(
        _ClientDetailRow(
          context: context,
          label: l10n.responsiblePersonName,
          value: clientDetails.responsiblePersonName!,
        ),
      );
    }
    if (rows.isEmpty) return const SizedBox.shrink();

    return Container(
      margin: const EdgeInsets.all(16),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.lightGray.withValues(alpha: 0.3),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.lightGray, width: 1),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.stretch,
        children: [
          Align(
            alignment: AlignmentDirectional.topStart,
            child: Text(
              l10n.clientDetails,
              style: const TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.w600,
                color: AppColors.darkGray,
              ),
            ),
          ),
          const SizedBox(height: 12),
          ...rows,
        ],
      ),
    );
  }
}

class _ClientDetailRow extends StatelessWidget {
  const _ClientDetailRow({
    required this.context,
    required this.label,
    required this.value,
  });

  final BuildContext context;
  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        crossAxisAlignment: CrossAxisAlignment.start,
        textDirection: Directionality.of(this.context),
        children: [
          Flexible(
            child: Text(
              label,
              style: const TextStyle(
                fontSize: 14,
                color: AppColors.mediumBlueGray,
              ),
              textAlign: TextAlign.end,
            ),
          ),
          const SizedBox(width: 12),
          Flexible(
            child: Text(
              value,
              style: const TextStyle(
                fontSize: 14,
                fontWeight: FontWeight.w600,
                color: AppColors.darkGray,
              ),
              textAlign: TextAlign.start,
            ),
          ),
        ],
      ),
    );
  }
}
