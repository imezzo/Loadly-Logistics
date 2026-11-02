import 'package:flutter/cupertino.dart';

import '../../../../../core/theme/app_colors.dart';
import '../../../../../l10n/gen/app_localizations.dart';
import '../../../domain/entities/trip.dart';
import 'trip_details_constants.dart';
import 'trip_details_location_item.dart';

typedef _LocationItem = ({
  String label,
  String address,
  String? subtitle,
  bool isCompleted,
  bool isArrived,
  bool isLoadingCargo,
  bool isLoadingCompleted,
  bool isLoadingUnloading,
  bool isUnloadingCompleted,
});

/// Pickup and delivery locations timeline.
class TripDetailsLocationsSection extends StatelessWidget {
  const TripDetailsLocationsSection({
    super.key,
    required this.trip,
    required this.currentUserLocationAddress,
    required this.lineAnimationController,
    this.startLocationAddress,
  });

  final Trip trip;
  final String? currentUserLocationAddress;
  final AnimationController? lineAnimationController;

  /// When trip has startLocation from API, pass reverse-geocoded address here.
  final String? startLocationAddress;

  List<_LocationItem> _buildItems(AppLocalizations l10n) {
    final waypoints = trip.waypoints ?? [];
    final departedFromFirst = hasDepartedFromFirstWaypoint(trip);
    final items = <_LocationItem>[];
    // Prefer API-recorded startLocation address; fallback to current user location.
    final startTripAddress =
        startLocationAddress ??
        currentUserLocationAddress ??
        (trip.startLocation != null
            ? '${trip.startLocation!.latitude.toStringAsFixed(6)}, ${trip.startLocation!.longitude.toStringAsFixed(6)}'
            : l10n.currentLocationLoading);

    if (departedFromFirst) {
      if (waypoints.isNotEmpty) {
        items.add((
          label: l10n.startTripLocation,
          address: startTripAddress,
          subtitle: null,
          isCompleted: true,
          isArrived: false,
          isLoadingCargo: false,
          isLoadingCompleted: false,
          isLoadingUnloading: false,
          isUnloadingCompleted: false,
        ));
        items.add((
          label: l10n.pickupLocation,
          address: waypoints.first.address,
          subtitle: waypoints.first.taskDetails,
          isCompleted: isWaypointDepartedOrCompleted(waypoints.first.status),
          isArrived: waypoints.first.status == kWaypointArrived,
          isLoadingCargo: waypoints.first.status == kWaypointLoadingStarted,
          isLoadingCompleted:
              waypoints.first.status == kWaypointLoadingCompleted,
          isLoadingUnloading:
              waypoints.first.status == kWaypointUnloadingStarted,
          isUnloadingCompleted:
              waypoints.first.status == kWaypointUnloadingCompleted,
        ));
        for (final wp in waypoints.sublist(1)) {
          items.add((
            label: wp.isPickup
                ? l10n.pickupLocation
                : wp.isDepot
                ? l10n.depotLocation
                : l10n.deliveryLocation,
            address: wp.address,
            subtitle: wp.taskDetails,
            isCompleted: isWaypointDepartedOrCompleted(wp.status),
            isArrived: wp.status == kWaypointArrived,
            isLoadingCargo: wp.status == kWaypointLoadingStarted,
            isLoadingCompleted: wp.status == kWaypointLoadingCompleted,
            isLoadingUnloading: wp.status == kWaypointUnloadingStarted,
            isUnloadingCompleted: wp.status == kWaypointUnloadingCompleted,
          ));
        }
        if (waypoints.length == 1) {
          items.add((
            label: l10n.deliveryLocation,
            address: trip.deliveryLocation.address,
            subtitle: null,
            isCompleted: false,
            isArrived: false,
            isLoadingCargo: false,
            isLoadingCompleted: false,
            isLoadingUnloading: false,
            isUnloadingCompleted: false,
          ));
        }
      } else {
        items.add((
          label: l10n.startTripLocation,
          address: startTripAddress,
          subtitle: null,
          isCompleted: true,
          isArrived: false,
          isLoadingCargo: false,
          isLoadingCompleted: false,
          isLoadingUnloading: false,
          isUnloadingCompleted: false,
        ));
        items.add((
          label: l10n.pickupLocation,
          address: trip.pickupLocation.address,
          subtitle: null,
          isCompleted: true,
          isArrived: false,
          isLoadingCargo: false,
          isLoadingCompleted: false,
          isLoadingUnloading: false,
          isUnloadingCompleted: false,
        ));
        items.add((
          label: l10n.deliveryLocation,
          address: trip.deliveryLocation.address,
          subtitle: null,
          isCompleted: false,
          isArrived: false,
          isLoadingCargo: false,
          isLoadingCompleted: false,
          isLoadingUnloading: false,
          isUnloadingCompleted: false,
        ));
      }
    } else {
      items.add((
        label: l10n.startTripLocation,
        address: startTripAddress,
        subtitle: null,
        isCompleted: false,
        isArrived: false,
        isLoadingCargo: false,
        isLoadingCompleted: false,
        isLoadingUnloading: false,
        isUnloadingCompleted: false,
      ));
      if (waypoints.isNotEmpty) {
        for (final wp in waypoints) {
          items.add((
            label: wp.isPickup
                ? l10n.pickupLocation
                : wp.isDepot
                ? l10n.depotLocation
                : l10n.deliveryLocation,
            address: wp.address,
            subtitle: wp.taskDetails,
            isCompleted: isWaypointDepartedOrCompleted(wp.status),
            isArrived: wp.status == kWaypointArrived,
            isLoadingCargo: wp.status == kWaypointLoadingStarted,
            isLoadingCompleted: wp.status == kWaypointLoadingCompleted,
            isLoadingUnloading: wp.status == kWaypointUnloadingStarted,
            isUnloadingCompleted: wp.status == kWaypointUnloadingCompleted,
          ));
        }
        if (waypoints.length == 1) {
          items.add((
            label: l10n.deliveryLocation,
            address: trip.deliveryLocation.address,
            subtitle: null,
            isCompleted: false,
            isArrived: false,
            isLoadingCargo: false,
            isLoadingCompleted: false,
            isLoadingUnloading: false,
            isUnloadingCompleted: false,
          ));
        }
      } else {
        items.add((
          label: l10n.pickupLocation,
          address: trip.pickupLocation.address,
          subtitle: null,
          isCompleted: false,
          isArrived: false,
          isLoadingCargo: false,
          isLoadingCompleted: false,
          isLoadingUnloading: false,
          isUnloadingCompleted: false,
        ));
        items.add((
          label: l10n.deliveryLocation,
          address: trip.deliveryLocation.address,
          subtitle: null,
          isCompleted: false,
          isArrived: false,
          isLoadingCargo: false,
          isLoadingCompleted: false,
          isLoadingUnloading: false,
          isUnloadingCompleted: false,
        ));
      }
    }

    return items;
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final items = _buildItems(l10n);

    return Container(
      margin: const EdgeInsets.all(16),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.white,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.lightGray, width: 1),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            l10n.tripPoints,
            style: const TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.w600,
              color: AppColors.darkGray,
            ),
          ),
          const SizedBox(height: 16),
          for (var i = 0; i < items.length; i++)
            TripDetailsLocationItem(
              label: items[i].label,
              address: items[i].address,
              subtitle: items[i].subtitle,
              isCompleted: items[i].isCompleted,
              isArrived: items[i].isArrived,
              isLoadingCargo: items[i].isLoadingCargo,
              isLoadingCompleted: items[i].isLoadingCompleted,
              isLoadingUnloading: items[i].isLoadingUnloading,
              isUnloadingCompleted: items[i].isUnloadingCompleted,
              isFirst: i == 0,
              isLast: i == items.length - 1,
              isLineCompleted:
                  i < items.length - 1 &&
                  items[i].isCompleted &&
                  items[i + 1].isCompleted,
              isLineActive:
                  i < items.length - 1 &&
                  items[i].isCompleted &&
                  !items[i + 1].isCompleted,
              lineAnimation: lineAnimationController,
            ),
        ],
      ),
    );
  }
}
