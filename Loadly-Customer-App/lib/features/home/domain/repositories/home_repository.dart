import 'package:dartz/dartz.dart';

import '../entities/monthly_trips_kilometers_result.dart';
import '../entities/trips_statistics_result.dart';
import '../failures/home_failure.dart';

abstract interface class HomeRepository {
  Future<Either<HomeFailure, TripsStatisticsResult>> getTripsStatistics({
    int? year,
    required bool lastMonth,
    required bool lastWeek,
  });
  Future<Either<HomeFailure, List<MonthlyTripsKilometersResult>>>
  getMonthlyTripsKilometers({int? year, int? month});
}
