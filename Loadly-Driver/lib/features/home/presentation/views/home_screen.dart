import 'dart:async';
import 'dart:io' show Platform;

import 'package:firebase_messaging/firebase_messaging.dart';
import 'package:flutter/cupertino.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:go_router/go_router.dart';
import 'package:google_maps_flutter/google_maps_flutter.dart';

import '../../../../core/services/fcm_service.dart';
import '../../../../core/services/local_notification_service.dart';
import '../../../../core/services/notification_launch_handler.dart';
import '../../../../core/utils/fcm_notification_logger.dart';
import '../../../../core/theme/app_colors.dart';
import '../../../../core/theme/app_layout.dart';
import '../../../../core/widgets/main_scaffold.dart';
import '../../../../l10n/gen/app_localizations.dart';
import '../../../auth/presentation/bloc/auth_bloc.dart';
import '../../../auth/presentation/bloc/auth_state.dart';
import '../../../trips/domain/entities/trip.dart';
import '../../../trips/presentation/bloc/trips_bloc.dart';
import '../../../trips/presentation/bloc/trips_event.dart';
import '../../../trips/presentation/bloc/trips_state.dart';
import '../../../trips/presentation/widgets/trip_card.dart';
import '../bloc/home_bloc.dart';
import '../bloc/home_event.dart';
import '../bloc/home_state.dart';
import '../home_location_messages.dart';
import '../widgets/location_prominent_disclosure_page.dart';
import '../widgets/trip_schedule_update_card.dart';

/// Home screen with map showing live location and swipe button for online/offline
class HomeScreen extends StatefulWidget {
  const HomeScreen({super.key});

  @override
  State<HomeScreen> createState() => _HomeScreenState();
}

class _HomeScreenState extends State<HomeScreen> with WidgetsBindingObserver {
  GoogleMapController? _mapController;
  final Set<Marker> _markers = {};
  bool _hasRequestedLocation = false;
  BitmapDescriptor? _truckIcon;
  StreamSubscription? _scheduleUpdateSubscription;
  late Object _mapKey;
  bool _locationDisclosureRouteOpen = false;
  bool _backgroundLocationDisclosureRouteOpen = false;
  bool _androidNotificationDisclosureStarted = false;

  @override
  void initState() {
    super.initState();
    WidgetsBinding.instance.addObserver(this);
    _mapKey = Object();
    _createTruckMarkerIcon();
    // Check for pending trip schedule update (e.g. from notification tap)
    final pending = LocalNotificationService.instance.pendingUpdate;
    if (pending != null) {
      context.read<HomeBloc>().add(HomeTripScheduleUpdateReceived(pending));
      LocalNotificationService.instance.clearPendingUpdate();
    }
    // Listen for trip schedule updates from push notifications
    _scheduleUpdateSubscription = LocalNotificationService
        .instance
        .scheduleUpdateStream
        .listen((update) {
          if (mounted) {
            context.read<HomeBloc>().add(
              HomeTripScheduleUpdateReceived(update),
            );
          }
        });
    // Request initial location when screen is shown
    WidgetsBinding.instance.addPostFrameCallback((_) {
      if (mounted && !_hasRequestedLocation) {
        _hasRequestedLocation = true;
        context.read<HomeBloc>().add(const HomeGetInitialLocation());
      }
      _scheduleAndroidNotificationDisclosureIfNeeded();
    });
    // Load current trips for display
    context.read<TripsBloc>().add(
      const TripsLoadTrips(page: 1, limit: 10, status: null),
    );
    // Open trip from notification tap after splash / login
    FcmNotificationLogger.logStep(
      step: 'home_check_pending_route',
      source: 'homeScreen',
      route: NotificationLaunchHandler.pendingRoute,
      detail: 'calling navigateIfPending',
    );
    NotificationLaunchHandler.navigateIfPending(isLoggedIn: true);
  }

  @override
  void dispose() {
    WidgetsBinding.instance.removeObserver(this);
    _scheduleUpdateSubscription?.cancel();
    // Defer disposal to avoid "Shutting down renderer while it's not idle" when
    // navigating away before the map has finished initializing.
    final controller = _mapController;
    _mapController = null;
    if (controller != null) {
      Future.microtask(() => controller.dispose());
    }
    super.dispose();
  }

  Future<void> _createTruckMarkerIcon() async {
    try {
      // Try to load truck icon from assets
      _truckIcon = await BitmapDescriptor.asset(
        const ImageConfiguration(size: Size(48, 48)),
        'assets/images/truck_icon.png',
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

  void _updateMapCamera(double latitude, double longitude) {
    _mapController?.animateCamera(
      CameraUpdate.newLatLng(LatLng(latitude, longitude)),
    );
  }

  @override
  void didChangeAppLifecycleState(AppLifecycleState state) {
    if (state == AppLifecycleState.resumed) {
      _refreshMapOnResume();
      if (!mounted) return;
      final homeState = context.read<HomeBloc>().state;
      if (homeState is HomeLoaded && homeState.shouldRetryLocationOnResume) {
        context.read<HomeBloc>().add(const HomeGetInitialLocation());
      }
    }
  }

  void _refreshMapOnResume() {
    if (!mounted) return;
    // Force map rebuild to fix blank/blue screen when returning from background.
    // Previously used setMapStyle('[]') which is deprecated; use widget rebuild instead.
    setState(() {
      _mapKey = Object();
    });
  }

  Future<void> _scheduleAndroidNotificationDisclosureIfNeeded() async {
    if (!Platform.isAndroid || _androidNotificationDisclosureStarted) return;
    _androidNotificationDisclosureStarted = true;
    for (var i = 0; i < 30; i++) {
      await Future<void>.delayed(const Duration(milliseconds: 200));
      if (!mounted) return;
      final hs = context.read<HomeBloc>().state;
      if (hs is HomeLoaded &&
          hs.pendingLocationPermissionDisclosure == null &&
          hs.pendingBackgroundLocationDisclosure == null) {
        break;
      }
    }
    if (!mounted) return;
    final settings = await FirebaseMessaging.instance.getNotificationSettings();
    if (!mounted) return;
    if (settings.authorizationStatus == AuthorizationStatus.authorized ||
        settings.authorizationStatus == AuthorizationStatus.provisional) {
      await FCMService().sendTokenToBackend();
      return;
    }
    final l10n = AppLocalizations.of(context)!;
    final consent = await showCupertinoDialog<bool>(
      context: context,
      barrierDismissible: false,
      builder: (BuildContext ctx) => CupertinoAlertDialog(
        title: Text(l10n.notificationPermissionDisclosureTitle),
        content: Padding(
          padding: const EdgeInsets.only(top: 8),
          child: Text(l10n.notificationPermissionDisclosureBody),
        ),
        actions: <CupertinoDialogAction>[
          CupertinoDialogAction(
            onPressed: () => Navigator.pop(ctx, false),
            child: Text(l10n.prominentDisclosureNotNow),
          ),
          CupertinoDialogAction(
            isDefaultAction: true,
            onPressed: () => Navigator.pop(ctx, true),
            child: Text(l10n.prominentDisclosureContinue),
          ),
        ],
      ),
    );
    if (consent != true || !mounted) return;
    await FCMService().requestRuntimeNotificationPermission();
    if (mounted) await FCMService().sendTokenToBackend();
  }

  Future<void> _showLocationPermissionDisclosure() async {
    if (_locationDisclosureRouteOpen || !mounted) return;
    _locationDisclosureRouteOpen = true;
    final l10n = AppLocalizations.of(context)!;
    final homeBloc = context.read<HomeBloc>();
    await Navigator.of(context, rootNavigator: true).push<void>(
      CupertinoPageRoute<void>(
        fullscreenDialog: true,
        builder: (BuildContext ctx) => LocationProminentDisclosurePage(
          title: l10n.locationPermissionDisclosureTitle,
          body: l10n.locationPermissionDisclosureBody,
          continueLabel: l10n.prominentDisclosureContinue,
          notNowLabel: l10n.prominentDisclosureNotNow,
          onContinue: () {
            Navigator.of(ctx).pop();
            homeBloc.add(const HomeContinueLocationPermissionAfterDisclosure());
          },
          onNotNow: () {
            Navigator.of(ctx).pop();
            homeBloc.add(const HomeDismissLocationPermissionDisclosure());
          },
        ),
      ),
    );
    if (mounted) {
      _locationDisclosureRouteOpen = false;
    }
  }

  Future<void> _showBackgroundLocationPermissionDisclosure() async {
    if (_backgroundLocationDisclosureRouteOpen || !mounted) return;
    _backgroundLocationDisclosureRouteOpen = true;
    final l10n = AppLocalizations.of(context)!;
    final homeBloc = context.read<HomeBloc>();
    await Navigator.of(context, rootNavigator: true).push<void>(
      CupertinoPageRoute<void>(
        fullscreenDialog: true,
        builder: (BuildContext ctx) => LocationProminentDisclosurePage(
          title: l10n.locationBackgroundPermissionDisclosureTitle,
          body: l10n.locationBackgroundPermissionDisclosureBody,
          continueLabel: l10n.prominentDisclosureContinue,
          notNowLabel: l10n.prominentDisclosureNotNow,
          onContinue: () {
            Navigator.of(ctx).pop();
            homeBloc.add(const HomeContinueBackgroundLocationDisclosure());
          },
          onNotNow: () {
            Navigator.of(ctx).pop();
            homeBloc.add(const HomeDismissBackgroundLocationDisclosure());
          },
        ),
      ),
    );
    if (mounted) {
      _backgroundLocationDisclosureRouteOpen = false;
    }
  }

  void _updateMarker(double latitude, double longitude) {
    setState(() {
      _markers.clear();
      _markers.add(
        Marker(
          markerId: const MarkerId('current_location'),
          position: LatLng(latitude, longitude),
          icon:
              _truckIcon ??
              BitmapDescriptor.defaultMarkerWithHue(BitmapDescriptor.hueBlue),
          infoWindow: const InfoWindow(title: 'Your Location'),
          anchor: const Offset(0.5, 0.5),
        ),
      );
    });
  }

  String localizeSuccessMessage(String message, AppLocalizations l10n) {
    switch (message) {
      case 'Successfully went online':
        return l10n.successfullyWentOnline;
      case 'Successfully went offline':
        return l10n.successfullyWentOffline;
      default:
        return message;
    }
  }

  @override
  Widget build(BuildContext context) {
    final l10n = AppLocalizations.of(context)!;

    return CupertinoPageScaffold(
      backgroundColor: AppColors.mainBlue,
      navigationBar: CupertinoNavigationBar(
        backgroundColor: AppColors.mainBlue,
        brightness: Brightness.dark,
        leading: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            Image.asset(
              'assets/images/user_icon.png',
              width: 32,
              height: 32,
              color: AppColors.white,
            ),
            const SizedBox(width: 8),
            Text(
              l10n.home,
              style: const TextStyle(
                color: AppColors.white,
                fontWeight: FontWeight.w600,
                fontSize: 17,
              ),
            ),
          ],
        ),
      ),
      child: MultiBlocListener(
        listeners: [
          BlocListener<AuthBloc, AuthState>(
            listener: (context, authState) {
              // Navigate to login when logout succeeds
              if (authState is AuthUnauthenticated) {
                context.go('/login');
              }
            },
          ),
          BlocListener<HomeBloc, HomeState>(
            listenWhen: (prev, curr) {
              if (curr is! HomeLoaded) return false;
              if (curr.pendingLocationPermissionDisclosure == null) {
                return false;
              }
              if (prev is! HomeLoaded) return true;
              return prev.pendingLocationPermissionDisclosure !=
                  curr.pendingLocationPermissionDisclosure;
            },
            listener: (context, state) {
              if (state is HomeLoaded &&
                  state.pendingLocationPermissionDisclosure != null) {
                _showLocationPermissionDisclosure();
              }
            },
          ),
          BlocListener<HomeBloc, HomeState>(
            listenWhen: (prev, curr) {
              if (curr is! HomeLoaded) return false;
              if (curr.pendingBackgroundLocationDisclosure == null) {
                return false;
              }
              if (prev is! HomeLoaded) return true;
              return prev.pendingBackgroundLocationDisclosure !=
                  curr.pendingBackgroundLocationDisclosure;
            },
            listener: (context, state) {
              if (state is HomeLoaded &&
                  state.pendingBackgroundLocationDisclosure != null) {
                _showBackgroundLocationPermissionDisclosure();
              }
            },
          ),
          BlocListener<HomeBloc, HomeState>(
            listenWhen: (prev, curr) {
              if (curr is! HomeLoaded) return false;
              return curr.latitude != null &&
                  curr.longitude != null &&
                  (prev is! HomeLoaded ||
                      prev.latitude != curr.latitude ||
                      prev.longitude != curr.longitude);
            },
            listener: (context, state) {
              if (state is HomeLoaded &&
                  state.latitude != null &&
                  state.longitude != null) {
                _updateMapCamera(state.latitude!, state.longitude!);
                _updateMarker(state.latitude!, state.longitude!);
              }
            },
          ),
        ],
        child: MainScaffold(
          child: BlocBuilder<HomeBloc, HomeState>(
            builder: (context, state) {
              if (state is HomeLoaded) {
                final locationBannerText = state.locationIssue != null
                    ? messageForHomeLocationIssue(l10n, state.locationIssue!)
                    : state.locationError;
                return Stack(
                  children: [
                    // Map with rounded corners
                    ClipRRect(
                      borderRadius: AppLayout.authContentBorderRadius,
                      child: Container(
                        color: AppColors.mainBlue,
                        child: GoogleMap(
                          key: ValueKey(_mapKey),
                          style: '[]',
                          onMapCreated: (controller) {
                            try {
                              _onMapCreated(controller);
                            } catch (e) {
                              // Error initializing map - handled gracefully
                            }
                          },
                          initialCameraPosition: CameraPosition(
                            target:
                                state.latitude != null &&
                                    state.longitude != null
                                ? LatLng(state.latitude!, state.longitude!)
                                : const LatLng(
                                    30.0444,
                                    31.2357,
                                  ), // Cairo default
                            zoom: 15.0,
                          ),
                          markers: _markers,
                          myLocationEnabled: true,
                          myLocationButtonEnabled: false,
                          mapType: MapType.normal,
                          zoomControlsEnabled: false,
                          compassEnabled: true,
                        ),
                      ),
                    ),
                    // Status indicator at the top
                    Positioned(
                      top: 0,
                      left: 0,
                      right: 0,
                      child: SafeArea(
                        bottom: false,
                        child: Padding(
                          padding: const EdgeInsets.only(top: 16),
                          child: Center(
                            child: AnimatedContainer(
                              duration: const Duration(milliseconds: 300),
                              curve: Curves.easeInOut,
                              padding: const EdgeInsets.symmetric(
                                horizontal: 20,
                                vertical: 12,
                              ),
                              decoration: BoxDecoration(
                                color: state.isOnline
                                    ? AppColors.lightGray
                                    : AppColors.darkGray,
                                borderRadius: BorderRadius.circular(24),
                                boxShadow: [
                                  BoxShadow(
                                    color: state.isOnline
                                        ? AppColors.lightGray.withValues(
                                            alpha: 0.3,
                                          )
                                        : AppColors.darkGray.withValues(
                                            alpha: 0.3,
                                          ),
                                    blurRadius: 12,
                                    offset: const Offset(0, 4),
                                  ),
                                ],
                              ),
                              child: Row(
                                mainAxisSize: MainAxisSize.min,
                                children: [
                                  // Animated status dot
                                  AnimatedContainer(
                                    duration: const Duration(milliseconds: 300),
                                    width: 12,
                                    height: 12,
                                    decoration: BoxDecoration(
                                      color: state.isOnline
                                          ? AppColors.green
                                          : AppColors.red,
                                      shape: BoxShape.circle,
                                      boxShadow: state.isOnline
                                          ? [
                                              BoxShadow(
                                                color: AppColors.green
                                                    .withValues(alpha: 0.6),
                                                blurRadius: 8,
                                                spreadRadius: 2,
                                              ),
                                            ]
                                          : [
                                              BoxShadow(
                                                color: AppColors.red.withValues(
                                                  alpha: 0.6,
                                                ),
                                                blurRadius: 8,
                                                spreadRadius: 2,
                                              ),
                                            ],
                                    ),
                                  ),
                                  const SizedBox(width: 10),
                                  Text(
                                    state.isOnline ? l10n.online : l10n.offline,
                                    style: CupertinoTheme.of(context)
                                        .textTheme
                                        .textStyle
                                        .copyWith(
                                          color: state.isOnline
                                              ? AppColors.darkGray
                                              : AppColors.white,
                                          fontWeight: FontWeight.w700,
                                          fontSize: 16,
                                          letterSpacing: 0.5,
                                        ),
                                  ),
                                ],
                              ),
                            ),
                          ),
                        ),
                      ),
                    ),
                    // Current trip card (when started trip exists)
                    BlocBuilder<TripsBloc, TripsState>(
                      buildWhen: (prev, curr) =>
                          curr is TripsLoaded || curr is TripsLoading,
                      builder: (context, tripsState) {
                        Trip? currentTrip;
                        if (tripsState is TripsLoaded &&
                            tripsState.trips.isNotEmpty) {
                          final current = tripsState.trips
                              .where(
                                (t) =>
                                    t.statusCode == kTripScheduledStatus ||
                                    t.statusCode == kTripInProgressStatus,
                              )
                              .toList();
                          if (current.isNotEmpty) {
                            currentTrip = current.first;
                          }
                        }
                        if (currentTrip == null) return const SizedBox.shrink();
                        return Positioned(
                          left: 16,
                          right: 16,
                          bottom: 50,
                          child: TripCard(
                            trip: currentTrip,
                            l10n: l10n,
                            returnRoute: '/home',
                            animateBubble:
                                currentTrip.statusCode == kTripScheduledStatus,
                          ),
                        );
                      },
                    ),
                    // Success message and errors overlay at the bottom
                    Positioned(
                      bottom: 40,
                      left: 0,
                      right: 0,
                      child: Center(
                        child: Column(
                          mainAxisSize: MainAxisSize.min,
                          children: [
                            // Success message
                            if (state.successMessage != null) ...[
                              const SizedBox(height: 16),
                              AnimatedContainer(
                                duration: const Duration(milliseconds: 300),
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 20,
                                  vertical: 12,
                                ),
                                decoration: BoxDecoration(
                                  color: AppColors.vibrantBlue,
                                  borderRadius: BorderRadius.circular(12),
                                  boxShadow: [
                                    BoxShadow(
                                      color: AppColors.vibrantBlue.withValues(
                                        alpha: 0.3,
                                      ),
                                      blurRadius: 8,
                                      offset: const Offset(0, 2),
                                    ),
                                  ],
                                ),
                                child: Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    const Icon(
                                      CupertinoIcons.check_mark_circled_solid,
                                      color: AppColors.white,
                                      size: 18,
                                    ),
                                    const SizedBox(width: 10),
                                    Flexible(
                                      child: Text(
                                        localizeSuccessMessage(
                                          state.successMessage!,
                                          l10n,
                                        ),
                                        style: CupertinoTheme.of(context)
                                            .textTheme
                                            .textStyle
                                            .copyWith(
                                              color: AppColors.white,
                                              fontSize: 14,
                                              fontWeight: FontWeight.w600,
                                            ),
                                        textAlign: TextAlign.center,
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            ],
                            // Location / API error message
                            if (locationBannerText != null) ...[
                              const SizedBox(height: 16),
                              Container(
                                padding: const EdgeInsets.symmetric(
                                  horizontal: 16,
                                  vertical: 10,
                                ),
                                decoration: BoxDecoration(
                                  color: CupertinoColors.systemRed,
                                  borderRadius: BorderRadius.circular(12),
                                  boxShadow: [
                                    BoxShadow(
                                      color: CupertinoColors.systemRed
                                          .withValues(alpha: 0.3),
                                      blurRadius: 8,
                                      offset: const Offset(0, 2),
                                    ),
                                  ],
                                ),
                                child: Row(
                                  mainAxisSize: MainAxisSize.min,
                                  children: [
                                    const Icon(
                                      CupertinoIcons
                                          .exclamationmark_circle_fill,
                                      color: AppColors.white,
                                      size: 16,
                                    ),
                                    const SizedBox(width: 8),
                                    Flexible(
                                      child: Text(
                                        locationBannerText,
                                        style: CupertinoTheme.of(context)
                                            .textTheme
                                            .textStyle
                                            .copyWith(
                                              color: AppColors.white,
                                              fontSize: 13,
                                              fontWeight: FontWeight.w600,
                                            ),
                                        textAlign: TextAlign.center,
                                      ),
                                    ),
                                  ],
                                ),
                              ),
                            ],
                          ],
                        ),
                      ),
                    ),
                    // Trip schedule update card overlay (from push notifications)
                    if (state.pendingScheduleUpdate != null)
                      Positioned(
                        left: 0,
                        right: 0,
                        bottom: 0,
                        child: TripScheduleUpdateCard(
                          update: state.pendingScheduleUpdate!,
                          onDismiss: () {
                            context.read<HomeBloc>().add(
                              const HomeDismissScheduleUpdate(),
                            );
                          },
                        ),
                      ),
                  ],
                );
              }

              return const Center(
                child: CupertinoActivityIndicator(color: AppColors.mainBlue),
              );
            },
          ),
        ),
      ),
    );
  }
}
