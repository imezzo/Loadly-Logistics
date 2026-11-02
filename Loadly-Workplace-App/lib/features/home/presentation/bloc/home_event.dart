// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:equatable/equatable.dart';

/// Events for Home BLoC
abstract class HomeEvent extends Equatable {
  const HomeEvent();

  @override
  List<Object?> get props => [];
}

class HomeLoadTrips extends HomeEvent {
  const HomeLoadTrips({
    this.page,
    this.pageSize,
    this.search,
    this.contractId,
    this.contractReferenceNumber,
    this.companyId,
    this.vehicleId,
    this.vehicleTypeId,
    this.driverId,
    this.status,
    this.tripDocumentStatuses,
    this.startDate,
    this.endDate,
    this.companyName,
    this.driverName,
    this.plate,
  });

  final int? page;
  final int? pageSize;
  final String? search;
  final int? contractId;
  final String? contractReferenceNumber;
  final int? companyId;
  final int? vehicleId;
  final int? vehicleTypeId;
  final int? driverId;
  final int? status;
  final List<int>? tripDocumentStatuses;
  final String? startDate;
  final String? endDate;
  final String? companyName;
  final String? driverName;
  final String? plate;

  @override
  List<Object?> get props => [
    page,
    pageSize,
    search,
    contractId,
    contractReferenceNumber,
    companyId,
    vehicleId,
    vehicleTypeId,
    driverId,
    status,
    tripDocumentStatuses,
    startDate,
    endDate,
    companyName,
    driverName,
    plate,
  ];
}

class HomeLoadDocuments extends HomeEvent {
  const HomeLoadDocuments();
}
