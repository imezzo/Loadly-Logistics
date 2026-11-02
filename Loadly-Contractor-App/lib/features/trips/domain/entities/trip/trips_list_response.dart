import 'trip.dart';

class TripsListResponse {
  const TripsListResponse({
    required this.trips,
    this.pagination,
    this.additional,
    this.message,
    this.success,
  });

  final List<Trip> trips;
  final TripsPagination? pagination;
  final TripsListAdditional? additional;
  final String? message;
  final bool? success;

  int get totalCount => pagination?.totalCount ?? trips.length;
}

class TripsPagination {
  const TripsPagination({
    this.totalCount,
    this.page,
    this.pageSize,
    this.totalPages,
  });

  final int? totalCount;
  final int? page;
  final int? pageSize;
  final int? totalPages;
}

class TripsListAdditional {
  const TripsListAdditional({this.exportKeys});

  final dynamic exportKeys;
}
