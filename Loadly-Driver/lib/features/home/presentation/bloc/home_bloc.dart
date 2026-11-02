import 'dart:async';
import 'dart:io';

import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:geolocator/geolocator.dart';
import 'package:permission_handler/permission_handler.dart';

import '../../../../core/errors/failure_message_mapper.dart';
import '../../../../core/location/driver_location_background_service.dart';
import '../../../../core/storage/auth_storage.dart';
import '../../domain/use_cases/go_offline_usecase.dart';
import '../../domain/use_cases/go_online_usecase.dart';
import 'home_event.dart';
import 'home_location_issue.dart';
import 'home_location_permission_disclosure_context.dart';
import 'home_state.dart';

/// BLoC for managing home screen state (online/offline status and location)
class HomeBloc extends Bloc<HomeEvent, HomeState> {
  HomeBloc({
    required this.goOnlineUseCase,
    required this.goOfflineUseCase,
    required this.backgroundLocationService,
  }) : super(
         HomeLoaded(isOnline: AuthStorage.instance.getIsOnline() ?? false),
       ) {
    on<HomeToggleOnlineStatus>(_onToggleOnlineStatus);
    on<HomeGetInitialLocation>(_onGetInitialLocation);
    on<HomeStartLocationTracking>(_onStartLocationTracking);
    on<HomeStopLocationTracking>(_onStopLocationTracking);
    on<HomeLocationUpdated>(_onLocationUpdated);
    on<HomeLocationPermissionDenied>(_onLocationPermissionDenied);
    on<HomeLocationError>(_onLocationError);
    on<HomeSetOnlineStatus>(_onSetOnlineStatus);
    on<HomeClearSuccessMessage>(_onClearSuccessMessage);
    on<HomeTripScheduleUpdateReceived>(_onTripScheduleUpdateReceived);
    on<HomeDismissScheduleUpdate>(_onDismissScheduleUpdate);
    on<HomeContinueLocationPermissionAfterDisclosure>(
      _onContinueLocationPermissionAfterDisclosure,
    );
    on<HomeDismissLocationPermissionDisclosure>(
      _onDismissLocationPermissionDisclosure,
    );
    on<HomeContinueBackgroundLocationDisclosure>(
      _onContinueBackgroundLocationDisclosure,
    );
    on<HomeDismissBackgroundLocationDisclosure>(
      _onDismissBackgroundLocationDisclosure,
    );
  }

  final GoOnlineUseCase goOnlineUseCase;
  final GoOfflineUseCase goOfflineUseCase;
  final DriverLocationBackgroundService backgroundLocationService;

  StreamSubscription<Position>? _positionSubscription;

  Future<void> _syncBackgroundLocationReporting() async {
    final currentState = state;
    if (currentState is! HomeLoaded) return;
    if (currentState.isOnline) {
      await backgroundLocationService.start();
    } else {
      await backgroundLocationService.stop();
    }
  }

  Future<void> _onToggleOnlineStatus(
    HomeToggleOnlineStatus event,
    Emitter<HomeState> emit,
  ) async {
    final currentState = state;
    if (currentState is! HomeLoaded) return;

    final newIsOnline = !currentState.isOnline;

    // Call API to update online status
    if (newIsOnline) {
      // Ensure that GPS is enabled
      bool serviceEnabled = await Geolocator.isLocationServiceEnabled();

      if (!serviceEnabled) {
        // Open location settings
        await Geolocator.openLocationSettings();

        // // Recheck after returning
        serviceEnabled = await Geolocator.isLocationServiceEnabled();

        // If the user refuses to enable GPS
        if (!serviceEnabled) {
          emit(
            currentState.copyWith(
              locationIssue: const HomeLocationIssue(
                kind: HomeLocationIssueKind.servicesDisabled,
              ),
              clearLocationError: true,
              successMessage: null,
            ),
          );

          return;
        }
      }

      // Going online - need location first
      if (currentState.latitude == null || currentState.longitude == null) {
        // Request location instead of showing error
        add(const HomeGetInitialLocation());

        return;
      }

      final result = await goOnlineUseCase(
        GoOnlineParams(
          latitude: currentState.latitude!,
          longitude: currentState.longitude!,
        ),
      );

      result.fold(
        (failure) {
          emit(
            currentState.copyWith(
              locationError: FailureMessageMapper.forHome(
                failure,
                serverFallback: 'Failed to go online',
                networkFallback: 'Network error. Please try again.',
              ),
              clearLocationIssue: true,
              successMessage: null,
            ),
          );
        },
        (_) {
          // Save online status to storage
          AuthStorage.instance.saveIsOnline(true);

          emit(
            currentState.copyWith(
              isOnline: true,
              successMessage: 'Successfully went online',
              clearLocationError: true,
              clearLocationIssue: true,
            ),
          );

          add(const HomeStartLocationTracking());

          unawaited(_syncBackgroundLocationReporting());

          // Clear success message after 1 second via separate event
          Future.delayed(const Duration(seconds: 1), () {
            if (!isClosed) {
              add(const HomeClearSuccessMessage());
            }
          });
        },
      );
    } else {
      await backgroundLocationService.stop();
      // Going offline - need location
      if (currentState.latitude == null || currentState.longitude == null) {
        // Request location instead of showing error
        add(const HomeGetInitialLocation());
        return;
      }

      final result = await goOfflineUseCase(
        GoOfflineParams(
          latitude: currentState.latitude!,
          longitude: currentState.longitude!,
        ),
      );

      result.fold(
        (failure) {
          emit(
            currentState.copyWith(
              locationError: FailureMessageMapper.forHome(
                failure,
                serverFallback: 'Failed to go offline',
                networkFallback: 'Network error. Please try again.',
              ),
              clearLocationIssue: true,
              successMessage: null,
            ),
          );
        },
        (_) {
          // Save offline status to storage
          AuthStorage.instance.saveIsOnline(false);
          emit(
            currentState.copyWith(
              isOnline: false,
              successMessage: 'Successfully went offline',
              clearLocationError: true,
              clearLocationIssue: true,
            ),
          );
          add(const HomeStopLocationTracking());
          // Clear success message after 1 second via separate event
          Future.delayed(const Duration(seconds: 1), () {
            if (!isClosed) add(const HomeClearSuccessMessage());
          });
        },
      );
    }
  }

  Future<void> _onGetInitialLocation(
    HomeGetInitialLocation event,
    Emitter<HomeState> emit,
  ) async {
    final currentState = state;
    if (currentState is! HomeLoaded) return;

    try {
      LocationPermission permission = await Geolocator.checkPermission();
      if (permission == LocationPermission.deniedForever) {
        emit(
          currentState.copyWith(
            locationIssue: const HomeLocationIssue(
              kind: HomeLocationIssueKind.permissionDeniedForever,
            ),
            clearLocationError: true,
            successMessage: null,
          ),
        );
        return;
      }
      if (permission == LocationPermission.denied) {
        emit(
          currentState.copyWith(
            pendingLocationPermissionDisclosure:
                HomeLocationPermissionDisclosureContext.getInitial,
            clearLocationError: true,
            clearLocationIssue: true,
            successMessage: null,
          ),
        );
        return;
      }

      await _completeGetInitialLocationAfterPermissionGranted(
        emit,
        currentState,
      );
    } catch (e) {
      emit(
        currentState.copyWith(
          locationIssue: const HomeLocationIssue(
            kind: HomeLocationIssueKind.unavailable,
          ),
          clearLocationError: true,
          successMessage: null,
        ),
      );
    }
  }

  Future<void> _completeGetInitialLocationAfterPermissionGranted(
    Emitter<HomeState> emit,
    HomeLoaded currentState,
  ) async {
    try {
      final bool serviceEnabled = await Geolocator.isLocationServiceEnabled();
      if (!serviceEnabled) {
        emit(
          currentState.copyWith(
            locationIssue: const HomeLocationIssue(
              kind: HomeLocationIssueKind.servicesDisabled,
            ),
            clearLocationError: true,
            successMessage: null,
          ),
        );
        return;
      }

      try {
        final position = await Geolocator.getCurrentPosition(
          locationSettings: const LocationSettings(
            accuracy: LocationAccuracy.high,
          ),
        );
        final updatedState = currentState.copyWith(
          latitude: position.latitude,
          longitude: position.longitude,
          clearLocationError: true,
          clearLocationIssue: true,
          successMessage: null,
        );
        emit(updatedState);

        if (updatedState.isOnline) {
          add(const HomeStartLocationTracking());
          unawaited(_syncBackgroundLocationReporting());
        }
      } catch (e) {
        emit(
          currentState.copyWith(
            locationIssue: HomeLocationIssue(
              kind: HomeLocationIssueKind.getPositionFailed,
              detail: '$e',
            ),
            clearLocationError: true,
            successMessage: null,
          ),
        );
      }
    } catch (e) {
      emit(
        currentState.copyWith(
          locationIssue: const HomeLocationIssue(
            kind: HomeLocationIssueKind.unavailable,
          ),
          clearLocationError: true,
          successMessage: null,
        ),
      );
    }
  }

  Future<void> _onStartLocationTracking(
    HomeStartLocationTracking event,
    Emitter<HomeState> emit,
  ) async {
    final currentState = state;
    if (currentState is! HomeLoaded) return;

    try {
      LocationPermission permission = await Geolocator.checkPermission();
      if (permission == LocationPermission.deniedForever) {
        emit(
          currentState.copyWith(
            locationIssue: const HomeLocationIssue(
              kind: HomeLocationIssueKind.permissionDeniedForever,
            ),
            clearLocationError: true,
            isTrackingLocation: false,
            successMessage: null,
          ),
        );
        return;
      }
      if (permission == LocationPermission.denied) {
        emit(
          currentState.copyWith(
            pendingLocationPermissionDisclosure:
                HomeLocationPermissionDisclosureContext.startTracking,
            clearLocationError: true,
            clearLocationIssue: true,
            isTrackingLocation: false,
            successMessage: null,
          ),
        );
        return;
      }

      await _completeStartLocationTrackingAfterPermissionGranted(
        emit,
        currentState,
      );
    } catch (e) {
      emit(
        currentState.copyWith(
          locationIssue: const HomeLocationIssue(
            kind: HomeLocationIssueKind.unavailable,
          ),
          clearLocationError: true,
          isTrackingLocation: false,
          successMessage: null,
        ),
      );
    }
  }

  Future<void> _completeStartLocationTrackingAfterPermissionGranted(
    Emitter<HomeState> emit,
    HomeLoaded currentState,
  ) async {
    try {
      final bool serviceEnabled = await Geolocator.isLocationServiceEnabled();
      if (!serviceEnabled) {
        emit(
          currentState.copyWith(
            locationIssue: const HomeLocationIssue(
              kind: HomeLocationIssueKind.servicesDisabled,
            ),
            clearLocationError: true,
            isTrackingLocation: false,
            successMessage: null,
          ),
        );
        return;
      }

      emit(
        currentState.copyWith(
          isTrackingLocation: true,
          clearLocationError: true,
          clearLocationIssue: true,
        ),
      );

      final trackingState = state as HomeLoaded;

      try {
        final position = await Geolocator.getCurrentPosition(
          locationSettings: const LocationSettings(
            accuracy: LocationAccuracy.high,
          ),
        );
        add(
          HomeLocationUpdated(
            latitude: position.latitude,
            longitude: position.longitude,
          ),
        );
      } catch (e) {
        emit(
          trackingState.copyWith(
            locationIssue: HomeLocationIssue(
              kind: HomeLocationIssueKind.getInitialPositionFailed,
              detail: '$e',
            ),
            clearLocationError: true,
            isTrackingLocation: false,
            successMessage: null,
          ),
        );
        return;
      }

      _positionSubscription?.cancel();
      _positionSubscription =
          Geolocator.getPositionStream(
            locationSettings: const LocationSettings(
              accuracy: LocationAccuracy.high,
              distanceFilter: 10,
            ),
          ).listen(
            (Position position) {
              if (!isClosed) {
                add(
                  HomeLocationUpdated(
                    latitude: position.latitude,
                    longitude: position.longitude,
                  ),
                );
              }
            },
            onError: (error) {
              if (!isClosed) {
                add(
                  HomeLocationError(
                    HomeLocationIssue(
                      kind: HomeLocationIssueKind.streamError,
                      detail: '$error',
                    ),
                  ),
                );
              }
            },
          );
    } catch (e) {
      emit(
        currentState.copyWith(
          locationIssue: const HomeLocationIssue(
            kind: HomeLocationIssueKind.unavailable,
          ),
          clearLocationError: true,
          isTrackingLocation: false,
          successMessage: null,
        ),
      );
    }
  }

  Future<void> _onContinueLocationPermissionAfterDisclosure(
    HomeContinueLocationPermissionAfterDisclosure event,
    Emitter<HomeState> emit,
  ) async {
    final loaded = state;
    if (loaded is! HomeLoaded) return;
    final resume = loaded.pendingLocationPermissionDisclosure;
    if (resume == null) return;

    emit(loaded.copyWith(clearPendingLocationPermissionDisclosure: true));

    final afterClear = state as HomeLoaded;
    final whenInUseStatus = await Permission.locationWhenInUse.request();

    if (!whenInUseStatus.isGranted) {
      emit(
        afterClear.copyWith(
          locationIssue: HomeLocationIssue(
            kind: whenInUseStatus.isPermanentlyDenied
                ? HomeLocationIssueKind.permissionDeniedForever
                : HomeLocationIssueKind.permissionDenied,
          ),
          clearLocationError: true,
          isTrackingLocation: false,
          successMessage: null,
        ),
      );
      if (resume == HomeLocationPermissionDisclosureContext.startTracking) {
        add(const HomeLocationPermissionDenied());
      }
      return;
    }

    final alwaysStatus = await Permission.locationAlways.status;
    if (alwaysStatus.isGranted) {
      final latest = state as HomeLoaded;
      await _resumeLocationFlowAfterDisclosure(emit, latest, resume);
      return;
    }

    emit(afterClear.copyWith(pendingBackgroundLocationDisclosure: resume));
  }

  Future<void> _onContinueBackgroundLocationDisclosure(
    HomeContinueBackgroundLocationDisclosure event,
    Emitter<HomeState> emit,
  ) async {
    final loaded = state;
    if (loaded is! HomeLoaded) return;
    final resume = loaded.pendingBackgroundLocationDisclosure;
    if (resume == null) return;

    emit(loaded.copyWith(clearPendingBackgroundLocationDisclosure: true));
    final latest = state as HomeLoaded;

    // Request notification permission on Android 13+
    if (Platform.isAndroid) {
      final notifStatus = await Permission.notification.status;
      if (notifStatus.isDenied) {
        await Permission.notification.request();
      }
    }

    await Permission.locationAlways.request();
    await _resumeLocationFlowAfterDisclosure(emit, latest, resume);
  }

  Future<void> _onDismissBackgroundLocationDisclosure(
    HomeDismissBackgroundLocationDisclosure event,
    Emitter<HomeState> emit,
  ) async {
    final loaded = state;
    if (loaded is! HomeLoaded) return;
    final resume = loaded.pendingBackgroundLocationDisclosure;
    if (resume == null) return;

    emit(loaded.copyWith(clearPendingBackgroundLocationDisclosure: true));
    final latest = state as HomeLoaded;

    await _resumeLocationFlowAfterDisclosure(emit, latest, resume);
  }

  Future<void> _resumeLocationFlowAfterDisclosure(
    Emitter<HomeState> emit,
    HomeLoaded current,
    HomeLocationPermissionDisclosureContext resume,
  ) async {
    switch (resume) {
      case HomeLocationPermissionDisclosureContext.getInitial:
        await _completeGetInitialLocationAfterPermissionGranted(emit, current);
        break;
      case HomeLocationPermissionDisclosureContext.startTracking:
        await _completeStartLocationTrackingAfterPermissionGranted(
          emit,
          current,
        );
        break;
    }
  }

  void _onDismissLocationPermissionDisclosure(
    HomeDismissLocationPermissionDisclosure event,
    Emitter<HomeState> emit,
  ) {
    final loaded = state;
    if (loaded is! HomeLoaded) return;
    final resume = loaded.pendingLocationPermissionDisclosure;
    if (resume == null) return;

    emit(
      loaded.copyWith(
        clearPendingLocationPermissionDisclosure: true,
        locationIssue: const HomeLocationIssue(
          kind: HomeLocationIssueKind.permissionDenied,
        ),
        clearLocationError: true,
        isTrackingLocation: false,
        successMessage: null,
      ),
    );
    if (resume == HomeLocationPermissionDisclosureContext.startTracking) {
      add(const HomeLocationPermissionDenied());
    }
  }

  void _onStopLocationTracking(
    HomeStopLocationTracking event,
    Emitter<HomeState> emit,
  ) {
    _positionSubscription?.cancel();
    _positionSubscription = null;

    final currentState = state;
    if (currentState is HomeLoaded) {
      emit(
        currentState.copyWith(
          isTrackingLocation: false,
          clearLocationError: true,
          clearLocationIssue: true,
        ),
      );
    }
  }

  Future<void> _onLocationUpdated(
    HomeLocationUpdated event,
    Emitter<HomeState> emit,
  ) async {
    final currentState = state;
    if (currentState is! HomeLoaded) return;

    // Update local state only (location update API does not exist)
    emit(
      currentState.copyWith(
        latitude: event.latitude,
        longitude: event.longitude,
        clearLocationError: true,
        clearLocationIssue: true,
        successMessage: null,
      ),
    );
  }

  void _onLocationError(HomeLocationError event, Emitter<HomeState> emit) {
    final currentState = state;
    if (currentState is HomeLoaded) {
      emit(
        currentState.copyWith(
          locationIssue: event.issue,
          clearLocationError: true,
          isTrackingLocation: false,
          successMessage: null,
        ),
      );
    }
  }

  void _onLocationPermissionDenied(
    HomeLocationPermissionDenied event,
    Emitter<HomeState> emit,
  ) {
    final currentState = state;
    if (currentState is HomeLoaded) {
      emit(
        currentState.copyWith(
          locationIssue: const HomeLocationIssue(
            kind: HomeLocationIssueKind.permissionDenied,
          ),
          clearLocationError: true,
          isTrackingLocation: false,
          successMessage: null,
        ),
      );
    }
  }

  void _onSetOnlineStatus(HomeSetOnlineStatus event, Emitter<HomeState> emit) {
    final currentState = state;
    if (currentState is HomeLoaded) {
      // Save online status to storage
      AuthStorage.instance.saveIsOnline(event.isOnline);
      emit(currentState.copyWith(isOnline: event.isOnline));

      if (event.isOnline) {
        if (currentState.latitude != null && currentState.longitude != null) {
          add(const HomeStartLocationTracking());
          unawaited(_syncBackgroundLocationReporting());
        } else {
          add(const HomeGetInitialLocation());
        }
      } else {
        unawaited(backgroundLocationService.stop());
        add(const HomeStopLocationTracking());
      }
    }
  }

  void _onClearSuccessMessage(
    HomeClearSuccessMessage event,
    Emitter<HomeState> emit,
  ) {
    final currentState = state;
    if (currentState is HomeLoaded) {
      emit(currentState.copyWith(clearSuccessMessage: true));
    }
  }

  void _onTripScheduleUpdateReceived(
    HomeTripScheduleUpdateReceived event,
    Emitter<HomeState> emit,
  ) {
    final currentState = state;
    if (currentState is HomeLoaded) {
      emit(currentState.copyWith(pendingScheduleUpdate: event.update));
    }
  }

  void _onDismissScheduleUpdate(
    HomeDismissScheduleUpdate event,
    Emitter<HomeState> emit,
  ) {
    final currentState = state;
    if (currentState is HomeLoaded) {
      emit(currentState.copyWith(clearPendingScheduleUpdate: true));
    }
  }

  @override
  Future<void> close() {
    _positionSubscription?.cancel();
    unawaited(backgroundLocationService.stop());
    return super.close();
  }
}
