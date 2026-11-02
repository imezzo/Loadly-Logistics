import 'dart:async';

import 'package:flutter_bloc/flutter_bloc.dart';

import '../../domain/entities/trips_statistics_result.dart';
import '../../domain/failures/home_failure.dart';
import '../../domain/use_cases/get_monthly_trips_kilometers_usecase.dart';
import '../../domain/use_cases/get_trips_statistics_usecase.dart';
import 'home_event.dart';
import 'home_state.dart';

class HomeBloc extends Bloc<HomeEvent, HomeState> {
  HomeBloc({
    required this.getTripsStatisticsUseCase,
    required this.getMonthlyTripsKilometersUseCase,
  }) : super(const HomeInitial()) {
    on<HomeLoadStatistics>(_onLoadStatistics);
  }

  final GetTripsStatisticsUseCase getTripsStatisticsUseCase;
  final GetMonthlyTripsKilometersUseCase getMonthlyTripsKilometersUseCase;

  Future<void> _onLoadStatistics(
    HomeLoadStatistics event,
    Emitter<HomeState> emit,
  ) async {
    emit(const StatisticsLoading());
    TripsStatisticsResult tripsStatistics = const TripsStatisticsResult(
      totalTrips: 0,
      scheduledTrips: 0,
      inProgressTrips: 0,
      canceledTrips: 0,
      completedTrips: 0,
      otherTrips: 0,
    );
    String? error;

    final tripsStatisticsResult = await getTripsStatisticsUseCase(
      GetTripsStatisticsParams(
        year: event.tripsStatisticsYear,
        lastMonth: event.lastMonth,
        lastWeek: event.lastWeek,
      ),
    );

    tripsStatisticsResult.fold(
      (failure) {
        error = failure is HomeFailureServer
            ? failure.message ?? 'Failed to load trips'
            : failure is HomeFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
      },
      (tripsStatistic) {
        tripsStatistics = tripsStatistic;
        error = null;
      },
    );

    final monthlyTripsKilometersResult = await getMonthlyTripsKilometersUseCase(
      GetMonthlyTripsKilometersParams(
        year: event.monthlyTripsKilometersYear,
        month: event.month,
      ),
    );

    monthlyTripsKilometersResult.fold(
      (failure) {
        final errorMessage = failure is HomeFailureServer
            ? failure.message ?? 'Failed to load Vehicle Types'
            : failure is HomeFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(
          StatisticsLoaded(
            tripsStatistics: tripsStatistics,
            monthlyTripsKilometers: [],
            tripsStatisticsError: error,
            monthlyTripsKilometersError: errorMessage,
          ),
        );
      },
      (monthlyTripsKilometers) {
        emit(
          StatisticsLoaded(
            tripsStatistics: tripsStatistics,
            monthlyTripsKilometers: monthlyTripsKilometers,
            tripsStatisticsError: error,
            monthlyTripsKilometersError: null,
          ),
        );
      },
    );
  }
}
