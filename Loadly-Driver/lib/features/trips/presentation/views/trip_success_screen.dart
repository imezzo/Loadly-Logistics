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
import '../../domain/entities/trip.dart';
import '../bloc/trips_bloc.dart';
import '../bloc/trips_event.dart';
import '../bloc/trips_state.dart';
import '../widgets/trip_details/trip_details.dart';
import '../widgets/trip_step_documents_section.dart';

/// Full-screen success screen shown when a trip is completed.
/// Displays all trip details when [trip] is provided, using the same layout as Trip Details.
class TripSuccessScreen extends StatefulWidget {
  const TripSuccessScreen({super.key, this.trip});

  final Trip? trip;

  @override
  State<TripSuccessScreen> createState() => _TripSuccessScreenState();
}

class _TripSuccessScreenState extends State<TripSuccessScreen>
    with SingleTickerProviderStateMixin, WidgetsBindingObserver {
  late final TripDetailsMapDelegate _mapDelegate;
  late Object _mapKey;
  String? _currentUserLocationAddress;
  String? _startLocationAddress;
  int? _startLocationAddressTripId;
  String? _startLocationAddressLanguageCode;
  AnimationController? _lineAnimationController;
  Set<Marker> _mapMarkers = {};
  Set<Polyline> _mapPolylines = {};
  String? _lastSyncedMapSignature;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);
    _mapKey = Object();
    MapMarkerIcons.initialize().then((_) {
      if (mounted) setState(() {});
    });
    final trip = widget.trip;
    if (trip?.startLocation != null) {
      WidgetsBinding.instance.addPostFrameCallback((_) {
        _fetchStartLocationAddress(trip!);
      });
    }
    _lineAnimationController = AnimationController(
      vsync: this,
      duration: const Duration(milliseconds: 1500),
    )..repeat();
    _mapDelegate = TripDetailsMapDelegate(
      setState: (fn) => setState(fn),
      getMounted: () => mounted,
      includeTripStartRoute: true,
      onRefreshRequested: () => setState(() => _mapKey = Object()),
      onMapDataUpdated: (m, p) {
        if (mounted) {
          setState(() {
            _mapMarkers = m;
            _mapPolylines = p;
          });
        }
      },
    );
    _fetchCurrentUserLocation();
  }

  String _tripSignature(Trip trip) {
    final wp = trip.waypoints ?? [];
    final st = wp.map((w) => '${w.id}:${w.status}').join(',');
    final poly = trip.route?.polyline ?? '';
    return '${trip.id}|$st|${trip.statusCode}|$poly';
  }

  void _maybeSyncTripMap(Trip trip) {
    final sig = _tripSignature(trip);
    if (_lastSyncedMapSignature == sig) return;
    _lastSyncedMapSignature = sig;
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;
      _mapDelegate.updateMapWithTrip(trip);
    });
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
          _currentUserLocationAddress =
              address ??
              '${position.latitude.toStringAsFixed(6)}, ${position.longitude.toStringAsFixed(6)}';
        });
      }
    } catch (_) {
      if (mounted) {
        setState(() {
          _currentUserLocationAddress = null;
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
    final trip = widget.trip;
    if (trip != null) {
      _mapDelegate.updateMapWithTrip(trip);
    }
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (!mounted) return;
      _mapDelegate.retryFitCameraIfNeeded();
    });
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    if (state == AppLifecycleState.resumed) {
      _mapDelegate.refreshMapOnResume();
    }
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

  void _handleBackNavigation() => context.go('/trips');

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;
    final trip = widget.trip;

    return PopScope(
      canPop: false,
      onPopInvokedWithResult: (didPop, result) {
        if (!didPop) _handleBackNavigation();
      },
      child: BlocListener<TripsBloc, TripsState>(
        listenWhen: (_, curr) =>
            curr is TripDetailsLoaded && curr.documentUrlToOpen != null,
        listener: (context, state) async {
          if (state is! TripDetailsLoaded || state.documentUrlToOpen == null) {
            return;
          }
          final url = state.documentUrlToOpen!;
          context.read<TripsBloc>().add(const TripsClearDocumentUrl());
          if (context.mounted) {
            Navigator.of(context).push(
              CupertinoPageRoute(
                fullscreenDialog: true,
                builder: (_) => DocumentPreviewScreen(imageUrl: url),
              ),
            );
          }
        },
        child: CupertinoPageScaffold(
          backgroundColor: AppColors.mainBlue,
          navigationBar: CupertinoNavigationBar(
            backgroundColor: AppColors.mainBlue,
            brightness: Brightness.dark,
            leading: AppBackButton(onPressed: _handleBackNavigation),
            middle: Text(
              l10n.tripCompletedSuccessfully,
              style: const TextStyle(
                color: AppColors.white,
                fontSize: 18,
                fontWeight: FontWeight.w600,
              ),
            ),
            border: null,
          ),
          child: SafeArea(
            bottom: false,
            child: ClipRRect(
              borderRadius: AppLayout.authContentBorderRadius,
              child: Container(
                color: AppColors.white,
                child: trip != null
                    ? _buildTripContent(context, trip, l10n)
                    : _buildEmptyContent(context, l10n),
              ),
            ),
          ),
        ),
      ),
    );
  }

  Widget _buildEmptyContent(BuildContext context, AppLocalizations l10n) {
    return Column(
      mainAxisAlignment: MainAxisAlignment.center,
      children: [
        const Spacer(),
        Container(
          width: 80,
          height: 80,
          decoration: const BoxDecoration(
            shape: BoxShape.circle,
            color: AppColors.green,
          ),
          child: const Icon(
            CupertinoIcons.checkmark,
            size: 44,
            color: AppColors.white,
          ),
        ),
        const SizedBox(height: 20),
        Text(
          l10n.tripCompletedSuccessMessage,
          textAlign: TextAlign.center,
          style: const TextStyle(fontSize: 15, color: AppColors.mediumBlueGray),
        ),
        const Spacer(),
        Padding(
          padding: const EdgeInsets.fromLTRB(24, 16, 24, 24),
          child: AppButton(
            label: l10n.backToTrips,
            onPressed: () => context.go('/trips'),
          ),
        ),
      ],
    );
  }

  Widget _buildTripContent(
    BuildContext context,
    Trip trip,
    AppLocalizations l10n,
  ) {
    _maybeSyncTripMap(trip);
    return Column(
      crossAxisAlignment: CrossAxisAlignment.stretch,
      children: [
        _SuccessBanner(l10n: l10n),
        const SizedBox(height: 16),
        SizedBox(
          height: 300,
          child: TripDetailsMapSection(
            trip: trip,
            markers: _mapMarkers,
            polylines: _mapPolylines,
            mapKey: _mapKey,
            l10n: l10n,
            onMapCreated: _onMapCreated,
            onNavigateToWaypoint: (_, waypoint) {
              _mapDelegate.updateMapWithTrip(trip);
              _openNavigationToWaypoint(waypoint);
            },
          ),
        ),
        Expanded(
          child: CustomScrollView(
            slivers: [
              SliverToBoxAdapter(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    TripIdStatusBar(trip: trip, l10n: l10n),
                    TripDetailsLocationsSection(
                      trip: trip,
                      currentUserLocationAddress: _currentUserLocationAddress,
                      lineAnimationController: _lineAnimationController,
                      startLocationAddress: _startLocationAddress,
                    ),
                    if (trip.shipmentDetails != null)
                      TripShipmentDetailsCard(
                        shipment: trip.shipmentDetails!,
                        l10n: l10n,
                      ),
                    if (trip.vehicleDetails != null)
                      TripVehicleDetailsCard(
                        vehicleDetails: trip.vehicleDetails!,
                        l10n: l10n,
                      ),
                    TripStepDocumentsSection(trip: trip),
                    if (trip.clientDetails != null)
                      TripClientDetailsCard(
                        clientDetails: trip.clientDetails!,
                        l10n: l10n,
                      ),
                    _DocumentsCard(trip: trip, l10n: l10n),
                    const SizedBox(height: 24),
                    Padding(
                      padding: const EdgeInsets.fromLTRB(24, 0, 24, 24),
                      child: AppButton(
                        label: l10n.backToTrips,
                        onPressed: () => context.go('/trips'),
                      ),
                    ),
                  ],
                ),
              ),
            ],
          ),
        ),
      ],
    );
  }
}

class _SuccessBanner extends StatelessWidget {
  const _SuccessBanner({required this.l10n});

  final AppLocalizations l10n;

  @override
  Widget build(BuildContext context) {
    return Container(
      margin: const EdgeInsets.fromLTRB(16, 16, 16, 0),
      padding: const EdgeInsets.all(16),
      decoration: BoxDecoration(
        color: AppColors.green.withValues(alpha: 0.1),
        borderRadius: BorderRadius.circular(12),
        border: Border.all(color: AppColors.green.withValues(alpha: 0.3)),
      ),
      child: Row(
        children: [
          Container(
            width: 48,
            height: 48,
            decoration: const BoxDecoration(
              shape: BoxShape.circle,
              color: AppColors.green,
            ),
            child: const Icon(
              CupertinoIcons.checkmark,
              size: 28,
              color: AppColors.white,
            ),
          ),
          const SizedBox(width: 16),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  l10n.tripCompletedSuccessfully,
                  style: const TextStyle(
                    fontSize: 16,
                    fontWeight: FontWeight.w600,
                    color: AppColors.darkGray,
                  ),
                ),
                const SizedBox(height: 4),
                Text(
                  l10n.tripCompletedSuccessMessage,
                  style: const TextStyle(
                    fontSize: 14,
                    color: AppColors.mediumBlueGray,
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

class _DocumentsCard extends StatelessWidget {
  const _DocumentsCard({required this.trip, required this.l10n});

  final Trip trip;
  final AppLocalizations l10n;

  @override
  Widget build(BuildContext context) {
    final docs = trip.documents ?? [];
    if (docs.isEmpty) return const SizedBox.shrink();

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
            l10n.documents,
            style: const TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.w600,
              color: AppColors.darkGray,
            ),
          ),
          const SizedBox(height: 12),
          ...docs.expand((doc) {
            final paths = doc.paths ?? [];
            if (paths.isEmpty) return <Widget>[];
            return paths.map((path) {
              return Padding(
                padding: const EdgeInsets.only(bottom: 8),
                child: CupertinoButton(
                  padding: EdgeInsets.zero,
                  alignment: Alignment.centerLeft,
                  onPressed: () {
                    context.read<TripsBloc>().add(
                      TripsOpenDocument(tripId: trip.id, documentPath: path),
                    );
                  },
                  child: Row(
                    children: [
                      const Icon(
                        CupertinoIcons.doc_fill,
                        size: 20,
                        color: AppColors.mainBlue,
                      ),
                      const SizedBox(width: 10),
                      Expanded(
                        child: Text(
                          (doc.typeName != null &&
                                  doc.typeName!.trim().isNotEmpty)
                              ? doc.typeName!.trim()
                              : doc.type,
                          style: const TextStyle(
                            fontSize: 14,
                            fontWeight: FontWeight.w500,
                            color: AppColors.darkGray,
                          ),
                        ),
                      ),
                      const Icon(
                        CupertinoIcons.chevron_right,
                        size: 16,
                        color: AppColors.mediumBlueGray,
                      ),
                    ],
                  ),
                ),
              );
            });
          }),
        ],
      ),
    );
  }
}
