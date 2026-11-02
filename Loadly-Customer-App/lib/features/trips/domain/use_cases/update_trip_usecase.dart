import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../entities/trip.dart';
import '../failures/trips_failure.dart';
import '../repositories/trips_repository.dart';

class UpdateTripUseCase implements UseCase<void, UpdateTripParams> {
  UpdateTripUseCase(this._repository);

  final TripsRepository _repository;

  @override
  Future<Either<TripsFailure, void>> call(UpdateTripParams params) {
    return _repository.updateTrip(
      tripId: params.tripId,
      contractId: params.contractId,
      destinationId: params.destinationId,
      submitRequest: params.submitRequest,
      vehicleTypeId: params.vehicleTypeId,
      startDate: params.startDate,
      endDate: params.endDate,
      contractDistanceKm: params.contractDistanceKm,
      actualCargoWeightTons: params.actualCargoWeightTons,
      cargoPrice: params.cargoPrice,
      helpersCount: params.helpersCount,
      contactPersonName: params.contactPersonName,
      contactPersonMobile: params.contactPersonMobile,
      waypoints: params.waypoints,
    );
  }
}

class UpdateTripParams {
  const UpdateTripParams({
    this.tripId,
    this.contractId,
    this.destinationId,
    this.submitRequest,
    this.vehicleTypeId,
    this.startDate,
    this.endDate,
    this.contractDistanceKm,
    this.actualCargoWeightTons,
    this.cargoPrice,
    this.helpersCount,
    this.contactPersonName,
    this.contactPersonMobile,
    this.waypoints,
  });

  final int? tripId;
  final int? contractId;
  final int? destinationId;
  final bool? submitRequest;
  final int? vehicleTypeId;
  final String? startDate;
  final String? endDate;
  final double? contractDistanceKm;
  final double? actualCargoWeightTons;
  final double? cargoPrice;
  final int? helpersCount;
  final String? contactPersonName;
  final String? contactPersonMobile;
  final List<Waypoint>? waypoints;
}
