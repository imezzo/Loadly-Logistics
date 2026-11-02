sealed class TripStatisticsEvent {}

class GetInvitationStatistics extends TripStatisticsEvent {}

class GetStatusStatistics extends TripStatisticsEvent {}

class GetPayoutStatistics extends TripStatisticsEvent {}

class ResetTripStatistics extends TripStatisticsEvent {}
