import 'package:flutter/cupertino.dart';

import '../../../../../core/theme/app_colors.dart';
import '../../../../../l10n/gen/app_localizations.dart';
import '../../../domain/entities/trip.dart';

/// Vehicle details card (license plate, description, color, image).
class TripVehicleDetailsCard extends StatelessWidget {
  const TripVehicleDetailsCard({
    super.key,
    required this.vehicleDetails,
    required this.l10n,
  });

  final VehicleDetails vehicleDetails;
  final AppLocalizations l10n;

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.all(16),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.lightGray, width: 1),
      ),
      child: Row(
        children: [
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  l10n.vehicleDetails,
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w600,
                    color: AppColors.darkGray,
                  ),
                ),
                const SizedBox(height: 8),
                Text(
                  vehicleDetails.licensePlate,
                  style: const TextStyle(
                    fontSize: 18,
                    fontWeight: FontWeight.w600,
                    color: AppColors.mainBlue,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  vehicleDetails.description,
                  style: const TextStyle(
                    fontSize: 14,
                    color: AppColors.darkGray,
                  ),
                ),
                if (vehicleDetails.color != null) ...[
                  const SizedBox(height: 4),
                  Text(
                    vehicleDetails.color!,
                    style: const TextStyle(
                      fontSize: 14,
                      color: AppColors.mediumBlueGray,
                    ),
                  ),
                ],
              ],
            ),
          ),
          if (vehicleDetails.imageUrl != null)
            Container(
              width: 80,
              height: 80,
              decoration: BoxDecoration(
                color: AppColors.lightGray,
                borderRadius: BorderRadius.circular(8),
              ),
              child: ClipRRect(
                borderRadius: BorderRadius.circular(8),
                child: Image.network(
                  vehicleDetails.imageUrl!,
                  fit: BoxFit.cover,
                  errorBuilder: (context, error, stackTrace) {
                    return const Icon(
                      CupertinoIcons.car,
                      size: 40,
                      color: AppColors.mediumBlueGray,
                    );
                  },
                ),
              ),
            )
          else
            Container(
              width: 80,
              height: 80,
              decoration: BoxDecoration(
                color: AppColors.lightGray,
                borderRadius: BorderRadius.circular(8),
              ),
              child: const Icon(
                CupertinoIcons.car,
                size: 40,
                color: AppColors.mediumBlueGray,
              ),
            ),
        ],
      ),
    );
  }
}
