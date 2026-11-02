// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:equatable/equatable.dart';

import '../../domain/entities/trip.dart';

abstract class TripsState extends Equatable {
  const TripsState();

  @override
  List<Object?> get props => [];
}

class TripsInitial extends TripsState {
  const TripsInitial();
}

class TripsLoading extends TripsState {
  const TripsLoading();
}

class TripsLoaded extends TripsState {
  const TripsLoaded({
    required this.trips,
    this.error,
    this.page = 1,
    this.total,
    this.pageSize = 10,
  });

  final List<Trip> trips;
  final String? error;
  final int page;
  final int? total;
  final int pageSize;

  bool get hasMore => total != null && trips.length < total!;

  TripsLoaded copyWith({
    List<Trip>? trips,
    String? error,
    int? page,
    int? total,
    int? pageSize,
  }) {
    return TripsLoaded(
      trips: trips ?? this.trips,
      error: error,
      page: page ?? this.page,
      total: total ?? this.total,
      pageSize: pageSize ?? this.pageSize,
    );
  }

  @override
  List<Object?> get props => [trips, error, page, total, pageSize];
}

class TripDetailsLoading extends TripsState {
  const TripDetailsLoading();
}

class TripDetailsLoaded extends TripsState {
  const TripDetailsLoaded({
    required this.trip,
    this.error,
    this.documentUrlToOpen,
    this.tripJustCompleted = false,
  });

  final Trip trip;
  final String? error;

  /// When set, UI should open this URL (e.g. document download) then clear.
  final String? documentUrlToOpen;

  /// When true, trip was just completed successfully - navigate to success screen.
  final bool tripJustCompleted;

  TripDetailsLoaded copyWith({
    Trip? trip,
    String? error,
    String? documentUrlToOpen,
    bool? tripJustCompleted,
  }) {
    return TripDetailsLoaded(
      trip: trip ?? this.trip,
      error: error,
      documentUrlToOpen: documentUrlToOpen,
      tripJustCompleted: tripJustCompleted ?? this.tripJustCompleted,
    );
  }

  @override
  List<Object?> get props => [
    trip,
    error,
    documentUrlToOpen,
    tripJustCompleted,
  ];
}

class TripActionInProgress extends TripsState {
  const TripActionInProgress({required this.trip, required this.message});

  final Trip trip;
  final String message;

  @override
  List<Object?> get props => [trip, message];
}
