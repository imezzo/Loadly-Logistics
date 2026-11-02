// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:equatable/equatable.dart';

import '../../domain/entities/trip.dart';

abstract class TripsEvent extends Equatable {
  const TripsEvent();

  @override
  List<Object?> get props => [];
}

class TripsLoadTrips extends TripsEvent {
  const TripsLoadTrips({
    this.page,
    this.limit,
    this.status,
    this.search,
    this.vehicleTypeId,
    this.startDate,
    this.endDate,
    this.contractId,
  });

  final int? page;
  final int? limit;
  final int? status;
  final String? search;
  final int? vehicleTypeId;
  final int? contractId;
  final String? startDate;
  final String? endDate;

  @override
  List<Object?> get props => [
    page,
    limit,
    status,
    search,
    vehicleTypeId,
    contractId,
    startDate,
    endDate,
  ];
}

class TripsLoadTripDetails extends TripsEvent {
  const TripsLoadTripDetails({required this.tripId});

  final int tripId;

  @override
  List<Object?> get props => [tripId];
}

class TripsRefresh extends TripsEvent {
  const TripsRefresh();
}

class TripsLoadContracts extends TripsEvent {
  const TripsLoadContracts();
}

class TripsLoadVehicleTypes extends TripsEvent {
  const TripsLoadVehicleTypes({this.contractId});

  final int? contractId;

  @override
  List<Object?> get props => [contractId];
}

class TripsLoadConsumptionType extends TripsEvent {
  const TripsLoadConsumptionType({required this.contractId});

  final int contractId;

  @override
  List<Object?> get props => [contractId];
}

class TripsLoadContractDestinations extends TripsEvent {
  const TripsLoadContractDestinations({required this.contractId});

  final int contractId;

  @override
  List<Object?> get props => [contractId];
}

class TripsLoadContractDestination extends TripsEvent {
  const TripsLoadContractDestination({required this.destinationId});

  final int destinationId;

  @override
  List<Object?> get props => [destinationId];
}

class TripsExtractPlace extends TripsEvent {
  const TripsExtractPlace({required this.url});

  final String url;

  @override
  List<Object?> get props => [url];
}

class TripsAddtrip extends TripsEvent {
  const TripsAddtrip({
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

  @override
  List<Object?> get props => [
    contractId,
    destinationId,
    submitRequest,
    vehicleTypeId,
    startDate,
    endDate,
    contractDistanceKm,
    actualCargoWeightTons,
    cargoPrice,
    helpersCount,
    contactPersonName,
    contactPersonMobile,
    waypoints,
  ];
}

class TripsUpdatetrip extends TripsEvent {
  const TripsUpdatetrip({
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

  @override
  List<Object?> get props => [
    tripId,
    contractId,
    destinationId,
    submitRequest,
    vehicleTypeId,
    startDate,
    endDate,
    contractDistanceKm,
    actualCargoWeightTons,
    cargoPrice,
    helpersCount,
    contactPersonName,
    contactPersonMobile,
    waypoints,
  ];
}

class TripsDeletetrip extends TripsEvent {
  const TripsDeletetrip({required this.tripId});

  final int tripId;

  @override
  List<Object?> get props => [tripId];
}

class TripsTrackTrips extends TripsEvent {
  const TripsTrackTrips();
}
