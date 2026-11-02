// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:flutter_bloc/flutter_bloc.dart';

import '../../domain/entities/destination.dart';
import '../../domain/entities/extract_place_result.dart';
import '../../domain/entities/trip.dart';
import '../../domain/failures/trips_failure.dart';
import '../../domain/use_cases/add_trip_usecase.dart';
import '../../domain/use_cases/delete_trip_usecase.dart';
import '../../domain/use_cases/extract_place_usecase.dart';
import '../../domain/use_cases/get_contracts_usecase.dart';
import '../../domain/use_cases/get_trip_details_usecase.dart';
import '../../domain/use_cases/get_trips_usecase.dart';
import '../../domain/use_cases/track_trips_usecase.dart';
import '../../domain/use_cases/update_trip_usecase.dart';
import 'trips_event.dart';
import 'trips_state.dart';

class TripsBloc extends Bloc<TripsEvent, TripsState> {
  TripsBloc({
    required this.getTripsUseCase,
    required this.getTripDetailsUseCase,
    required this.getContractReferenceNumbersUseCase,
    required this.getVehicleTypesUseCase,
    required this.getConsumptionTypeUseCase,
    required this.getContractDestinationsUseCase,
    required this.getContractDestinationUseCase,
    required this.extractPlaceUseCase,
    required this.addTripUseCase,
    required this.updateTripUseCase,
    required this.deleteTripUsecase,
    required this.trackTripsUsecase,
  }) : super(const TripsInitial()) {
    on<TripsLoadTrips>(_onLoadTrips);
    on<TripsLoadTripDetails>(_onLoadTripDetails);
    on<TripsRefresh>(_onRefresh);
    on<TripsLoadContracts>(_onLoadContracts);
    on<TripsLoadVehicleTypes>(_onLoadVehicleTypes);
    on<TripsLoadConsumptionType>(_onLoadConsumptionType);
    on<TripsLoadContractDestinations>(_onLoadContractDestinations);
    on<TripsLoadContractDestination>(_onLoadContractDestination);
    on<TripsExtractPlace>(_onExtractPlace);
    on<TripsAddtrip>(_onAddtrip);
    on<TripsUpdatetrip>(_onUpdatetrip);
    on<TripsDeletetrip>(_onDeletetrip);
    on<TripsTrackTrips>(_onTrackTrips);
  }

  final GetTripsUseCase getTripsUseCase;
  final GetTripDetailsUseCase getTripDetailsUseCase;
  final GetContractReferenceNumbersUseCase getContractReferenceNumbersUseCase;
  final GetVehicleTypesUseCase getVehicleTypesUseCase;
  final GetConsumptionTypeUseCase getConsumptionTypeUseCase;
  final GetContractDestinationsUseCase getContractDestinationsUseCase;
  final GetContractDestinationUseCase getContractDestinationUseCase;
  final ExtractPlaceUseCase extractPlaceUseCase;
  final AddTripUseCase addTripUseCase;
  final UpdateTripUseCase updateTripUseCase;
  final DeleteTripUsecase deleteTripUsecase;
  final TrackTripsUsecase trackTripsUsecase;

  Future<void> _onLoadTrips(
    TripsLoadTrips event,
    Emitter<TripsState> emit,
  ) async {
    emit(const TripsLoading());

    final result = await getTripsUseCase(
      GetTripsParams(
        page: event.page,
        limit: event.limit,
        status: event.status,
        search: event.search,
        vehicleTypeId: event.vehicleTypeId,
        contractId: event.contractId,
        startDate: event.startDate,
        endDate: event.endDate,
      ),
    );

    result.fold(
      (failure) {
        final errorMessage = failure is TripsFailureServer
            ? failure.message ?? 'Failed to load trips'
            : failure is TripsFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(TripsLoaded(tripsList: const TripsList(), error: errorMessage));
      },
      (tripsList) {
        emit(TripsLoaded(tripsList: tripsList, error: null));
      },
    );
  }

  Future<void> _onLoadTripDetails(
    TripsLoadTripDetails event,
    Emitter<TripsState> emit,
  ) async {
    emit(const TripDetailsLoading());

    final result = await getTripDetailsUseCase(
      GetTripDetailsParams(tripId: event.tripId),
    );

    result.fold(
      (failure) {
        final errorMessage = failure is TripsFailureServer
            ? failure.message ?? 'Failed to load trip details'
            : failure is TripsFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(TripDetailsLoaded(trip: _getEmptyTrip(), error: errorMessage));
      },
      (trip) {
        emit(TripDetailsLoaded(trip: trip, error: null));
      },
    );
  }

  Future<void> _onRefresh(TripsRefresh event, Emitter<TripsState> emit) async {
    final currentState = state;
    if (currentState is TripsLoaded) {
      add(const TripsLoadTrips());
    }
  }

  Trip _getEmptyTrip() {
    return const Trip();
  }

  Future<void> _onLoadContracts(
    TripsLoadContracts event,
    Emitter<TripsState> emit,
  ) async {
    emit(const ContractsLoading());

    final result = await getContractReferenceNumbersUseCase(
      GetContractReferenceNumbersParams(),
    );

    result.fold(
      (failure) {
        final errorMessage = failure is TripsFailureServer
            ? failure.message ?? 'Failed to load Contracts'
            : failure is TripsFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(ContractsLoaded(contracts: [], error: errorMessage));
      },
      (contracts) {
        emit(ContractsLoaded(contracts: contracts, error: null));
      },
    );
  }

  Future<void> _onLoadVehicleTypes(
    TripsLoadVehicleTypes event,
    Emitter<TripsState> emit,
  ) async {
    emit(const VehicleTypesLoading());

    final result = await getVehicleTypesUseCase(
      GetVehicleTypesParams(contractId: event.contractId),
    );

    result.fold(
      (failure) {
        final errorMessage = failure is TripsFailureServer
            ? failure.message ?? 'Failed to load Vehicle Types'
            : failure is TripsFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(VehicleTypesLoaded(vehicleTypes: [], error: errorMessage));
      },
      (vehicleTypes) {
        emit(VehicleTypesLoaded(vehicleTypes: vehicleTypes, error: null));
      },
    );
  }

  Future<void> _onLoadConsumptionType(
    TripsLoadConsumptionType event,
    Emitter<TripsState> emit,
  ) async {
    final result = await getConsumptionTypeUseCase(
      GetConsumptionTypeParams(contractId: event.contractId),
    );

    result.fold(
      (failure) {
        final errorMessage = failure is TripsFailureServer
            ? failure.message ?? 'Failed to load Consumption Type'
            : failure is TripsFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(ConsumptionTypeLoaded(consumptionType: 0, error: errorMessage));
      },
      (consumptionType) {
        emit(
          ConsumptionTypeLoaded(consumptionType: consumptionType, error: null),
        );
      },
    );
  }

  Future<void> _onLoadContractDestinations(
    TripsLoadContractDestinations event,
    Emitter<TripsState> emit,
  ) async {
    emit(const DestinationLoading());

    final result = await getContractDestinationsUseCase(
      GetContractDestinationsParams(contractId: event.contractId),
    );

    result.fold(
      (failure) {
        final errorMessage = failure is TripsFailureServer
            ? failure.message ?? 'Failed to load Contract Destinations'
            : failure is TripsFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(DestinationsLoaded(destinations: [], error: errorMessage));
      },
      (destinations) {
        emit(DestinationsLoaded(destinations: destinations, error: null));
      },
    );
  }

  Future<void> _onLoadContractDestination(
    TripsLoadContractDestination event,
    Emitter<TripsState> emit,
  ) async {
    emit(const DestinationLoading());

    final result = await getContractDestinationUseCase(
      GetContractDestinationParams(destinationId: event.destinationId),
    );

    result.fold(
      (failure) {
        final errorMessage = failure is TripsFailureServer
            ? failure.message ?? 'Failed to load Contract Destination'
            : failure is TripsFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(
          DestinationLoaded(
            destination: const Destination(),
            error: errorMessage,
          ),
        );
      },
      (destination) {
        emit(DestinationLoaded(destination: destination, error: null));
      },
    );
  }

  Future<void> _onExtractPlace(
    TripsExtractPlace event,
    Emitter<TripsState> emit,
  ) async {
    emit(const ExtractPlaceLoading());

    final result = await extractPlaceUseCase(
      ExtractPlaceParams(url: event.url),
    );

    result.fold(
      (failure) {
        final errorMessage = failure is TripsFailureServer
            ? failure.message ?? 'Failed to load place'
            : failure is TripsFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(
          ExtractPlaceLoaded(
            extractedPlace: const ExtractPlaceResult(),
            error: errorMessage,
          ),
        );
      },
      (extractedPlace) {
        emit(ExtractPlaceLoaded(extractedPlace: extractedPlace, error: null));
      },
    );
  }

  Future<void> _onAddtrip(TripsAddtrip event, Emitter<TripsState> emit) async {
    emit(const AddTripLoading());

    final result = await addTripUseCase(
      AddTripParams(
        contractId: event.contractId,
        destinationId: event.destinationId,
        submitRequest: event.submitRequest,
        vehicleTypeId: event.vehicleTypeId,
        startDate: event.startDate,
        endDate: event.endDate,
        contractDistanceKm: event.contractDistanceKm,
        actualCargoWeightTons: event.actualCargoWeightTons,
        cargoPrice: event.cargoPrice,
        helpersCount: event.helpersCount,
        contactPersonName: event.contactPersonName,
        contactPersonMobile: event.contactPersonMobile,
        waypoints: event.waypoints,
      ),
    );

    result.fold(
      (failure) {
        final errorMessage = failure is TripsFailureServer
            ? failure.message ?? 'Failed to add trip'
            : failure is TripsFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(AddTripDone(error: errorMessage));
      },
      (_) {
        emit(const AddTripDone(error: null));
      },
    );
  }

  Future<void> _onUpdatetrip(
    TripsUpdatetrip event,
    Emitter<TripsState> emit,
  ) async {
    emit(const UpdateTripLoading());

    final result = await updateTripUseCase(
      UpdateTripParams(
        tripId: event.tripId,
        contractId: event.contractId,
        destinationId: event.destinationId,
        submitRequest: event.submitRequest,
        vehicleTypeId: event.vehicleTypeId,
        startDate: event.startDate,
        endDate: event.endDate,
        contractDistanceKm: event.contractDistanceKm,
        actualCargoWeightTons: event.actualCargoWeightTons,
        cargoPrice: event.cargoPrice,
        helpersCount: event.helpersCount,
        contactPersonName: event.contactPersonName,
        contactPersonMobile: event.contactPersonMobile,
        waypoints: event.waypoints,
      ),
    );

    result.fold(
      (failure) {
        final errorMessage = failure is TripsFailureServer
            ? failure.message ?? 'Failed to update trip'
            : failure is TripsFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(UpdateTripDone(error: errorMessage));
      },
      (_) {
        emit(const UpdateTripDone(error: null));
      },
    );
  }

  Future<void> _onDeletetrip(
    TripsDeletetrip event,
    Emitter<TripsState> emit,
  ) async {
    emit(const DeleteTripLoading());

    final result = await deleteTripUsecase(
      DeleteTripParams(tripId: event.tripId),
    );

    result.fold(
      (failure) {
        final errorMessage = failure is TripsFailureServer
            ? failure.message ?? 'Failed to delete trip'
            : failure is TripsFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(DeleteTripDone(error: errorMessage));
      },
      (_) {
        emit(const DeleteTripDone(error: null));
      },
    );
  }

  Future<void> _onTrackTrips(
    TripsTrackTrips event,
    Emitter<TripsState> emit,
  ) async {
    emit(const TrackTripsLoading());

    final result = await trackTripsUsecase(TrackTripsParams());

    result.fold(
      (failure) {
        final errorMessage = failure is TripsFailureServer
            ? failure.message ?? 'Failed to track trips'
            : failure is TripsFailureNetwork
            ? 'Network error. Please check your connection.'
            : 'An unexpected error occurred';
        emit(TrackTripsLoaded(trackedTrips: [], error: errorMessage));
      },
      (trackedTrips) {
        emit(TrackTripsLoaded(trackedTrips: trackedTrips, error: null));
      },
    );
  }
}
