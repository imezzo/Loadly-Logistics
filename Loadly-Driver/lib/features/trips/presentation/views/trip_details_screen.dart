import 'dart:io' show Platform;

import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:geolocator/geolocator.dart';
import 'package:go_router/go_router.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';
import 'package:url_launcher/url_launcher.dart';

import '../../../../core/utils/google_reverse_geocoding.dart';
import '../../../../core/utils/map_marker_icons.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/app_back_button.dart';
import '../../../../core/widgets/app_button.dart';
import '../../../../core/widgets/document_preview_screen.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../core/trip_error_localizer.dart';
import '../../domain/entities/trip.dart';
import '../bloc/trips_bloc.dart';
import '../bloc/trips_event.dart';
import '../bloc/trips_state.dart';
import '../widgets/trip_action_buttons.dart';
import '../widgets/trip_details/trip_details.dart';
import '../widgets/trip_step_documents_section.dart';

/// Trip details screen showing comprehensive trip information
class TripDetailsScreen extends StatefulWidget {
  const TripDetailsScreen({super.key, required this.tripId, this.returnRoute});

  final int tripId;

  /// When set (e.g. from pushReplacement), back navigates here instead of pop.
  final String? returnRoute;

  @override
  State<TripDetailsScreen> createState() => _TripDetailsScreenState();
}

class _TripDetailsScreenState extends State<TripDetailsScreen>
    with SingleTickerProviderStateMixin, WidgetsBindingObserver {
  late final TripDetailsMapDelegate _mapDelegate;
  late Object _mapKey;
  bool _mapVisible = true;
  bool _cameraSessionActive = false;
  bool _recoveringTripDetails = false;
  String? _startLocationAddress;
  int? _startLocationAddressTripId;
  String? _startLocationAddressLanguageCode;
  AnimationController? _lineAnimationController;
  final ValueNotifier<String?> _currentUserLocationAddressNotifier =
      ValueNotifier(null);

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);
    _mapKey = Object();
    MapMarkerIcons.initialize().then((_) {
      if (mounted) setState(() {});
    });
    _lineAnimationController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 1500),
    )..repeat();
    _mapDelegate = TripDetailsMapDelegate(
      setState: (fn) => setState(fn),
      getMounted: () => mounted,
      onRefreshRequested: () => setState(() => _mapKey = Object()),
    );
    // Mark as recovering to avoid scheduling a duplicate load from the
    // builder while we dispatch the initial request here.
    _recoveringTripDetails = true;
    context.read<TripsBloc>().add(TripsLoadTripDetails(tripId: widget.tripId));
    _fetchCurrentUserLocation();
  }

  Future<void> _fetchCurrentUserLocation() async {
    try {
      final position = await Geolocator.getCurrentPosition(
        locationSettings: const LocationSettings(
          accuracy: LocationAccuracy.medium,
        ),
      );
      if (!mounted) return;
      final address = await reverseGeocodeLatLng(
        latitude: position.latitude,
        longitude: position.longitude,
        languageCode: Localizations.localeOf(context).languageCode,
      );
      if (mounted) {
        setState(() {
          _currentUserLocationAddressNotifier.value =
              address ??
              '${position.latitude.toStringAsFixed(6)}, ${position.longitude.toStringAsFixed(6)}';
        });
      }
    } catch (_) {
      if (mounted) {
        setState(() {
          _currentUserLocationAddressNotifier.value = null;
        });
      }
    }
  }

  Future<void> _fetchStartLocationAddress(Trip trip) async {
    final sl = trip.startLocation;
    if (sl == null) {
      if (_startLocationAddressTripId == trip.id) {
        setState(() {
          _startLocationAddress = null;
          _startLocationAddressTripId = null;
          _startLocationAddressLanguageCode = null;
        });
      }
      return;
    }
    final languageCode = Localizations.localeOf(context).languageCode;
    if (_startLocationAddressTripId == trip.id &&
        _startLocationAddress != null &&
        _startLocationAddressLanguageCode == languageCode) {
      return;
    }
    _startLocationAddressTripId = trip.id;
    _startLocationAddressLanguageCode = languageCode;
    try {
      final address = await reverseGeocodeLatLng(
        latitude: sl.latitude,
        longitude: sl.longitude,
        languageCode: languageCode,
      );
      if (mounted && _startLocationAddressTripId == trip.id) {
        setState(() {
          _startLocationAddress =
              address ??
              '${sl.latitude.toStringAsFixed(6)}, ${sl.longitude.toStringAsFixed(6)}';
        });
      }
    } catch (_) {
      if (mounted && _startLocationAddressTripId == trip.id) {
        setState(() {
          _startLocationAddress =
              '${sl.latitude.toStringAsFixed(6)}, ${sl.longitude.toStringAsFixed(6)}';
        });
      }
    }
  }

  @override
  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
    _lineAnimationController?.dispose();
    _lineAnimationController = null;
    final controller = _mapDelegate.controller;
    _mapDelegate.controller = null;
    if (controller != null) {
      Future.microtask(() => controller.dispose());
    }
    super.dispose();
  }

  void _onMapCreated(GoogleMapController controller) {
    _mapDelegate.controller = controller;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;
      _mapDelegate.retryFitCameraIfNeeded();
    });
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    if (state == AppLifecycleState.paused && Platform.isAndroid) {
      if (_mapVisible) setState(() => _mapVisible = false);
    } else if (state == AppLifecycleState.resumed) {
      if (_cameraSessionActive) return;
      if (!_mapVisible && Platform.isAndroid) {
        setState(() {
          _mapKey = Object();
          _mapVisible = true;
        });
      }
      _mapDelegate.refreshMapOnResume();
    }
  }

  Future<void> _prepareMapForCamera() async {
    _cameraSessionActive = true;
    if (!mounted || !_mapVisible) return;
    setState(() => _mapVisible = false);
    await WidgetsBinding.instance.endOfFrame;
  }

  void _restoreMapAfterCamera() {
    _cameraSessionActive = false;
    if (!mounted) return;
    setState(() {
      _mapKey = Object();
      _mapVisible = true;
    });
  }

  Future<void> _openNavigationToWaypoint(TripWaypoint waypoint) async {
    final lat = waypoint.latitude!;
    final lng = waypoint.longitude!;
    final Uri uri = Platform.isIOS
        ? Uri.parse('https://maps.apple.com/?dirflg=d&daddr=$lat,$lng')
        : Uri.parse(
            'https://www.google.com/maps/dir/?api=1&destination=$lat,$lng',
          );
    try {
      await launchUrl(uri, mode: LaunchMode.externalApplication);
    } catch (_) {
      if (!Platform.isIOS) {
        final geoUri = Uri.parse('geo:$lat,$lng');
        await launchUrl(geoUri, mode: LaunchMode.externalApplication);
      }
    }
  }

  Future<void> _callSupport() async {
    showCupertinoDialog(
      context: context,
      builder: (context) => CupertinoAlertDialog(
        title: Text(AppLocalizations.of(context)!.support),
        content: Text(
          '${AppLocalizations.of(context)!.callSupport}: \u200E+201234567890',
        ),
        actions: [
          CupertinoDialogAction(
            child: Text(AppLocalizations.of(context)!.ok),
            onPressed: () => Navigator.of(context).pop(),
          ),
        ],
      ),
    );
  }

  void _handleBackNavigation() {
    final returnRoute = widget.returnRoute;
    if (returnRoute != null && returnRoute.isNotEmpty) {
      context.go(returnRoute);
    } else if (context.canPop()) {
      context.pop();
    } else {
      context.go('/trips');
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) _handleBackNavigation();
      },
      child: CupertinoPageScaffold(
        backgroundColor: AppColors.mainBlue,
        navigationBar: CupertinoNavigationBar(
          backgroundColor: AppColors.mainBlue,
          brightness: Brightness.dark,
          leading: Row(
            children: [
              AppBackButton(onPressed: _handleBackNavigation),
              const SizedBox(width: 10),
              Text(
                l10n.tripDetails,
                style: const TextStyle(
                  color: AppColors.white,
                  fontSize: 18,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ],
          ),
          border: null,
        ),
        child: SafeArea(
          bottom: false,
          child: ClipRRect(
            borderRadius: AppLayout.authContentBorderRadius,
            child: Container(
              margin: const EdgeInsets.only(top: 5),
              color: AppColors.white,
              child: BlocConsumer<TripsBloc, TripsState>(
                listenWhen: (prev, curr) {
                  if (curr is TripDetailsLoaded &&
                      curr.documentUrlToOpen != null) {
                    return true;
                  }
                  if (curr is TripDetailsLoaded && curr.tripJustCompleted) {
                    return true;
                  }
                  if (curr is TripDetailsLoaded &&
                      curr.error != null &&
                      curr.trip.id != 0 &&
                      prev is TripActionInProgress) {
                    return true;
                  }
                  return false;
                },
                listener: (context, state) async {
                  if (state is TripDetailsLoaded && state.tripJustCompleted) {
                    if (context.mounted) {
                      context.go('/trips/success', extra: {'trip': state.trip});
                    }
                    return;
                  }
                  if (state is TripDetailsLoaded &&
                      state.documentUrlToOpen != null &&
                      state.documentUrlToOpen!.isNotEmpty) {
                    final url = state.documentUrlToOpen!;
                    if (context.mounted) {
                      context.read<TripsBloc>().add(
                        const TripsClearDocumentUrl(),
                      );
                      Navigator.of(context).push(
                        CupertinoPageRoute(
                          fullscreenDialog: true,
                          builder: (_) => DocumentPreviewScreen(imageUrl: url),
                        ),
                      );
                    }
                  }
                  if (state is TripDetailsLoaded &&
                      state.error != null &&
                      state.trip.id != 0 &&
                      context.mounted) {
                    final msg = localizeTripError(
                      state.error,
                      AppLocalizations.of(context)!,
                    );
                    showCupertinoDialog<void>(
                      context: context,
                      builder: (ctx) => CupertinoAlertDialog(
                        title: Text(l10n.error),
                        content: Text(msg),
                        actions: [
                          CupertinoDialogAction(
                            onPressed: () => Navigator.of(ctx).pop(),
                            child: Text(l10n.ok),
                          ),
                        ],
                      ),
                    );
                  }
                },
                buildWhen: (prev, curr) {
                  if (curr is! TripDetailsLoaded) return true;
                  if (prev is! TripDetailsLoaded) return true;
                  return curr.documentUrlToOpen == prev.documentUrlToOpen;
                },
                builder: (context, state) {
                  if (state is TripDetailsLoaded ||
                      state is TripActionInProgress) {
                    _recoveringTripDetails = false;
                  }
                  if (state is TripDetailsLoading) {
                    return const Center(
                      child: CupertinoActivityIndicator(
                        color: AppColors.mainBlue,
                        radius: 16,
                      ),
                    );
                  }

                  if (state is TripActionInProgress) {
                    final trip = state.trip;
                    WidgetsBinding.instance.addPostFrameCallback((_) {
                      if (!mounted ||
                          !(ModalRoute.of(context)?.isCurrent ?? false)) {
                        return;
                      }
                      _fetchStartLocationAddress(trip);
                      if (!_mapDelegate.isUpdating) {
                        _mapDelegate.updateMapWithTrip(trip);
                      }
                    });
                    return _buildTripContent(context, trip, l10n);
                  }

                  if (state is TripDetailsLoaded) {
                    if (state.error != null && state.trip.id == 0) {
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
                                localizeTripError(state.error, l10n),
                                style: const TextStyle(
                                  fontSize: 16,
                                  color: AppColors.darkGray,
                                ),
                                textAlign: TextAlign.center,
                              ),
                              const SizedBox(height: 24),
                              AppButton(
                                label: l10n.retry,
                                onPressed: () {
                                  context.read<TripsBloc>().add(
                                    TripsLoadTripDetails(tripId: widget.tripId),
                                  );
                                },
                              ),
                            ],
                          ),
                        ),
                      );
                    }

                    final trip = state.trip;
                    WidgetsBinding.instance.addPostFrameCallback((_) {
                      _fetchStartLocationAddress(trip);
                      if (!_mapDelegate.isUpdating) {
                        _mapDelegate.updateMapWithTrip(trip);
                      }
                    });
                    return _buildTripContent(context, trip, l10n);
                  }

                  if (!_recoveringTripDetails &&
                      state is! TripDetailsLoading &&
                      state is! TripDetailsLoaded &&
                      (ModalRoute.of(context)?.isCurrent ?? false)) {
                    _recoveringTripDetails = true;
                    WidgetsBinding.instance.addPostFrameCallback((_) {
                      if (!mounted ||
                          !(ModalRoute.of(context)?.isCurrent ?? false)) {
                        return;
                      }
                      context.read<TripsBloc>().add(
                        TripsLoadTripDetails(tripId: widget.tripId),
                      );
                    });
                  }
                  return const Center(
                    child: CupertinoActivityIndicator(
                      color: AppColors.mainBlue,
                      radius: 16,
                    ),
                  );
                },
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildTripContent(
    BuildContext context,
    Trip trip,
    AppLocalizations l10n,
  ) {
    return CustomScrollView(
      slivers: [
        CupertinoSliverRefreshControl(
          onRefresh: () async {
            context.read<TripsBloc>().add(
              TripsLoadTripDetails(tripId: widget.tripId),
            );
            await Future.delayed(const Duration(milliseconds: 500));
          },
        ),
        SliverToBoxAdapter(
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              TripIdStatusBar(trip: trip, l10n: l10n),
              if (_mapVisible)
                TripDetailsMapSection(
                  trip: trip,
                  markers: _mapDelegate.markers,
                  polylines: _mapDelegate.polylines,
                  mapKey: _mapKey,
                  l10n: l10n,
                  onMapCreated: _onMapCreated,
                  onNavigateToWaypoint: (trip, waypoint) {
                    _mapDelegate.updateMapWithTrip(trip);
                    _openNavigationToWaypoint(waypoint);
                  },
                )
              else
                Container(
                  height: 300,
                  margin: const EdgeInsets.symmetric(horizontal: 16),
                  decoration: BoxDecoration(
                    color: AppColors.lightGray,
                    borderRadius: BorderRadius.circular(12),
                  ),
                  child: const Center(child: CupertinoActivityIndicator()),
                ),
              ValueListenableBuilder<String?>(
                valueListenable: _currentUserLocationAddressNotifier,
                builder: (context, address, _) {
                  return TripDetailsLocationsSection(
                    trip: trip,
                    currentUserLocationAddress: address,
                    lineAnimationController: _lineAnimationController,
                    startLocationAddress: _startLocationAddress,
                  );
                },
              ),
              if (trip.shipmentDetails != null)
                TripShipmentDetailsCard(
                  shipment: trip.shipmentDetails!,
                  l10n: l10n,
                ),
              if (trip.statusCode == kTripScheduled &&
                  trip.vehicleDetails != null)
                TripVehicleDetailsCard(
                  vehicleDetails: trip.vehicleDetails!,
                  l10n: l10n,
                ),
              TripStepDocumentsSection(
                trip: trip,
                onPrepareForCamera: _prepareMapForCamera,
                onCameraFinished: _restoreMapAfterCamera,
              ),
              if (trip.statusCode == kTripScheduled &&
                  trip.clientDetails != null)
                TripClientDetailsCard(
                  clientDetails: trip.clientDetails!,
                  l10n: l10n,
                ),
              if ((trip.customsStatus == 1 || trip.customsStatus == 2) &&
                  trip.statusCode != 2)
                Container(
                  padding: const EdgeInsets.symmetric(horizontal: 16),
                  width: double.infinity,
                  child: AppButton(
                    label: trip.customsStatus == 1
                        ? '${l10n.enterCustoms}${l10n.questionMark}'
                        : '${l10n.exitCustoms}${l10n.questionMark}',
                    onPressed: () => context.go(
                      '/trips/${widget.tripId}/customs',
                      extra: trip,
                    ),
                    color: trip.customsStatus == 1
                        ? AppColors.orange
                        : AppColors.green,
                  ),
                ),
              TripActionButtons(trip: trip, onCallSupport: _callSupport),
            ],
          ),
        ),
      ],
    );
  }
}
