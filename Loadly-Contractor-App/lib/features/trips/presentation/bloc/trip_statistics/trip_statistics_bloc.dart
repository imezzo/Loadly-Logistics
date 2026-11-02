// ignore_for_file: prefer_initializing_formals

import 'package:flutter_bloc/flutter_bloc.dart';

import '../../../domain/use_cases/get_trip_statistics_usecase.dart';
import 'trip_statistics_event.dart';
import 'trip_statistics_state.dart';

class TripStatisticsBloc
    extends Bloc<TripStatisticsEvent, TripStatisticsState> {
  TripStatisticsBloc({
    required GetInvitationStatisticsUseCase getInvitationStatisticsUseCase,
    required GetStatusStatisticsUseCase getStatusStatisticsUseCase,
    required GetPayoutStatisticsUseCase getPayoutStatisticsUseCase,
  }) : _getInvitationStatisticsUseCase = getInvitationStatisticsUseCase,
       _getStatusStatisticsUseCase = getStatusStatisticsUseCase,
       _getPayoutStatisticsUseCase = getPayoutStatisticsUseCase,
       super(const TripStatisticsInitial()) {
    on<GetInvitationStatistics>(_onGetInvitationStatistics);
    on<GetStatusStatistics>(_onGetStatusStatistics);
    on<GetPayoutStatistics>(_onGetPayoutStatistics);
    on<ResetTripStatistics>(_onReset);
  }

  final GetInvitationStatisticsUseCase _getInvitationStatisticsUseCase;
  final GetStatusStatisticsUseCase _getStatusStatisticsUseCase;
  final GetPayoutStatisticsUseCase _getPayoutStatisticsUseCase;

  Future<void> _onGetInvitationStatistics(
    GetInvitationStatistics event,
    Emitter<TripStatisticsState> emit,
  ) async {
    final result = await _getInvitationStatisticsUseCase();
    result.fold((_) {}, (stats) {
      emit(_ready.copyWith(invitationStatistics: stats));
    });
  }

  Future<void> _onGetStatusStatistics(
    GetStatusStatistics event,
    Emitter<TripStatisticsState> emit,
  ) async {
    final result = await _getStatusStatisticsUseCase();
    result.fold((_) {}, (stats) {
      emit(_ready.copyWith(statusStatistics: stats));
    });
  }

  Future<void> _onGetPayoutStatistics(
    GetPayoutStatistics event,
    Emitter<TripStatisticsState> emit,
  ) async {
    final result = await _getPayoutStatisticsUseCase();
    result.fold((_) {}, (stats) {
      emit(_ready.copyWith(payoutStatistics: stats));
    });
  }

  void _onReset(ResetTripStatistics event, Emitter<TripStatisticsState> emit) {
    emit(const TripStatisticsInitial());
  }

  TripStatisticsReady get _ready {
    final current = state;
    if (current is TripStatisticsReady) return current;
    return const TripStatisticsReady();
  }
}
