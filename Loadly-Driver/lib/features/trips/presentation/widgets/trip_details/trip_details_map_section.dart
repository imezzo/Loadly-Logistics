import 'package:flutter/cupertino.dart';
import 'package:flutter/foundation.dart' show Factory;
import 'package:flutter/gestures.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';

import '../../../../../core/theme/app_colors.dart';
import '../../../../../l10n/gen/app_localizations.dart';
import '../../../domain/entities/trip.dart';
import 'trip_details_constants.dart';

/// Map section showing trip route and optional navigate button.
class TripDetailsMapSection extends StatelessWidget {
  const TripDetailsMapSection({
    super.key,
    required this.trip,
    required this.markers,
    required this.polylines,
    required this.mapKey,
    required this.l10n,
    required this.onMapCreated,
    required this.onNavigateToWaypoint,
  });

  final Trip trip;
  final Set<Marker> markers;
  final Set<Polyline> polylines;
  final Object mapKey;
  final AppLocalizations l10n;
  final void Function(GoogleMapController controller) onMapCreated;
  final void Function(Trip trip, TripWaypoint waypoint) onNavigateToWaypoint;

  @override
  Widget build(BuildContext context) {
    double? centerLat;
    double? centerLng;

    if (trip.pickupLocation.latitude != null &&
        trip.pickupLocation.longitude != null) {
      centerLat = trip.pickupLocation.latitude;
      centerLng = trip.pickupLocation.longitude;
    } else if (trip.deliveryLocation.latitude != null &&
        trip.deliveryLocation.longitude != null) {
      centerLat = trip.deliveryLocation.latitude;
      centerLng = trip.deliveryLocation.longitude;
    }

    final nextWaypoint = getNextWaypointToNavigateTo(trip);

    return Container(
      height: 300,
      margin: const EdgeInsets.symmetric(horizontal: 16),
      decoration: BoxDecoration(
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.lightGray, width: 1),
      ),
      child: ClipRRect(
        borderRadius: BorderRadius.circular(12),
        child: Stack(
          children: [
            GoogleMap(
              key: ValueKey(mapKey),
              style: '[]',
              onMapCreated: onMapCreated,
              initialCameraPosition: CameraPosition(
                target: centerLat != null && centerLng != null
                    ? LatLng(centerLat, centerLng)
                    : const LatLng(30.0444, 31.2357), // Default to Cairo
                zoom: 12,
              ),
              markers: markers,
              polylines: polylines,
              mapType: MapType.normal,
              myLocationButtonEnabled: false,
              myLocationEnabled: false,
              zoomControlsEnabled: false,
              gestureRecognizers: {
                Factory<OneSequenceGestureRecognizer>(
                  () => EagerGestureRecognizer(),
                ),
              },
            ),
            if (nextWaypoint != null && shouldShowNavigateButton(trip))
              Positioned(
                bottom: 12,
                left: 12,
                right: 12,
                child: CupertinoButton(
                  padding: const EdgeInsets.symmetric(vertical: 12),
                  color: AppColors.mainBlue,
                  borderRadius: BorderRadius.circular(10),
                  onPressed: () => onNavigateToWaypoint(trip, nextWaypoint),
                  child: Row(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      const Icon(
                        CupertinoIcons.location_fill,
                        color: AppColors.white,
                        size: 20,
                      ),
                      const SizedBox(width: 8),
                      Text(
                        nextWaypoint.isPickup
                            ? l10n.navigateToPickup
                            : nextWaypoint.isDepot
                            ? l10n.navigateToDepot
                            : l10n.navigateToDropoff,
                        style: const TextStyle(
                          color: AppColors.white,
                          fontSize: 16,
                          fontWeight: FontWeight.w600,
                        ),
                      ),
                    ],
                  ),
                ),
              ),
          ],
        ),
      ),
    );
  }
}
