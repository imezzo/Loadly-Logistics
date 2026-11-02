import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../entities/get_trip_cost_period_result.dart';
import '../failures/reports_failure.dart';
import '../repositories/reports_repository.dart';

class GetTripCostPeriodUseCase
    implements UseCase<GetTripCostPeriodResult, GetTripCostPeriodParams> {
  GetTripCostPeriodUseCase(this._repository);

  final ReportsRepository _repository;

  @override
  Future<Either<ReportsFailure, GetTripCostPeriodResult>> call(
    GetTripCostPeriodParams params,
  ) {
    return _repository.getTripCostPeriod();
  }
}

class GetTripCostPeriodParams {}
