import '../../../domain/entities/trip/trip_statistics.dart';

sealed class TripStatisticsState {
  const TripStatisticsState();
}

class TripStatisticsInitial extends TripStatisticsState {
  const TripStatisticsInitial();
}

class TripStatisticsReady extends TripStatisticsState {
  const TripStatisticsReady({
    this.invitationStatistics,
    this.statusStatistics,
    this.payoutStatistics,
  });

  final InvitationStatistics? invitationStatistics;
  final TripStatusStatistics? statusStatistics;
  final PayoutStatistics? payoutStatistics;

  TripStatisticsReady copyWith({
    InvitationStatistics? invitationStatistics,
    TripStatusStatistics? statusStatistics,
    PayoutStatistics? payoutStatistics,
  }) {
    return TripStatisticsReady(
      invitationStatistics: invitationStatistics ?? this.invitationStatistics,
      statusStatistics: statusStatistics ?? this.statusStatistics,
      payoutStatistics: payoutStatistics ?? this.payoutStatistics,
    );
  }
}
