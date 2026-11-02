import '../../../domain/entities/trip/trip.dart';
import 'trip_event.dart';

sealed class TripState {}

class TripInitial extends TripState {}

class TripDetailLoading extends TripState {
  TripDetailLoading(this.tripId);

  final int tripId;
}

class TripDetailLoaded extends TripState {
  TripDetailLoaded(this.trip);

  final Trip trip;
}

class TripDetailError extends TripState {
  TripDetailError(this.message, {this.tripId});

  final String message;
  final int? tripId;
}

class TripLoading extends TripState {}

class TripLoaded extends TripState {
  TripLoaded(
    this.trips, {
    int? totalCount,
    required this.query,
    this.currentPage = 1,
    this.pageSize = 10,
    this.isLoadingMore = false,
  }) : totalCount = totalCount ?? trips.length;

  final List<Trip> trips;
  final int totalCount;
  final TripsListQuery query;

  final int currentPage;
  final int pageSize;
  final bool isLoadingMore;

  bool get hasMore => trips.length < totalCount;

  TripLoaded copyWith({
    List<Trip>? trips,
    int? totalCount,
    TripsListQuery? query,
    int? currentPage,
    int? pageSize,
    bool? isLoadingMore,
  }) {
    return TripLoaded(
      trips ?? this.trips,
      totalCount: totalCount ?? this.totalCount,
      query: query ?? this.query,
      currentPage: currentPage ?? this.currentPage,
      pageSize: pageSize ?? this.pageSize,
      isLoadingMore: isLoadingMore ?? this.isLoadingMore,
    );
  }
}

class TripError extends TripState {
  TripError(this.message);

  final String message;
}

class TripBooking extends TripState {}

class TripBooked extends TripState {}

class TripBookError extends TripState {
  TripBookError(this.message);

  final String message;
}

class TripWithdrawing extends TripState {}

class TripWithdrawn extends TripState {}

class TripWithdrawError extends TripState {
  TripWithdrawError(this.message);

  final String message;
}
