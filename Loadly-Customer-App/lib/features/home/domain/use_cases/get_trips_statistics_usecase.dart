import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../entities/trips_statistics_result.dart';
import '../failures/home_failure.dart';
import '../repositories/home_repository.dart';

class GetTripsStatisticsUseCase
    implements UseCase<TripsStatisticsResult, GetTripsStatisticsParams> {
  GetTripsStatisticsUseCase(this._repository);

  final HomeRepository _repository;

  @override
  Future<Either<HomeFailure, TripsStatisticsResult>> call(
    GetTripsStatisticsParams params,
  ) {
    return _repository.getTripsStatistics(
      year: params.year,
      lastMonth: params.lastMonth,
      lastWeek: params.lastWeek,
    );
  }
}

class GetTripsStatisticsParams {
  const GetTripsStatisticsParams({
    this.year,
    required this.lastMonth,
    required this.lastWeek,
  });

  final int? year;
  final bool lastMonth;
  final bool lastWeek;
}
