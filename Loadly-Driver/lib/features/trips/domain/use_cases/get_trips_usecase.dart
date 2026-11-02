import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../entities/trips_list_result.dart';
import '../failures/trips_failure.dart';
import '../repositories/trips_repository.dart';

class GetTripsUseCase implements UseCase<TripsListResult, GetTripsParams> {
  GetTripsUseCase(this._repository);

  final TripsRepository _repository;

  @override
  Future<Either<TripsFailure, TripsListResult>> call(GetTripsParams params) {
    return _repository.getTrips(
      page: params.page,
      limit: params.limit,
      status: params.status,
      startDateFrom: params.startDateFrom,
      startDateTo: params.startDateTo,
    );
  }
}

class GetTripsParams {
  const GetTripsParams({
    this.page,
    this.limit,
    this.status,
    this.startDateFrom,
    this.startDateTo,
  });

  final int? page;
  final int? limit;
  final int? status;
  final String? startDateFrom;
  final String? startDateTo;
}
