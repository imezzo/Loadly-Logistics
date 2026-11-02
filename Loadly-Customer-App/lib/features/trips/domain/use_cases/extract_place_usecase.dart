import 'package:dartz/dartz.dart';

import '../../../auth/domain/use_cases/usecase.dart';
import '../entities/extract_place_result.dart';
import '../failures/trips_failure.dart';
import '../repositories/trips_repository.dart';

class ExtractPlaceUseCase
    implements UseCase<ExtractPlaceResult, ExtractPlaceParams> {
  ExtractPlaceUseCase(this._repository);

  final TripsRepository _repository;

  @override
  Future<Either<TripsFailure, ExtractPlaceResult>> call(
    ExtractPlaceParams params,
  ) {
    return _repository.extractPlace(params.url);
  }
}

class ExtractPlaceParams {
  const ExtractPlaceParams({required this.url});

  final String url;
}
