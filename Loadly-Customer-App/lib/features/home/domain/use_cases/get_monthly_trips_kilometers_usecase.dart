import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../entities/monthly_trips_kilometers_result.dart';
import '../failures/home_failure.dart';
import '../repositories/home_repository.dart';

class GetMonthlyTripsKilometersUseCase
    implements
        UseCase<
          List<MonthlyTripsKilometersResult>,
          GetMonthlyTripsKilometersParams
        > {
  GetMonthlyTripsKilometersUseCase(this._repository);

  final HomeRepository _repository;

  @override
  Future<Either<HomeFailure, List<MonthlyTripsKilometersResult>>> call(
    GetMonthlyTripsKilometersParams params,
  ) {
    return _repository.getMonthlyTripsKilometers(
      year: params.year,
      month: params.month,
    );
  }
}

class GetMonthlyTripsKilometersParams {
  const GetMonthlyTripsKilometersParams({this.year, this.month});

  final int? year;
  final int? month;
}
