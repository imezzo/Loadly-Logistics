import 'package:equatable/equatable.dart';

import '../../domain/entities/destination.dart';
import '../../domain/entities/extract_place_result.dart';
import '../../domain/entities/lookup.dart';
import '../../domain/entities/track_trips_result.dart';
import '../../domain/entities/trip.dart' hide Contract;

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
  const TripsLoaded({required this.tripsList, this.error});

  final TripsList tripsList;
  final String? error;

  TripsLoaded copyWith({TripsList? tripsList, String? error}) {
    return TripsLoaded(tripsList: tripsList ?? this.tripsList, error: error);
  }

  @override
  List<Object?> get props => [tripsList, error];
}

class TripDetailsLoading extends TripsState {
  const TripDetailsLoading();
}

class TripDetailsLoaded extends TripsState {
  const TripDetailsLoaded({required this.trip, this.error});

  final Trip trip;
  final String? error;

  TripDetailsLoaded copyWith({Trip? trip, String? error}) {
    return TripDetailsLoaded(trip: trip ?? this.trip, error: error);
  }

  @override
  List<Object?> get props => [trip, error];
}

class ContractsLoading extends TripsState {
  const ContractsLoading();
}

class ContractsLoaded extends TripsState {
  const ContractsLoaded({required this.contracts, this.error});

  final List<Lookup> contracts;
  final String? error;
}

class VehicleTypesLoading extends TripsState {
  const VehicleTypesLoading();
}

class VehicleTypesLoaded extends TripsState {
  const VehicleTypesLoaded({required this.vehicleTypes, this.error});

  final List<Lookup> vehicleTypes;
  final String? error;
}

class ConsumptionTypeLoaded extends TripsState {
  const ConsumptionTypeLoaded({required this.consumptionType, this.error});

  final int consumptionType;
  final String? error;
}

class DestinationLoading extends TripsState {
  const DestinationLoading();
}

class DestinationsLoaded extends TripsState {
  const DestinationsLoaded({required this.destinations, this.error});

  final List<Lookup> destinations;
  final String? error;
}

class DestinationLoaded extends TripsState {
  const DestinationLoaded({required this.destination, this.error});

  final Destination destination;
  final String? error;
}

class ExtractPlaceLoading extends TripsState {
  const ExtractPlaceLoading();
}

class ExtractPlaceLoaded extends TripsState {
  const ExtractPlaceLoaded({required this.extractedPlace, this.error});

  final ExtractPlaceResult extractedPlace;
  final String? error;

  ExtractPlaceLoaded copyWith({
    ExtractPlaceResult? extractedPlace,
    String? error,
  }) {
    return ExtractPlaceLoaded(
      extractedPlace: extractedPlace ?? this.extractedPlace,
      error: error,
    );
  }

  @override
  List<Object?> get props => [extractedPlace, error];
}

class AddTripLoading extends TripsState {
  const AddTripLoading();
}

class AddTripDone extends TripsState {
  const AddTripDone({this.error});

  final String? error;

  AddTripDone copyWith({String? error}) {
    return AddTripDone(error: error);
  }
}

class UpdateTripLoading extends TripsState {
  const UpdateTripLoading();
}

class UpdateTripDone extends TripsState {
  const UpdateTripDone({this.error});

  final String? error;

  UpdateTripDone copyWith({String? error}) {
    return UpdateTripDone(error: error);
  }
}

class DeleteTripLoading extends TripsState {
  const DeleteTripLoading();
}

class DeleteTripDone extends TripsState {
  const DeleteTripDone({this.error});

  final String? error;

  DeleteTripDone copyWith({String? error}) {
    return DeleteTripDone(error: error);
  }
}

class TrackTripsLoading extends TripsState {
  const TrackTripsLoading();
}

class TrackTripsLoaded extends TripsState {
  const TrackTripsLoaded({required this.trackedTrips, this.error});

  final List<TrackTripsResult> trackedTrips;
  final String? error;

  TrackTripsLoaded copyWith({
    List<TrackTripsResult>? trackedTrips,
    String? error,
  }) {
    return TrackTripsLoaded(
      trackedTrips: trackedTrips ?? this.trackedTrips,
      error: error,
    );
  }

  @override
  List<Object?> get props => [trackedTrips, error];
}
