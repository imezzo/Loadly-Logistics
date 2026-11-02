class InvitationStatistics {
  const InvitationStatistics({
    this.openInvitationTrips = 0,
    this.pendingBookingRequestTrips = 0,
  });

  final int openInvitationTrips;
  final int pendingBookingRequestTrips;

  static const empty = InvitationStatistics();
}

class TripStatusStatistics {
  const TripStatusStatistics({
    this.completedTrips = 0,
    this.scheduledTrips = 0,
    this.inProgressTrips = 0,
    this.pendingTrips = 0,
  });

  final int completedTrips;
  final int scheduledTrips;
  final int inProgressTrips;
  final int pendingTrips;

  static const empty = TripStatusStatistics();
}

class PayoutStatistics {
  const PayoutStatistics({
    this.completedPaidTrips = 0,
    this.completedUnpaidTrips = 0,
    this.completedPaidTripsTotalAmount = 0,
    this.completedUnpaidTripsTotalAmount = 0,
  });

  final int completedPaidTrips;
  final int completedUnpaidTrips;
  final double completedPaidTripsTotalAmount;
  final double completedUnpaidTripsTotalAmount;

  static const empty = PayoutStatistics();
}
