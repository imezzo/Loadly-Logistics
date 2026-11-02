import 'package:flutter/cupertino.dart';

import '../../../../../core/theme/app_colors.dart';
import '../../../../../l10n/gen/app_localizations.dart';
import '../../../domain/entities/trip.dart';

/// Shipment details card (weight, type, duration, dimensions).
class TripShipmentDetailsCard extends StatelessWidget {
  const TripShipmentDetailsCard({
    super.key,
    required this.shipment,
    required this.l10n,
  });

  final ShipmentDetails shipment;
  final AppLocalizations l10n;

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.symmetric(horizontal: 16),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.lightGray, width: 1),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          _DetailRow(
            label: l10n.weight,
            value: '${shipment.weight.toStringAsFixed(0)} kg',
          ),
          const SizedBox(height: 12),
          _DetailRow(label: l10n.shipmentType, value: shipment.type),
          const SizedBox(height: 12),
          _DetailRow(
            label: l10n.tripDuration,
            value: '${shipment.duration} ${l10n.hours} ${l10n.approximately}',
          ),
          if (shipment.dimensions != null) ...[
            const SizedBox(height: 12),
            _DetailRow(
              label: l10n.shipmentDimensions,
              value: shipment.dimensions!,
            ),
          ],
        ],
      ),
    );
  }
}

class _DetailRow extends StatelessWidget {
  const _DetailRow({required this.label, required this.value});

  final String label;
  final String value;

  @override
  Widget build(BuildContext context) {
    return Row(
      crossAxisAlignment: CrossAxisAlignment.start,
      children: [
        Expanded(
          flex: 2,
          child: Text(
            label,
            style: const TextStyle(
              fontSize: 14,
              color: AppColors.mediumBlueGray,
            ),
          ),
        ),
        Expanded(
          flex: 3,
          child: Text(
            value,
            style: const TextStyle(
              fontSize: 14,
              fontWeight: FontWeight.w600,
              color: AppColors.darkGray,
            ),
          ),
        ),
      ],
    );
  }
}
