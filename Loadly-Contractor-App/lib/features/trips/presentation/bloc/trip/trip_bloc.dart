// ignore_for_file: prefer_initializing_formals

import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../../../core/storage/auth_storage.dart';
import '../../../../home/presentation/widgets/home_new_trip_highlight_tracker.dart';
import '../../../domain/entities/trip/trip.dart';
import '../../../domain/use_cases/book_trip_usecase.dart';
import '../../../domain/use_cases/get_trip_usecase.dart';
import '../../../domain/use_cases/get_trips_usecase.dart';
import '../../../domain/use_cases/withdraw_trip_usecase.dart';
import 'trip_event.dart';
import 'trip_state.dart';

class TripBloc extends Bloc<TripEvent, TripState> {
  TripBloc({
    required GetTripsUseCase getTripsUseCase,
    required GetTripUseCase getTripUseCase,
    required BookTripUseCase bookTripUseCase,
    required WithdrawTripUseCase withdrawTripUseCase,
  }) : _getTripsUseCase = getTripsUseCase,
       _getTripUseCase = getTripUseCase,
       _bookTripUseCase = bookTripUseCase,
       _withdrawTripUseCase = withdrawTripUseCase,
       super(TripInitial()) {
    on<GetTrip>(_onGetTrip);
    on<GetTrips>(_onGetTrips);
    on<BookTrip>(_onBookTrip);
    on<WithdrawTrip>(_onWithdrawTrip);
  }

  TripLoaded? lastTripLoaded;

  final GetTripsUseCase _getTripsUseCase;
  final GetTripUseCase _getTripUseCase;
  final BookTripUseCase _bookTripUseCase;
  final WithdrawTripUseCase _withdrawTripUseCase;
  final Map<int, Trip> cachedTrips = {};
  final HomeNewTripHighlightTracker _homeNewTripHighlight =
      HomeNewTripHighlightTracker();

  bool isHomeNewTripHighlighted(int tripId) =>
      _homeNewTripHighlight.isHighlighted(tripId);

  bool get hasActiveHomeNewTripHighlights =>
      _homeNewTripHighlight.hasActiveHighlights();

  void resetHomeNewTripHighlightsForSessionChange() {
    _homeNewTripHighlight.reset();
  }

  static bool shouldTrackHomeNewTripHighlights(GetTrips event) {
    if (event.loadMore || event.page != 1) return false;
    if (event.status != 1 || event.openInviteOnly != true) return false;
    if (event.statuses != null && event.statuses!.isNotEmpty) return false;
    if (event.vehicleTypeId != null) return false;
    final search = event.search?.trim();
    if (search != null && search.isNotEmpty) return false;
    if (event.startDateFrom != null && event.startDateFrom!.isNotEmpty) {
      return false;
    }
    if (event.startDateTo != null && event.startDateTo!.isNotEmpty) {
      return false;
    }
    return true;
  }

  Future<void> _onGetTrip(GetTrip event, Emitter<TripState> emit) async {
    emit(TripDetailLoading(event.tripId));
    final result = await _getTripUseCase(GetTripParams(event.tripId));
    result.fold(
      (failure) => emit(TripDetailError(failure.message, tripId: event.tripId)),
      (trip) {
        final id = trip.id;
        if (id != null) cachedTrips[id] = trip;
        emit(TripDetailLoaded(trip));
      },
    );
  }

  Future<void> _onGetTrips(GetTrips event, Emitter<TripState> emit) async {
    final previousState = state is TripLoaded ? state as TripLoaded : null;
    final staleList = previousState ?? lastTripLoaded;

    final incomingQuery = TripsListQuery.fromGetTrips(event);

    if (event.loadMore) {
      if (previousState == null ||
          previousState.isLoadingMore ||
          !previousState.hasMore) {
        return;
      }

      emit(previousState.copyWith(isLoadingMore: true));
    } else {
      final keepVisibleList =
          previousState != null && previousState.query == incomingQuery;
      if (!keepVisibleList) {
        emit(TripLoading());
      }
    }

    final result = await _getTripsUseCase(
      GetTripsParams(
        page: event.page,
        pageSize: event.pageSize,
        status: event.status,
        statuses: event.statuses,
        openInviteOnly: event.openInviteOnly,
        search: event.search,
        vehicleTypeId: event.vehicleTypeId,
        startDateFrom: event.startDateFrom,
        startDateTo: event.startDateTo,
        isContractorPaid: event.isContractorPaid,
        hasContractor: event.hasContractor,
      ),
    );

    await result.fold(
      (failure) async {
        if (event.loadMore && previousState != null) {
          emit(previousState.copyWith(isLoadingMore: false));
        } else if (staleList != null) {
          emit(staleList);
        } else {
          emit(TripError(failure.message));
        }
      },
      (data) async {
        for (final trip in data.trips) {
          final id = trip.id;
          if (id != null) {
            cachedTrips[id] = trip;
          }
        }

        final pageTrips = data.trips;
        final totalCount = data.totalCount;

        final allTrips = event.loadMore && previousState != null
            ? [...previousState.trips, ...pageTrips]
            : pageTrips;

        if (shouldTrackHomeNewTripHighlights(event)) {
          await _homeNewTripHighlight.onOpenInviteTripsLoaded(
            allTrips,
            contractorUserId: AuthStorage.instance.getContractorUserId(),
          );
        }

        lastTripLoaded = TripLoaded(
          allTrips,
          totalCount: totalCount,
          query: incomingQuery,
          currentPage: event.page,
          pageSize: event.pageSize,
          isLoadingMore: false,
        );
        emit(lastTripLoaded!);
      },
    );
  }

  void _restoreTripDetail(int tripId, Emitter<TripState> emit) {
    final trip = cachedTrips[tripId];
    if (trip != null) {
      emit(TripDetailLoaded(trip));
    }
  }

  Future<void> _onBookTrip(BookTrip event, Emitter<TripState> emit) async {
    emit(TripBooking());
    final result = await _bookTripUseCase(event.tripId);
    result.fold(
      (failure) {
        emit(TripBookError(failure.message));
        _restoreTripDetail(event.tripId, emit);
      },
      (_) {
        emit(TripBooked());
        _restoreTripDetail(event.tripId, emit);
      },
    );
  }

  Future<void> _onWithdrawTrip(
    WithdrawTrip event,
    Emitter<TripState> emit,
  ) async {
    emit(TripWithdrawing());
    final result = await _withdrawTripUseCase(event.tripId);
    result.fold((failure) => emit(TripWithdrawError(failure.message)), (_) {
      emit(TripWithdrawn());
      add(GetTrips(pageSize: 50));
    });
  }
}
