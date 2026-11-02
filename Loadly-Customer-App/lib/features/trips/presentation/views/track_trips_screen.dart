import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';

import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_nav_bar.dart';
import '../../../../core/widgets/app_primary_button.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../trips/presentation/bloc/trips_bloc.dart';
import '../../../trips/presentation/bloc/trips_event.dart';
import '../../../trips/presentation/bloc/trips_state.dart';
import '../widgets/track_trip_popup.dart';

class TrackTripsScreen extends StatefulWidget {
  const TrackTripsScreen({super.key});

  @override
  State<TrackTripsScreen> createState() => _TrackTripsScreenState();
}

class _TrackTripsScreenState extends State<TrackTripsScreen> {
  GoogleMapController? _mapController;
  BitmapDescriptor? _truckIcon;

  @override
  void initState() {
    super.initState();
    _createTruckMarkerIcon();
    WidgetsBinding.instance.addPostFrameCallback((_) {
      context.read<TripsBloc>().add(const TripsTrackTrips());
    });
  }

  @override
  void dispose() {
    _mapController?.dispose();
    super.dispose();
  }

  Future<void> _createTruckMarkerIcon() async {
    try {
      // Try to load truck icon from assets
      _truckIcon = await BitmapDescriptor.asset(
        const ImageConfiguration(size: Size(48, 48)),
        'assets/images/truck_fill.png',
      );
    } catch (e) {
      // Fallback: Use a better default marker or create improved truck icon
      // For now, use a blue marker until truck icon asset is added
      _truckIcon = BitmapDescriptor.defaultMarkerWithHue(
        BitmapDescriptor.hueBlue,
      );
    }
  }

  void _onMapCreated(GoogleMapController controller) {
    _mapController = controller;
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) {
          context.go('/home');
        }
      },
      child: CupertinoPageScaffold(
        backgroundColor: AppColors.mainGray.withValues(alpha: 1.0),
        navigationBar: const AppNavBar(),
        child: MainScaffold(
          child: Padding(
            padding: const EdgeInsets.symmetric(vertical: 5),
            child: ClipRRect(
              borderRadius: AppLayout.authContentBorderRadius,
              child: Container(
                color: AppColors.white,
                child: BlocBuilder<TripsBloc, TripsState>(
                  builder: (context, state) {
                    if (state is TrackTripsLoading) {
                      return const Center(
                        child: CupertinoActivityIndicator(
                          color: AppColors.mainBlue,
                          radius: 16,
                        ),
                      );
                    }

                    if (state is TrackTripsLoaded) {
                      if (state.error != null) {
                        return Center(
                          child: Padding(
                            padding: const EdgeInsets.all(24.0),
                            child: Column(
                              mainAxisAlignment: MainAxisAlignment.center,
                              children: [
                                const Icon(
                                  CupertinoIcons.exclamationmark_triangle,
                                  size: 64,
                                  color: AppColors.red,
                                ),
                                const SizedBox(height: 16),
                                Text(
                                  state.error!,
                                  style: const TextStyle(
                                    fontSize: 16,
                                    color: AppColors.darkGray,
                                  ),
                                  textAlign: TextAlign.center,
                                ),
                                const SizedBox(height: 24),
                                AppPrimaryButton(
                                  label: l10n.retry,
                                  onPressed: () => context
                                      .read<TripsBloc>()
                                      .add(const TripsTrackTrips()),
                                ),
                              ],
                            ),
                          ),
                        );
                      }

                      if (state.trackedTrips.isEmpty) {
                        return Padding(
                          padding: const EdgeInsets.symmetric(vertical: 5),
                          child: ClipRRect(
                            borderRadius: AppLayout.authContentBorderRadius,
                            child: Container(
                              color: AppColors.white,
                              child: Center(
                                child: Padding(
                                  padding: const EdgeInsets.all(24.0),
                                  child: Column(
                                    mainAxisAlignment: MainAxisAlignment.center,
                                    children: [
                                      const Icon(
                                        CupertinoIcons.doc_text,
                                        size: 64,
                                        color: AppColors.mediumBlueGray,
                                      ),
                                      const SizedBox(height: 16),
                                      Text(
                                        l10n.noTripsAvailable,
                                        style: const TextStyle(
                                          fontSize: 16,
                                          color: AppColors.darkGray,
                                        ),
                                        textAlign: TextAlign.center,
                                      ),
                                    ],
                                  ),
                                ),
                              ),
                            ),
                          ),
                        );
                      }

                      return GoogleMap(
                        onMapCreated: (controller) {
                          try {
                            _onMapCreated(controller);
                          } catch (e) {
                            // Error initializing map - handled gracefully
                          }
                        },
                        initialCameraPosition: const CameraPosition(
                          target: LatLng(30.0444, 31.2357), // Cairo default
                          zoom: 8,
                        ),
                        markers: state.trackedTrips
                            .where(
                              (trip) =>
                                  trip.currentLocation != null &&
                                  trip.currentLocation!.latitude != null &&
                                  trip.currentLocation!.longitude != null,
                            )
                            .map(
                              (trip) => Marker(
                                markerId: MarkerId(trip.id.toString()),
                                position: LatLng(
                                  trip.currentLocation!.latitude!,
                                  trip.currentLocation!.longitude!,
                                ),
                                icon:
                                    _truckIcon ??
                                    BitmapDescriptor.defaultMarkerWithHue(
                                      BitmapDescriptor.hueBlue,
                                    ),
                                onTap: () => showCupertinoModalPopup(
                                  context: context,
                                  builder: (context) =>
                                      TrackTripPopup(trip: trip),
                                ),
                              ),
                            )
                            .toSet(),
                        myLocationEnabled: true,
                        myLocationButtonEnabled: false,
                        mapType: MapType.normal,
                        zoomControlsEnabled: false,
                        compassEnabled: true,
                      );
                    }

                    return const SizedBox.shrink();
                  },
                ),
              ),
            ),
          ),
        ),
      ),
    );
  }
}
