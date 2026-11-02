import 'trip.dart';

/// Result of a paginated trips list request.
class TripsListResult {
  const TripsListResult({
    required this.trips,
    this.total,
    this.page = 1,
    this.pageSize = 10,
  });

  final List<Trip> trips;
  final int? total;
  final int page;
  final int pageSize;

  bool get hasMore => total != null && trips.length < total!;
}
