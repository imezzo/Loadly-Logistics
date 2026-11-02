import 'package:equatable/equatable.dart';

class TripsStatisticsResult extends Equatable {
  final int totalTrips;
  final int scheduledTrips;
  final int inProgressTrips;
  final int canceledTrips;
  final int completedTrips;
  final int otherTrips;

  const TripsStatisticsResult({
    required this.totalTrips,
    required this.scheduledTrips,
    required this.inProgressTrips,
    required this.canceledTrips,
    required this.completedTrips,
    required this.otherTrips,
  });

  @override
  List<Object?> get props {
    return [
      totalTrips,
      scheduledTrips,
      inProgressTrips,
      canceledTrips,
      completedTrips,
      otherTrips,
    ];
  }
}
