import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../core/errors/failure_message_mapper.dart';
import '../../../../core/location/current_position_reader.dart';
import '../../../../core/location/trip_gps_session_reconciler.dart';
import '../../../../core/location/trip_odometer_service.dart';
import '../../domain/entities/trip.dart';
import '../../domain/failures/trips_failure.dart';
import '../../domain/use_cases/get_trip_details_usecase.dart';
import '../../domain/use_cases/get_trips_usecase.dart';
import '../../domain/use_cases/trip_actions_usecase.dart';
import 'trips_event.dart';
import 'trips_state.dart';

class TripsBloc extends Bloc<TripsEvent, TripsState> {
  TripsBloc({
    required this.getTripsUseCase,
    required this.getTripDetailsUseCase,
    required this.getDocumentDownloadUrlUseCase,
    required this.startLoadingUseCase,
    required this.completeLoadingUseCase,
    required this.startUnloadingUseCase,
    required this.completeUnloadingUseCase,
    required this.startTripUseCase,
    required this.arriveAtWaypointUseCase,
    required this.departFromWaypointUseCase,
    required this.enterCustomsUseCase,
    required this.exitCustomsUseCase,
    required this.completeTripUseCase,
    CurrentPositionReader? positionReader,
    TripOdometerService? tripOdometerService,
    TripGpsSessionReconciler? tripGpsSessionReconciler,
  }) : _positionReader =
           positionReader ?? const GeolocatorCurrentPositionReader(),
       _tripOdometer = tripOdometerService,
       _gpsReconciler = tripGpsSessionReconciler,
       super(const TripsInitial()) {
    on<TripsLoadTrips>(_onLoadTrips);
    on<TripsLoadTripDetails>(_onLoadTripDetails);
    on<TripsRefresh>(_onRefresh);
    on<TripsLoadMore>(_onLoadMore);
    on<TripsStartLoading>(_onStartLoading);
    on<TripsCompleteLoading>(_onCompleteLoading);
    on<TripsStartUnloading>(_onStartUnloading);
    on<TripsCompleteUnloading>(_onCompleteUnloading);
    on<TripsStartTrip>(_onStartTrip);
    on<TripsArriveAtWaypoint>(_onArriveAtWaypoint);
    on<TripsDepartFromWaypoint>(_onDepartFromWaypoint);
    on<TripsEnterCustoms>(_onEnterCustoms);
    on<TripsExitCustoms>(_onExitCustoms);
    on<TripsCompleteTrip>(_onCompleteTrip);
    on<TripsOpenDocument>(_onOpenDocument);
    on<TripsClearDocumentUrl>(_onClearDocumentUrl);
  }

  final GetTripsUseCase getTripsUseCase;
  final GetTripDetailsUseCase getTripDetailsUseCase;
  final GetDocumentDownloadUrlUseCase getDocumentDownloadUrlUseCase;
  final StartLoadingUseCase startLoadingUseCase;
  final CompleteLoadingUseCase completeLoadingUseCase;
  final StartUnloadingUseCase startUnloadingUseCase;
  final CompleteUnloadingUseCase completeUnloadingUseCase;
  final StartTripUseCase startTripUseCase;
  final ArriveAtWaypointUseCase arriveAtWaypointUseCase;
  final DepartFromWaypointUseCase departFromWaypointUseCase;
  final EnterCustomsUseCase enterCustomsUseCase;
  final ExitCustomsUseCase exitCustomsUseCase;
  final CompleteTripUseCase completeTripUseCase;
  final CurrentPositionReader _positionReader;
  final TripOdometerService? _tripOdometer;
  final TripGpsSessionReconciler? _gpsReconciler;

  Future<void> _syncOdometerForTrip(Trip trip) async {
    await _gpsReconciler?.applyServerTripStatus(trip);
  }

  Future<(double?, double?)> _getLocation() async {
    return _positionReader.readCurrentPosition();
  }

  String _actionFailureMessage(TripsEvent event) {
    return switch (event) {
      TripsStartLoading() => 'Failed to start loading',
      TripsCompleteLoading() => 'Failed to complete loading',
      TripsStartUnloading() => 'Failed to start unloading',
      TripsCompleteUnloading() => 'Failed to complete unloading',
      TripsStartTrip() => 'Failed to start trip',
      TripsArriveAtWaypoint() => 'Failed to mark arrival',
      TripsDepartFromWaypoint() => 'Failed to depart',
      TripsEnterCustoms() => 'Failed to Enter Customs',
      TripsExitCustoms() => 'Failed to Exit Customs',
      TripsCompleteTrip() => 'Failed to complete trip',
      _ => 'Action failed',
    };
  }

  Future<void> _onLoadTrips(
    TripsLoadTrips event,
    Emitter<TripsState> emit,
  ) async {
    emit(const TripsLoading());

    final result = await getTripsUseCase(
      GetTripsParams(
        page: event.page ?? 1,
        limit: event.limit ?? 10,
        status: event.status,
        startDateFrom: _normalizeDate(_startDateFrom),
        startDateTo: _normalizeDate(_startDateTo),
      ),
    );

    await result.fold(
      (failure) async {
        final errorMessage = FailureMessageMapper.forTrips(
          failure,
          serverFallback: 'Failed to load trips',
        );
        emit(TripsLoaded(trips: const [], error: errorMessage));
      },
      (listResult) async {
        await _gpsReconciler?.reconcileFromTrips(listResult.trips);
        _storeTripsFilter(event.status, event.startDateFrom, event.startDateTo);
        emit(
          TripsLoaded(
            trips: listResult.trips,
            error: null,
            page: listResult.page,
            total: listResult.total,
            pageSize: listResult.pageSize,
          ),
        );
      },
    );
  }

  Future<void> _onLoadMore(
    TripsLoadMore event,
    Emitter<TripsState> emit,
  ) async {
    final current = state;
    if (current is! TripsLoaded || !current.hasMore || current.error != null) {
      return;
    }

    final result = await getTripsUseCase(
      GetTripsParams(
        page: current.page + 1,
        limit: current.pageSize,
        status: _statusFromCurrentState(current),
        startDateFrom: _startDateFrom,
        startDateTo: _startDateTo,
      ),
    );

    result.fold(
      (failure) {
        final errorMessage = FailureMessageMapper.forTrips(
          failure,
          serverFallback: 'Failed to load more',
          networkFallback: 'Network error. Please check your connection.',
        );
        emit(current.copyWith(error: errorMessage));
      },
      (listResult) {
        emit(
          current.copyWith(
            trips: [...current.trips, ...listResult.trips],
            page: listResult.page,
            total: listResult.total,
            error: null,
          ),
        );
      },
    );
  }

  int? _statusFromCurrentState(TripsLoaded s) => _tripsLoadedStatus;
  String? _startDateFrom;
  String? _startDateTo;
  int? _tripsLoadedStatus;

  void _storeTripsFilter(int? status, String? from, String? to) {
    _tripsLoadedStatus = status;
    _startDateFrom = _normalizeDate(from);
    _startDateTo = _normalizeDate(to);
  }

  Future<void> _onLoadTripDetails(
    TripsLoadTripDetails event,
    Emitter<TripsState> emit,
  ) async {
    final current = state;
    final keepTripVisible = switch (current) {
      TripDetailsLoaded(:final trip)
          when trip.id == event.tripId && trip.id != 0 =>
        true,
      TripActionInProgress(:final trip)
          when trip.id == event.tripId && trip.id != 0 =>
        true,
      _ => false,
    };
    // A full-screen loader disposes the step form and drops photos the driver
    // just captured. Keep the current trip on screen while a refresh runs.
    if (!keepTripVisible) {
      emit(const TripDetailsLoading());
    }

    final result = await getTripDetailsUseCase(
      GetTripDetailsParams(tripId: event.tripId),
    );

    TripsFailure? loadFailure;
    Trip? loadedTrip;
    result.fold(
      (failure) => loadFailure = failure,
      (trip) => loadedTrip = trip,
    );
    if (loadFailure != null) {
      final errorMessage = FailureMessageMapper.forTrips(
        loadFailure!,
        serverFallback: 'Failed to load trip details',
      );
      emit(TripDetailsLoaded(trip: _getEmptyTrip(), error: errorMessage));
      return;
    }
    final trip = loadedTrip!;
    await _syncOdometerForTrip(trip);
    emit(TripDetailsLoaded(trip: trip, error: null));
  }

  Future<void> _onRefresh(TripsRefresh event, Emitter<TripsState> emit) async {
    final currentState = state;
    final pageSize = currentState is TripsLoaded ? currentState.pageSize : 10;
    add(
      TripsLoadTrips(
        page: 1,
        limit: pageSize,
        status: _tripsLoadedStatus,
        startDateFrom: _startDateFrom,
        startDateTo: _startDateTo,
      ),
    );
  }

  Future<void> _onStartLoading(
    TripsStartLoading event,
    Emitter<TripsState> emit,
  ) async {
    final current = state;
    if (current is! TripDetailsLoaded || current.trip.id != event.tripId) {
      add(TripsLoadTripDetails(tripId: event.tripId));
      return;
    }
    emit(
      TripActionInProgress(trip: current.trip, message: 'Starting loading...'),
    );
    var (lat, lng) = (event.latitude, event.longitude);
    if (lat == null || lng == null) {
      final loc = await _getLocation();
      lat = loc.$1;
      lng = loc.$2;
    }
    final result = await startLoadingUseCase(
      tripId: event.tripId,
      waypointId: event.waypointId,
      latitude: lat,
      longitude: lng,
    );
    result.fold(
      (failure) => emit(
        TripDetailsLoaded(
          trip: current.trip,
          error: FailureMessageMapper.forTrips(
            failure,
            serverFallback: _actionFailureMessage(event),
            networkFallback: _actionFailureMessage(event),
            unknownFallback: _actionFailureMessage(event),
          ),
        ),
      ),
      (trip) {
        emit(TripDetailsLoaded(trip: trip, error: null));
        add(TripsLoadTripDetails(tripId: event.tripId));
      },
    );
  }

  Future<void> _onCompleteLoading(
    TripsCompleteLoading event,
    Emitter<TripsState> emit,
  ) async {
    final current = state;
    if (current is! TripDetailsLoaded || current.trip.id != event.tripId) {
      add(TripsLoadTripDetails(tripId: event.tripId));
      return;
    }
    emit(
      TripActionInProgress(
        trip: current.trip,
        message: 'Completing loading...',
      ),
    );
    var (lat, lng) = (event.latitude, event.longitude);
    if (lat == null || lng == null) {
      final loc = await _getLocation();
      lat = loc.$1;
      lng = loc.$2;
    }
    final result = await completeLoadingUseCase(
      tripId: event.tripId,
      waypointId: event.waypointId,
      receiptNoteImages: event.receiptNoteImages,
      shipmentImages: event.shipmentImages,
      note: event.note,
      latitude: lat,
      longitude: lng,
    );
    result.fold(
      (failure) => emit(
        TripDetailsLoaded(
          trip: current.trip,
          error: FailureMessageMapper.forTrips(
            failure,
            serverFallback: _actionFailureMessage(event),
            networkFallback: _actionFailureMessage(event),
            unknownFallback: _actionFailureMessage(event),
          ),
        ),
      ),
      (trip) {
        emit(TripDetailsLoaded(trip: trip, error: null));
        add(TripsLoadTripDetails(tripId: event.tripId));
      },
    );
  }

  Future<void> _onStartUnloading(
    TripsStartUnloading event,
    Emitter<TripsState> emit,
  ) async {
    final current = state;
    if (current is! TripDetailsLoaded || current.trip.id != event.tripId) {
      add(TripsLoadTripDetails(tripId: event.tripId));
      return;
    }
    emit(
      TripActionInProgress(
        trip: current.trip,
        message: 'Starting unloading...',
      ),
    );
    var (lat, lng) = (event.latitude, event.longitude);
    if (lat == null || lng == null) {
      final loc = await _getLocation();
      lat = loc.$1;
      lng = loc.$2;
    }
    final result = await startUnloadingUseCase(
      tripId: event.tripId,
      waypointId: event.waypointId,
      latitude: lat,
      longitude: lng,
    );
    result.fold(
      (failure) => emit(
        TripDetailsLoaded(
          trip: current.trip,
          error: FailureMessageMapper.forTrips(
            failure,
            serverFallback: _actionFailureMessage(event),
            networkFallback: _actionFailureMessage(event),
            unknownFallback: _actionFailureMessage(event),
          ),
        ),
      ),
      (trip) {
        emit(TripDetailsLoaded(trip: trip, error: null));
        add(TripsLoadTripDetails(tripId: event.tripId));
      },
    );
  }

  Future<void> _onCompleteUnloading(
    TripsCompleteUnloading event,
    Emitter<TripsState> emit,
  ) async {
    final current = state;
    if (current is! TripDetailsLoaded || current.trip.id != event.tripId) {
      add(TripsLoadTripDetails(tripId: event.tripId));
      return;
    }
    emit(
      TripActionInProgress(
        trip: current.trip,
        message: 'Completing unloading...',
      ),
    );
    var (lat, lng) = (event.latitude, event.longitude);
    if (lat == null || lng == null) {
      final loc = await _getLocation();
      lat = loc.$1;
      lng = loc.$2;
    }
    final result = await completeUnloadingUseCase(
      tripId: event.tripId,
      waypointId: event.waypointId,
      odometer: event.odometer,
      odometerImages: event.odometerImages,
      unloadingImages: event.unloadingImages,
      deliveryNoteImages: event.deliveryNoteImages,
      note: event.note,
      latitude: lat,
      longitude: lng,
    );
    result.fold(
      (failure) => emit(
        TripDetailsLoaded(
          trip: current.trip,
          error: FailureMessageMapper.forTrips(
            failure,
            serverFallback: _actionFailureMessage(event),
            networkFallback: _actionFailureMessage(event),
            unknownFallback: _actionFailureMessage(event),
          ),
        ),
      ),
      (trip) {
        emit(TripDetailsLoaded(trip: trip, error: null));
        add(TripsLoadTripDetails(tripId: event.tripId));
      },
    );
  }

  Future<void> _onStartTrip(
    TripsStartTrip event,
    Emitter<TripsState> emit,
  ) async {
    final current = state;
    if (current is! TripDetailsLoaded || current.trip.id != event.tripId) {
      add(TripsLoadTripDetails(tripId: event.tripId));
      return;
    }
    emit(TripActionInProgress(trip: current.trip, message: 'Starting trip...'));
    var (lat, lng) = (event.latitude, event.longitude);
    if (lat == null || lng == null) {
      final loc = await _getLocation();
      lat = loc.$1;
      lng = loc.$2;
    }
    final result = await startTripUseCase(
      tripId: event.tripId,
      note: event.note,
      latitude: lat,
      longitude: lng,
    );
    TripsFailure? actionFailure;
    Trip? updatedTrip;
    result.fold(
      (failure) => actionFailure = failure,
      (trip) => updatedTrip = trip,
    );
    if (actionFailure != null) {
      emit(
        TripDetailsLoaded(
          trip: current.trip,
          error: FailureMessageMapper.forTrips(
            actionFailure!,
            serverFallback: _actionFailureMessage(event),
            networkFallback: _actionFailureMessage(event),
            unknownFallback: _actionFailureMessage(event),
          ),
        ),
      );
      return;
    }
    if (lat != null && lng != null) {
      await _tripOdometer?.startTrip(
        event.tripId,
        seedLatitude: lat,
        seedLongitude: lng,
      );
    }
    emit(TripDetailsLoaded(trip: updatedTrip!, error: null));
    add(TripsLoadTripDetails(tripId: event.tripId));
  }

  Future<void> _onArriveAtWaypoint(
    TripsArriveAtWaypoint event,
    Emitter<TripsState> emit,
  ) async {
    final current = state;
    if (current is! TripDetailsLoaded || current.trip.id != event.tripId) {
      add(TripsLoadTripDetails(tripId: event.tripId));
      return;
    }
    emit(
      TripActionInProgress(trip: current.trip, message: 'Marking arrival...'),
    );
    var (lat, lng) = (event.latitude, event.longitude);
    if (lat == null || lng == null) {
      final loc = await _getLocation();
      lat = loc.$1;
      lng = loc.$2;
    }
    final result = await arriveAtWaypointUseCase(
      tripId: event.tripId,
      waypointId: event.waypointId,
      latitude: lat,
      longitude: lng,
    );
    result.fold(
      (failure) => emit(
        TripDetailsLoaded(
          trip: current.trip,
          error: FailureMessageMapper.forTrips(
            failure,
            serverFallback: _actionFailureMessage(event),
            networkFallback: _actionFailureMessage(event),
            unknownFallback: _actionFailureMessage(event),
          ),
        ),
      ),
      (trip) {
        emit(TripDetailsLoaded(trip: trip, error: null));
        add(TripsLoadTripDetails(tripId: event.tripId));
      },
    );
  }

  Future<void> _onDepartFromWaypoint(
    TripsDepartFromWaypoint event,
    Emitter<TripsState> emit,
  ) async {
    final current = state;
    if (current is! TripDetailsLoaded || current.trip.id != event.tripId) {
      add(TripsLoadTripDetails(tripId: event.tripId));
      return;
    }
    emit(TripActionInProgress(trip: current.trip, message: 'Departing...'));
    var (lat, lng) = (event.latitude, event.longitude);
    if (lat == null || lng == null) {
      final loc = await _getLocation();
      lat = loc.$1;
      lng = loc.$2;
    }
    final result = await departFromWaypointUseCase(
      tripId: event.tripId,
      waypointId: event.waypointId,
      deliveryNoteImages: event.deliveryNoteImages,
      note: event.note,
      latitude: lat,
      longitude: lng,
      odometer: event.odometer,
      odometerImages: event.odometerImages,
    );
    result.fold(
      (failure) => emit(
        TripDetailsLoaded(
          trip: current.trip,
          error: FailureMessageMapper.forTrips(
            failure,
            serverFallback: _actionFailureMessage(event),
            networkFallback: _actionFailureMessage(event),
            unknownFallback: _actionFailureMessage(event),
          ),
        ),
      ),
      (trip) {
        emit(TripDetailsLoaded(trip: trip, error: null));
        add(TripsLoadTripDetails(tripId: event.tripId));
      },
    );
  }

  Future<void> _onEnterCustoms(
    TripsEnterCustoms event,
    Emitter<TripsState> emit,
  ) async {
    final current = state;
    if (current is! TripDetailsLoaded || current.trip.id != event.tripId) {
      add(TripsLoadTripDetails(tripId: event.tripId));
      return;
    }
    emit(
      TripActionInProgress(trip: current.trip, message: 'Entering Customs...'),
    );
    var (lat, lng) = (event.latitude, event.longitude);
    if (lat == null || lng == null) {
      final loc = await _getLocation();
      lat = loc.$1;
      lng = loc.$2;
    }
    final result = await enterCustomsUseCase(
      tripId: event.tripId,
      customDocuments: event.customDocuments,
      note: event.note,
      latitude: lat,
      longitude: lng,
    );
    result.fold(
      (failure) => emit(
        TripDetailsLoaded(
          trip: current.trip,
          error: FailureMessageMapper.forTrips(
            failure,
            serverFallback: _actionFailureMessage(event),
            networkFallback: _actionFailureMessage(event),
            unknownFallback: _actionFailureMessage(event),
          ),
        ),
      ),
      (trip) {
        emit(TripDetailsLoaded(trip: trip, error: null));
        add(TripsLoadTripDetails(tripId: event.tripId));
      },
    );
  }

  Future<void> _onExitCustoms(
    TripsExitCustoms event,
    Emitter<TripsState> emit,
  ) async {
    final current = state;
    if (current is! TripDetailsLoaded || current.trip.id != event.tripId) {
      add(TripsLoadTripDetails(tripId: event.tripId));
      return;
    }
    emit(
      TripActionInProgress(trip: current.trip, message: 'Exiting Customs...'),
    );
    var (lat, lng) = (event.latitude, event.longitude);
    if (lat == null || lng == null) {
      final loc = await _getLocation();
      lat = loc.$1;
      lng = loc.$2;
    }
    final result = await exitCustomsUseCase(
      tripId: event.tripId,
      customsTotalAmount: event.customsTotalAmount,
      customsReceipts: event.customsReceipts,
      note: event.note,
      latitude: lat,
      longitude: lng,
    );
    result.fold(
      (failure) => emit(
        TripDetailsLoaded(
          trip: current.trip,
          error: FailureMessageMapper.forTrips(
            failure,
            serverFallback: _actionFailureMessage(event),
            networkFallback: _actionFailureMessage(event),
            unknownFallback: _actionFailureMessage(event),
          ),
        ),
      ),
      (trip) {
        emit(TripDetailsLoaded(trip: trip, error: null));
        add(TripsLoadTripDetails(tripId: event.tripId));
      },
    );
  }

  Future<void> _onCompleteTrip(
    TripsCompleteTrip event,
    Emitter<TripsState> emit,
  ) async {
    final current = state;
    if (current is! TripDetailsLoaded || current.trip.id != event.tripId) {
      add(TripsLoadTripDetails(tripId: event.tripId));
      return;
    }
    emit(
      TripActionInProgress(trip: current.trip, message: 'Completing trip...'),
    );
    var (lat, lng) = (event.latitude, event.longitude);
    if (lat == null || lng == null) {
      final loc = await _getLocation();
      lat = loc.$1;
      lng = loc.$2;
    }
    final result = await completeTripUseCase(
      tripId: event.tripId,
      odometer: event.odometer,
      odometerImages: event.odometerImages,
      tollgateImages: event.tollgateImages,
      tollgateWeightImages: event.tollgateWeightImages,
      note: event.note,
      latitude: lat,
      longitude: lng,
    );
    TripsFailure? actionFailure;
    Trip? completedTrip;
    result.fold(
      (failure) => actionFailure = failure,
      (trip) => completedTrip = trip,
    );
    if (actionFailure != null) {
      final failure = actionFailure!;
      emit(
        TripDetailsLoaded(
          trip: current.trip,
          error: failure is TripsFailureServer
              ? failure.message ?? _actionFailureMessage(event)
              : _actionFailureMessage(event),
        ),
      );
      return;
    }
    final lifecycleActor =
        completedTrip!.lifecycleActor ?? kTripLifecycleActorDriver;
    if (lat != null && lng != null) {
      await _tripOdometer?.finalizeTrip(
        event.tripId,
        finalLatitude: lat,
        finalLongitude: lng,
        lifecycleActor: lifecycleActor,
      );
    } else {
      await _tripOdometer?.finalizeTrip(
        event.tripId,
        lifecycleActor: lifecycleActor,
      );
    }
    emit(
      TripDetailsLoaded(
        trip: completedTrip!,
        error: null,
        tripJustCompleted: true,
      ),
    );
  }

  Future<void> _onOpenDocument(
    TripsOpenDocument event,
    Emitter<TripsState> emit,
  ) async {
    final current = state;
    final trip =
        (current is TripDetailsLoaded && current.trip.id == event.tripId)
        ? current.trip
        : _getEmptyTrip();

    final result = await getDocumentDownloadUrlUseCase(
      tripId: event.tripId,
      documentPath: event.documentPath,
    );
    result.fold((failure) {
      if (current is TripDetailsLoaded && current.trip.id == event.tripId) {
        emit(
          current.copyWith(
            error: FailureMessageMapper.forTrips(
              failure,
              serverFallback: 'Failed to get document',
              networkFallback: 'Failed to get document',
              unknownFallback: 'Failed to get document',
            ),
          ),
        );
      }
    }, (url) => emit(TripDetailsLoaded(trip: trip, documentUrlToOpen: url)));
  }

  void _onClearDocumentUrl(
    TripsClearDocumentUrl event,
    Emitter<TripsState> emit,
  ) {
    final current = state;
    if (current is TripDetailsLoaded) {
      emit(current.copyWith(documentUrlToOpen: null));
    }
  }

  Trip _getEmptyTrip() {
    return const Trip(
      cutoffDuration: 0,
      id: 0,
      title: '',
      startDateTime: '',
      startDate: '',
      startTime: '',
      pickupLocation: Location(address: ''),
      deliveryLocation: Location(address: ''),
      shipmentDetails: ShipmentDetails(weight: 0, type: '', duration: 0),
    );
  }

  String? _normalizeDate(String? date) {
    if (date == null || date.isEmpty) return null;

    try {
      final parsed = DateTime.parse(date);
      return parsed.toUtc().toIso8601String();
    } catch (_) {
      return date;
    }
  }
}
