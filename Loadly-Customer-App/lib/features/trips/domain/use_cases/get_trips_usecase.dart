import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../entities/trip.dart';
import '../failures/trips_failure.dart';
import '../repositories/trips_repository.dart';

class GetTripsUseCase implements UseCase<TripsList, GetTripsParams> {
  GetTripsUseCase(this._repository);

  final TripsRepository _repository;

  @override
  Future<Either<TripsFailure, TripsList>> call(GetTripsParams params) {
    return _repository.getTrips(
      page: params.page,
      limit: params.limit,
      status: params.status,
      search: params.search,
      vehicleTypeId: params.vehicleTypeId,
      contractId: params.contractId,
      startDate: params.startDate,
      endDate: params.endDate,
    );
  }
}

class GetTripsParams {
  const GetTripsParams({
    this.page,
    this.limit,
    this.status,
    this.search,
    this.vehicleTypeId,
    this.contractId,
    this.startDate,
    this.endDate,
  });

  final int? page;
  final int? limit;
  final int? status;
  final String? search;
  final int? vehicleTypeId;
  final int? contractId;
  final String? startDate;
  final String? endDate;
}
