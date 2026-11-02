// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../entities/collector_trips.dart';
import '../failures/home_failure.dart';
import '../repositories/home_repository.dart';

class GetTripsUseCase implements UseCase<CollectorTripsList, GetTripsParams> {
  GetTripsUseCase(this._repository);

  final HomeRepository _repository;

  @override
  Future<Either<HomeFailure, CollectorTripsList>> call(GetTripsParams params) {
    return _repository.getCollectorTrips(
      companyId: params.companyId,
      contractId: params.contractId,
      page: params.page,
      pageSize: params.pageSize,
      search: params.search,
      contractReferenceNumber: params.contractReferenceNumber,
      vehicleId: params.vehicleId,
      vehicleTypeId: params.vehicleTypeId,
      driverId: params.driverId,
      status: params.status,
      tripDocumentStatuses: params.tripDocumentStatuses,
      startDate: params.startDate,
      endDate: params.endDate,
      companyName: params.companyName,
      driverName: params.driverName,
      plate: params.plate,
    );
  }
}

class GetTripsParams {
  const GetTripsParams({
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
}
